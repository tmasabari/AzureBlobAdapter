using System;
using System.IO.Abstractions;
using Azure.BlobAdapter;

namespace FileSystemProxy
{
    public enum FileSystemTypes { SystemIO, AzureBlob }

    public class FileSystemProxy
    {
        public readonly IFileSystem FileSystem;

        public FileSystemProxy(FileSystemTypes fileSystemType, AzureBlobSettings azureBlobSettings = null)
        {
            if (fileSystemType == FileSystemTypes.SystemIO)
            {
                FileSystem = new FileSystem();
                return;
            }
            if(azureBlobSettings == null)
            {
                throw new InvalidOperationException("Azure Blob Settings is required to initialize the Azure");
            }
            FileSystem = new AzureBlobAdapter(azureBlobSettings);
        }
    }
}
