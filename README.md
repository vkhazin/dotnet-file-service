# DotNet File Retrieval Service

## Overview

* Create dotnet core micro-serivce with a single route GET /api/file/{file-name}
* The service will not run on Azure, it will be deployed elsewhere
* The file storage will use [Azure Files](https://docs.microsoft.com/en-us/azure/storage/files/?toc=%2fazure%2fstorage%2ffiles%2ftoc.json) with SMB protocol
* The code will check whether file is already present in a Azure Storage #1
* If the file present in the Azure Storage #1 serve the file suitable to be referenced from html image tage, src attribute, i.e. ***not json***
* If the file is not present in Azure Storage #1 download the file from Azure Storage #2, copy the file to Azure Storage #1 and serve the file in the same fashion
* Provide scripts to create Azure Storage and credentials to connect to the service from the end-point
* Include unit tests to test the scenarios: file present, file not present, file non-existent, error getting the file, file name parameter is missing
* Include markdown documentation
* Use proper http status code: 200 - for success, 404 - when file is not found in either of the storages, 400 - when file parameter is missing, 500 - when there is an error
* Enable logging at info, warning, and error levels for the above status codes: 200 - info, 400 & 404 - warning, 500 - error 

## Azure Setup

Repeat the following steps for both Storage 1 and Storage 2:

### Create resource group and storage account

Replace `<resource_group>`, `<storage_name>`, `<location>` with your values: 

    az group create --name <resource_group> --location <location>
    az storage account create --name <storage_name> --location <location> --resource-group <resource_group> --sku Standard_LRS
    
### Construct connection string:

List storage account keys:

    az storage account keys list --account-name <storage_name> --resource-group <resource_group> --output table

Constuct connection string using first key from the list and storage account name:

    DefaultEndpointsProtocol=https;AccountName=<storage_name>;AccountKey=<storage_key>;EndpointSuffix=core.windows.net

### Create file share

Using storage key from the previous step:

    az storage share create --name <share_name> --account-name <storage_name> --account-key <storage_key>
    
## Upload sample file to file share

    az storage file upload --share-name <share_name> --account-name <storage_name> --account-key <storage_key> --source "readme.txt"
    
## Run and test

1. Copy appsettings.Development.json to the FileService folder.
2. Run API:

    `cd FileService`
    
    `dotnet run`
    
3. Request test files: http://localhost:5000/api/file/readme.txt and http://localhost:5000/api/file/image.jpg
4. Run tests:

    `cd FileService.Tests`
    
    `dotnet test`
