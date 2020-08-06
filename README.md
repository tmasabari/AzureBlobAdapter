# AzureBlobAdapter

This is an adapter for the Azure Blob Storage to integrate with the existing .NET System.IO based file system access code with little or no changes.


## Design Guidelines
1.  This library implements the interfaces of the [System.IO.Abstractions](https://www.nuget.org/packages/System.IO.Abstractions/) package. The System.IO.Abstractions NuGet package provides a layer of abstraction over the file system that is API-compatible with the existing code. They help to make file access code more unit testable as well.

1. This library uses the latest version of [Azure Blob SDK - version 12.x](https://docs.microsoft.com/en-us/dotnet/api/overview/azure/storage?view=azure-dotnet)
1. Directories are supported via the latest version of [Azure Data Lake Storage](https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-introduction). It
Combines the power of a Hadoop compatible file system with integrated hierarchical namespace with the massive scale and economy of Azure Blob Storage to help speed your transition from proof of concept to production. Refer [documentation](https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-introduction), [SDK](https://docs.microsoft.com/en-us/dotnet/api/azure.storage.files.datalake?view=azure-dotnet), [Samples](https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-directory-file-acl-dotnet), [source code](https://github.com/Azure/azure-sdk-for-net/tree/master/sdk/storage/Azure.Storage.Files.DataLake).

1. The class library is implemented in .NET Standard 2.0. So that it will support both
   * The most widely used and the legacy .NET Framework and
   * the latest .NET core applications
1. Drives and shared folders are implemented via the containers. The drive letters and shared folders can be mounted via mount points via settings.


## Differences between System.IO and AzureBlobAdapter
1. The Azure blob needs to be initialized with the connection strings and to be injected into the application via these interfaces. But the file system can be used directly
2. Many functionalities of the file systems are not available with the Blob. Example: FileSystemSecurity (audits, access rules, etc).
3. The advanced features in the Blob storage don't exist in file system calls. So they are not accessible via these interfaces.

## Implemented functionality summary
1.  Most of the time in the .NET applications, we create/update/deleting/listing files and directories. (CRUD)
2. File and Directory Information - Work in progress

### Implemented Interfaces and Methods
| Interface | Remarks |
| ------ | ------ |
|IFileSystem|IFile, IDriveInfoFactory, IPath|
|IFile|Implemented create, append, read, delete, copy, move, rename operations for both text and binary files.  Except methods for continuous Stream based create/update operations, individual file level encryption/decryption, read/change AccessControl and Attributes|
|IDriveInfoFactory|100%|
|IDriveInfo|Name, IsReady|
|IDirectory|Work In progress|


### Todos

 - Write MORE Tests

License
----

MIT