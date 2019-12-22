using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileService.Controllers.Api.Tests
{
    [TestClass()]
    public class FileControllerTests
    {

        [TestMethod()]
        public async Task GetSuccess()
        {
            var fileController = new FileController(Mock.Of<ILogger<FileController>>(), Configuration());

            var result = await fileController.Get("readme.txt");
            Assert.IsTrue(result is FileStreamResult);
        }

        [TestMethod()]
        public async Task GetError400OnEmptyFileName()
        {
            var fileController = new FileController(Mock.Of<ILogger<FileController>>(), Configuration());

            var result = await fileController.Get(string.Empty);
            Assert.IsTrue(result is StatusCodeResult);
            Assert.AreEqual(400, ((StatusCodeResult)result).StatusCode);
        }

        [TestMethod()]
        public async Task GetError404OnFileNotExists()
        {
            var fileController = new FileController(Mock.Of<ILogger<FileController>>(), Configuration());

            var result = await fileController.Get("readme.jpg");
            Assert.IsTrue(result is NotFoundResult);
        }

        [TestMethod()]
        public async Task GetError500OnConnectionError()
        {
            var fileControllerMock = new Mock<FileController>(Mock.Of<ILogger<FileController>>(), Configuration());
            fileControllerMock.Setup(c => c.FileShare1).Returns("zzz"); // Set incorrect file share name

            var fileController = fileControllerMock.Object;

            var result = await fileController.Get("readme.txt");
            Assert.IsTrue(result is StatusCodeResult);
            Assert.AreEqual(500, ((StatusCodeResult)result).StatusCode);
        }

        private IConfiguration Configuration()
        {
            string projectPath = AppDomain.CurrentDomain.BaseDirectory.Split(new string[] { @"FileService.Tests\" }, StringSplitOptions.None)[0];
            return new ConfigurationBuilder()
                .SetBasePath(projectPath)
                .AddJsonFile("FileService\\appsettings.Development.json")
                .Build();
        }
    }
}