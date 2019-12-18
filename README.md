# DotNet File Retrieval Service

## Overview

* Create dotnet core micro-serivce with a single route /api/file/{file-name}
* The service will not run on Azure
* The code will check whether file is already present in a Azure Blob Storage #1
* If the file present in the Azure Blob Storage #1 serve the file suitable to be referenced from html image tage, src attribute, i.e. ***not json***
* If the file is not present in Azure Blob Storage #1 download the file from Azure Blob Storage #2, copy the file to Azure Blob Storage #1 and serve the file in the same fashion
* Provide scripts to create Azure Blob Storage and credentials to connect to the service from the end-point
* Include unit tests to test the scenarios: file present, file not present, file non-existent, error getting the file, file name parameter is missing
* Include markdown documentation
* Use proper http status code: 200 - for success, 404 - when file is not found in either of the storages, 400 - when file parameter is missing, 500 - when there is an error
* Enable logging at info, warning, and error levels for the above status codes: 200 - info, 400 & 404 - warning, 500 - error 
