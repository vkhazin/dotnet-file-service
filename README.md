# DotNet File Retrieval Service

## Overview

* Create dotnet core micro-serivce with a single route /api/file/{file-name}
* The service will not run on Azure
* Check whether file already present in a Azure Blob Service #1
* If the file present in the Azure Blob Service #1 serve the file suitable to be referenced from html image tage, src attribute
* Not Json
* If the file is not present download file from Azure Blob Service #2, copy the file to Azure Blob Service #1 and serve the file in the same fashion
* Provide scripts to create Azure blob service and credentials to connect to the service from the end-point
* Include unit tests
* Include markdown documentation
