using System;
using System.IO.Abstractions;

namespace Azure.BlobAdapter
{
    public enum FileSystemTypes { SystemIO, AzureBlob }

    public static class FileSystemProxy
    {
        public static IFileSystem GetFileSystem(FileSystemTypes fileSystemType, AzureBlobSettings azureBlobSettings = null)
        {
            IFileSystem FileSystem;
            if (fileSystemType == FileSystemTypes.SystemIO)
            {
                FileSystem = new FileSystem();
                return FileSystem;
            }
            if(azureBlobSettings == null)
            {
                throw new InvalidOperationException("Azure Blob Settings is required to initialize the Azure");
            }
            FileSystem = new AzureBlobAdapter(azureBlobSettings);
            return FileSystem;
        }
    }
}
