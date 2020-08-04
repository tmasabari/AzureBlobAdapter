using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;

namespace Azure.BlobAdapter
{
    public class AzureBlobAdapter : IFileSystem
    {
        internal IAzureBlobSettings AzureBlobSettingsData { get; private set; }

        public BlobServiceClient BlobServiceClientObject { get; private set; }
        public BlobServiceProperties BlobServicePropertiesObject { get; private set; }

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
        }

        public IFile File => new AzureFile(this); 
        public IDriveInfoFactory DriveInfo => new AzureDriveInfoFactory(this);

        public IPath Path => new PathWrapper(this);


        #region Not Implemented
        [ExcludeFromCodeCoverage]
        public IDirectory Directory => throw new System.NotImplementedException();  //=> new AzureDirectory(this);

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
