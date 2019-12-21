using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.File;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace FileService.Controllers.Api
{
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly ILogger<FileController> _logger;
        private readonly IConfiguration _configuration;

        public FileController(ILogger<FileController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        CloudStorageAccount StorageAccount1 => CloudStorageAccount.Parse(
            _configuration.GetConnectionString("Storage1ConnectionString"));

        CloudStorageAccount StorageAccount2 => CloudStorageAccount.Parse(
            _configuration.GetConnectionString("Storage2ConnectionString"));

        public virtual string FileShare1 => _configuration.GetValue<string>("FileShare1");
        public virtual string FileShare2 => _configuration.GetValue<string>("FileShare2");

        [Route("api/file/{name?}")]
        public async Task<IActionResult> Get(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                _logger.LogWarning("File name parameter is missing");
                return new StatusCodeResult(400);
            }

            var fileClient1 = StorageAccount1.CreateCloudFileClient();
            var fileShare1 = fileClient1.GetShareReference(FileShare1);

            if (fileShare1.Exists())
            {
                var file = fileShare1.GetRootDirectoryReference().GetFileReference(name);

                if (file.Exists())
                {
                    try
                    {
                        MemoryStream downloadStream = new MemoryStream();
                        await file.DownloadToStreamAsync(downloadStream).ConfigureAwait(false);
                        downloadStream.Position = 0;

                        _logger.LogInformation($"File served {name}");
                        return File(downloadStream, "application/octet-stream", name);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError($"Failed to download and serve file {name}: {ex.Message}");
                        return new StatusCodeResult(500);
                    }
                }
                else
                {
                    var fileClient2 = StorageAccount2.CreateCloudFileClient();
                    var fileShare2 = fileClient2.GetShareReference(FileShare2);

                    if (fileShare2.Exists())
                    {
                        var fileOnShare2 = fileShare2.GetRootDirectoryReference().GetFileReference(name);

                        if (fileOnShare2.Exists())
                        {
                            try
                            {
                                var fileStream = await CopyCloudFileWithStreamOutputAsync(fileOnShare2, file);

                                _logger.LogInformation($"File served {name}");
                                return File(fileStream, "application/octet-stream", name);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError($"Failed to copy and serve file {name}: {ex.Message}");
                                return new StatusCodeResult(500);
                            }
                        }
                        else
                        {
                            _logger.LogWarning($"File {name} is not found in either of the storages");
                            return NotFound();
                        }
                    }
                    else
                    {
                        _logger.LogError($"Couldn't connect to share {FileShare2}");
                        return new StatusCodeResult(500);
                    }
                }
            }
            else
            {
                _logger.LogError($"Couldn't connect to share {FileShare1}");
                return new StatusCodeResult(500);
            }
        }

        private async Task<Stream> CopyCloudFileWithStreamOutputAsync(CloudFile source, CloudFile destination)
        {
            MemoryStream downloadStream = new MemoryStream();
            await source.DownloadToStreamAsync(downloadStream).ConfigureAwait(false);
            downloadStream.Position = 0;

            using MemoryStream upload = new MemoryStream();
            downloadStream.CopyTo(upload);
            upload.Position = 0;

            await destination.UploadFromStreamAsync(upload).ConfigureAwait(false);

            downloadStream.Position = 0;
            return downloadStream;
        }
    }
}