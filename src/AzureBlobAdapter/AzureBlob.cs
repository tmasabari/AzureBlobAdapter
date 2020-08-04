using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs.Specialized;

using Stream = System.IO.Stream;
using StreamWriter = System.IO.StreamWriter;

using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel;
using System.IO;

namespace Azure.BlobAdapter
{
    public class AzureBlob
    {
        #region "Helper methods"

        protected AzureBlobAdapter _azureBlobAdapter;

        protected Tuple<string, string> ExtractContainerBlobPortions(string blobName)
        {
            //https://docs.microsoft.com/en-us/dotnet/api/system.io.path.getpathroot?view=netstandard-2.0
            // Some times 
            //  "X:" (Windows: path specified an absolute path on a given drive).
            //  "\\ComputerName\SharedFolder"(Windows: a UNC path).
            var containerPortion = _azureBlobAdapter.Path.GetPathRoot(blobName);
            if (string.IsNullOrEmpty(containerPortion))
                throw new InvalidDataException("Invalid blob name, mount point not found");

            //_azureBlobAdapter.Path.VolumeSeparatorChar 58 ':'
            //_azureBlobAdapter.Path.DirectorySeparatorChar 92 '\\'
            //_azureBlobAdapter.Path.AltDirectorySeparatorChar 47 '/'

            var blobPathStart = blobName.IndexOf(containerPortion) + containerPortion.Length;
            // GetPathRoot returns with or without  DirectorySeparatorChar for different scenarios. 
            // So always remove DirectorySeparatorChar from container portion and blob portion.
            if (containerPortion.EndsWith(_azureBlobAdapter.Path.DirectorySeparatorChar.ToString()))
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

        protected string ExtractContainerName(string mountName)
        {
            //throws System.InvalidOperationException: No element satisfies the condition in predicate. 
            //-or- The source sequence is empty.
            var driveInfo = (AzureDriveInfo)_azureBlobAdapter.DriveInfo.FromDriveName(mountName);

            return driveInfo.ContainerName;
        }

        protected BlobContainerClient GetBlobContainerClient(string containerName)
        {
            //public BlobContainerClient BlobContainerClientObject { get; private set; }
            var BlobContainerClientObject = _azureBlobAdapter.BlobServiceClientObject.GetBlobContainerClient(containerName);
            return BlobContainerClientObject;
        }
        protected BlobContainerProperties GetBlobContainerClient(BlobContainerClient containerClient)
        {
            //public BlobContainerProperties BlobContainerPropertiesObject { get; private set; }
            var blobContainerPropertiesResponse = containerClient.GetProperties();
            var BlobContainerPropertiesObject = blobContainerPropertiesResponse.Value;
            return BlobContainerPropertiesObject;
        }

        public virtual BlobClient GetBlobClient(string blobName)
        {
            var Names = ExtractContainerBlobPortions(blobName);
            var containerName = ExtractContainerName(Names.Item1);

            return GetBlobContainerClient(containerName).GetBlobClient(Names.Item2);
        }
        public virtual AppendBlobClient GetAppendBlobClient(string blobName)
        {
            var Names = ExtractContainerBlobPortions(blobName);
            var containerName = ExtractContainerName(Names.Item1);

            return GetBlobContainerClient(containerName).GetAppendBlobClient(Names.Item2);
        }

        public virtual void CreateReplaceBlobFromStream(string containerName, string blobName, Stream stream)
        {
            var containerClient = GetBlobContainerClient(containerName);
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Debug.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            //Reset the Position of the MemoryStream after you copy to it, or else the BlobClient will try to read from the end.
            //so it will hang.
            stream.Position = 0;

            // Open the file and upload its data
            blobClient.Upload(stream, true);
        }
        public virtual void CreateReplaceBlobFromStream(string blobName, Stream data)
        {
            var Names = ExtractContainerBlobPortions(blobName);
            var containerName = ExtractContainerName(Names.Item1);
            CreateReplaceBlobFromStream(containerName, Names.Item2, data);
        }
        public virtual void CreateReplaceBlobFromLocalFile(BlobContainerClient containerClient, string blobName, string filePath)
        {
            // Get a reference to a blob
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            Debug.WriteLine("Uploading to Blob storage as blob:\n\t {0}\n", blobClient.Uri);

            // Open the file and upload its data
            blobClient.Upload(filePath, true);
        }

        public virtual Response DownloadFromBlobToStream(BlobContainerClient containerClient, string blobName, Stream stream)
        {
            Debug.WriteLine("Downloading from Blob storage as blob:\n\t {0}\n", blobName);
            var blobClient = containerClient.GetBlobClient(blobName);
            // Download the blob's contents and save it to a file
            Response download = blobClient.DownloadTo(stream);
            return download;
        }
        public virtual Response DownloadFromBlobToStream(string blobName, Stream stream)
        {
            var Names = ExtractContainerBlobPortions(blobName);
            var containerName = ExtractContainerName(Names.Item1);

            return DownloadFromBlobToStream(GetBlobContainerClient(containerName), Names.Item2, stream);
        }

        public virtual Response DownloadFromBlobToLocalFile(BlobContainerClient containerClient, string blobName, string localFilePah)
        {
            Debug.WriteLine("Downloading from Blob storage as blob:\n\t {0}\n", blobName);
            var blobClient = containerClient.GetBlobClient(blobName);
            // Download the blob's contents and save it to a file
            Response download = blobClient.DownloadTo(localFilePah);
            return download;
        }

        protected virtual void InternalWriteAllText(Stream stream, String contents, Encoding encoding, bool leaveStreamOpen)
        {
            //Contract.Requires(path != null);
            //Contract.Requires(encoding != null);
            //Contract.Requires(path.Length > 0);

            using (StreamWriter sw = stream.GetStreamWriter( encoding, leaveStreamOpen))
                sw.Write(contents);
        }
        #endregion

        #region "IFile implementation"
        public virtual DateTime GetLastModified(string blobName)
        {
            Debug.WriteLine("Get Last Modified Date Time of blob:\n\t {0}\n", blobName);
            BlobClient blobClient = GetBlobClient(blobName);

            BlobProperties blobProperties = blobClient.GetProperties().Value;
            return blobProperties.LastModified.DateTime;

        }


        public virtual bool Exists(string blobName)
        {
            BlobClient blobClient = GetBlobClient(blobName);
            return blobClient.Exists().Value;
        }

        public void Delete(string blobName)
        {
            Debug.WriteLine("Deleting blob:\n\t {0}\n", blobName);
            BlobClient blobClient = GetBlobClient(blobName);
            if (blobClient.Exists().Value)
            {
                blobClient.Delete();
            }
            else
            {
                throw new InvalidOperationException("Blob not found");
            }
        }

        public virtual void AppendAllText(string blobName, string contents, Encoding encoding)
        {
            //    //Status: 409 (Specified feature is not yet supported for hierarchical namespace accounts.)
            //    //ErrorCode: FeatureNotYetSupportedForHierarchicalNamespaceAccounts
            //Debug.WriteLine("Called to append all text to blob:\n\t {0}\n", blobName);
            //AppendBlobClient appendBlobClient = GetAppendBlobClient(blobName);
            //if (!appendBlobClient.Exists().Value)
            //    throw new InvalidOperationException("Blob not found");
            //using (var stream = new System.IO.MemoryStream())
            //{
            //    InternalWriteAllText(stream, contents, encoding, true);
            //    stream.Position = 0;
            //    appendBlobClient.AppendBlock(stream);
            //}
        }

        public virtual void CreateDirectory(string directoryName)
        {
            //BlobClient d = _azureBlobAdapter.BlobContainerClientObject.di (blobName);
        }

        #endregion

        #region "Blob Specific Implementation"

        public virtual bool CancelCopy(string destBlobName,string copyId)
        {
            // Fetch the destination blob's properties before checking the copy state.
            BlobClient destinationBlobClient = GetBlobClient(destBlobName);
            BlobProperties destinationBlobProperties = destinationBlobClient.GetProperties().Value;

            // Check the copy status. If it is still pending, abort the copy operation.
            if (destinationBlobProperties.CopyStatus == CopyStatus.Pending)
            {
                destinationBlobClient.AbortCopyFromUri(copyId);
                Debug.WriteLine("Copy operation {0} has been aborted.", copyId);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Based upon https://stackoverflow.com/questions/3734672/azure-storage-blob-rename
        /// </summary>
        /// <param name="sourceBlobName"></param>
        /// <param name="destinationBlobName"></param>
        /// <returns></returns>
        public virtual async Task<bool> CopyBlockBlobAsync(string sourceBlobName, string destinationBlobName, 
            bool overwrite =false, bool deleteSource = false)
        {
            BlobClient sourceBlobClient = null;
            BlobClient destinationBlobClient = null;

            try
            {
                // Get a blob from the container to use as the source.
                sourceBlobClient = GetBlobClient(sourceBlobName);
                // Ensure that the source blob exists.
                if (!await sourceBlobClient.ExistsAsync())
                {
                    throw new InvalidOperationException("Source blob name " + sourceBlobName + " not found" );
                }

                // Get a reference to a destination blob (in this case, a new blob).
                destinationBlobClient = GetBlobClient(destinationBlobName);
                // If overwrite = false then only check destination and ensure that the destination blob does not exists.
                if (!overwrite && await destinationBlobClient.ExistsAsync())
                {
                    throw new InvalidOperationException("Destination blob name " + sourceBlobName + " already found and overwrite was disabled");
                }

                //var sourceLeaseClient = sourceBlobClient.GetBlobLeaseClient();
                //// Lease the source blob for the copy operation to prevent another client from modifying it.
                //// Specifying null for the lease interval creates an infinite lease.
                //var sourceBlobLease = sourceLeaseClient.Acquire(new TimeSpan(-1)).Value; // await sourceBlob.AcquireLeaseAsync(null);
                //var leaseId = sourceBlobLease.LeaseId;


                // Get the ID of the copy operation.
                destinationBlobClient.StartCopyFromUri( sourceBlobClient.Uri );  //var copyOperation = 

                // Fetch the destination blob's properties before checking the copy state.
                BlobProperties destinationBlobProperties = destinationBlobClient.GetProperties().Value;
                while (destinationBlobProperties.CopyStatus == CopyStatus.Pending)
                    await Task.Delay(100);

                Debug.WriteLine("Status of copy operation: {0}", destinationBlobProperties.CopyStatus);
                if (destinationBlobProperties.CopyStatus != CopyStatus.Success)
                    throw new Exception("Copy failed: " + destinationBlobProperties.CopyStatus);

                Debug.WriteLine("Completion time: {0}", destinationBlobProperties.CopyCompletedOn.DateTime );
                //Debug.WriteLine("Bytes copied: {0}", copyOperation.Value ); // throws exception copyOperation.Status is not giving correct status
                //"Total bytes: {0}" not available

                if (deleteSource)
                {
                    await sourceBlobClient.DeleteAsync();
                }
                return true;
            }
            finally
            {
                //nothing (lease) to close or dispose 
                //// Break the lease on the source blob, if not terminated
                //if (sourceBlobClient != null)
                //{
                //    // Fetch the destination blob's properties before checking the copy state.
                //    BlobProperties blobProperties = sourceBlobClient.GetProperties().Value;

                //    if (blobProperties.LeaseState != LeaseState.Available)
                //    {
                //        var sourceLeaseClient = sourceBlobClient.GetBlobLeaseClient();
                //        sourceLeaseClient.Break();
                //    }
                //}
            }
        }

        public void Move(string sourceFileName, string destFileName)
        {
            RenameAsync(sourceFileName, destFileName).Wait();
        }

        public virtual async Task<bool> RenameAsync(string sourceBlobName, string destinationBlobName)
        {
            return await CopyBlockBlobAsync( sourceBlobName, destinationBlobName, overwrite: false, deleteSource: true);
        }

        #endregion

    }
}
