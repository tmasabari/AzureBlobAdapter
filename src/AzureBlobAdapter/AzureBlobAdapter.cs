using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Azure;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Azure.BlobAdapter
{
    public class AzureBlobAdapter : IFileSystem
    {
        public string DirectorySeparator { get; } = "\\";
        public string BlobPathSeparator { get; } = "/";
        internal IAzureBlobSettings AzureBlobSettingsData { get; private set; }

        public BlobServiceClient BlobServiceClientObject { get; private set; }
        public BlobServiceProperties BlobServicePropertiesObject { get; private set; }

        //Create DataLakeServiceClient using StorageSharedKeyCredentials
        public DataLakeServiceClient DataLakeServiceClientObject { get; private set; }

        /// <summary>
        /// Use a connection string to connect to a Storage account.
        ///
        /// A connection string includes the authentication information
        /// required for your application to access data in an Azure Storage
        /// account at runtime using Shared Key authorization.
        /// </summary>
        public AzureBlobAdapter(IAzureBlobSettings azureBlobSettings)
        {

            AzureBlobSettingsData = azureBlobSettings;

            //authentication methods
            //https://github.com/Azure/azure-sdk-for-net/blob/master/sdk/storage/Azure.Storage.Blobs/samples/Sample02_Auth.cs
            // Create a BlobServiceClient object which will be used to create a container client
            BlobServiceClientObject = new BlobServiceClient(azureBlobSettings.ConnectionString);

            // Make a service request to verify we've successfully authenticated
            var servicePropertiesResponse = BlobServiceClientObject.GetProperties();
            BlobServicePropertiesObject = servicePropertiesResponse.Value;

            StorageSharedKeyCredential sharedKeyCredential = new StorageSharedKeyCredential(
                azureBlobSettings.StorageAccountName, azureBlobSettings.Key);
            string dfsUri = "https://" + azureBlobSettings.StorageAccountName + ".dfs.core.windows.net";
            DataLakeServiceClientObject = new DataLakeServiceClient(new Uri(dfsUri), sharedKeyCredential);

            //this will simply list all the containers and the last modified date time
            //foreach (var fileSystem in DataLakeServiceClientObject.GetFileSystems())
            //{
            //    Debug.WriteLine(fileSystem.Name + " - LastModified:" + fileSystem.Properties.LastModified);
            //}
            
        }

        public IFile File => new AzureFile(this); 
        public IDriveInfoFactory DriveInfo => new AzureDriveInfoFactory(this);

        public IPath Path => new PathWrapper(this);

        public IDirectory Directory => new AzureDirectory(this);  //throw new System.NotImplementedException();  //=> 

        #region Helper methods

        public Tuple<string, string> ExtractContainerBlobPortions(string blobName)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getpathroot?view=netstandard-2.0
            // Some times 
            //  "X:" (Windows: path specified an absolute path on a given drive).
            //  "\\ComputerName\SharedFolder"(Windows: a UNC path).
            var containerPortion = Path.GetPathRoot(blobName);
            if (string.IsNullOrEmpty(containerPortion))
                throw new System.IO.InvalidDataException("Invalid blob name, mount point not found");

            //_azureBlobAdapter.Path.VolumeSeparatorChar 58 ':'
            //_azureBlobAdapter.Path.DirectorySeparatorChar 92 '\\'
            //_azureBlobAdapter.Path.AltDirectorySeparatorChar 47 '/'

            var blobPathStart = blobName.IndexOf(containerPortion) + containerPortion.Length;
            // GetPathRoot returns with or without  DirectorySeparatorChar for different scenarios. 
            // So always remove DirectorySeparatorChar from container portion and blob portion.
            if (containerPortion.EndsWith(DirectorySeparator))
            {
                //remvoe the character and blobPathStart already correct
                containerPortion = containerPortion.Substring(0, containerPortion.Length - 1);
            }
            else
            {
                //container portion is correct and exclude the VolumeSeparatorChar from blob portion
                blobPathStart++;
            }
            var blobPath = blobName.Substring(blobPathStart);

            return new Tuple<string, string>(containerPortion, blobPath);
            //string[] driveSeparator = { @":\" };
            //var driveNameParts = blobName.Split(driveSeparator, StringSplitOptions.RemoveEmptyEntries);
            //if (driveNameParts.Length < 2)
            //    throw new InvalidDataException("Invalid blob name, mount point not found");

            //return driveNameParts[0];
        }

        public string ExtractContainerName(string mountName)
        {
            //throws System.InvalidOperationException: No element satisfies the condition in predicate. 
            //-or- The source sequence is empty.
            var driveInfo = (AzureDriveInfo)DriveInfo.FromDriveName(mountName);

            return driveInfo.ContainerName;
        }
        #endregion

        #region Not Implemented

        [ExcludeFromCodeCoverage]
        public IFileInfoFactory FileInfo => throw new System.NotImplementedException();

        [ExcludeFromCodeCoverage]
        public IFileStreamFactory FileStream => throw new System.NotImplementedException();

        [ExcludeFromCodeCoverage]
        public IDirectoryInfoFactory DirectoryInfo => throw new System.NotImplementedException();

        [ExcludeFromCodeCoverage]
        public IFileSystemWatcherFactory FileSystemWatcher => throw new System.NotImplementedException();

        #endregion
    }
}
