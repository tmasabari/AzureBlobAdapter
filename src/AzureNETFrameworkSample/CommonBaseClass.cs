﻿using Azure.BlobAdapter;
using AzureBlobNETFramework;
using System.Configuration;
using System.IO.Abstractions;

namespace AzureNETFramework.IO
{
    public class CommonIOBaseClass
    {
        private readonly IFileSystem fileSystem;
        public IFile File;
        public IDirectory Directory;
        public IPath Path;
        public IDriveInfoFactory DriveInfo;

        public CommonIOBaseClass()
        {
            fileSystem = FileSystemProxy.GetFileSystem(
                        ConfigurationManager.AppSettings["FileSystemType"] == "AzureBlob" ? FileSystemTypes.AzureBlob : FileSystemTypes.SystemIO,
                        AzureBlobConfiguration.GetAzureBlobSettings());

            File = fileSystem.File;
            Directory = fileSystem.Directory;
            Path = fileSystem.Path;
            DriveInfo = fileSystem.DriveInfo;
        }
    }
}
