using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;
using Azure.Storage.Files.DataLake;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake.Models;
using System.Runtime.InteropServices;
using System.Diagnostics.CodeAnalysis;

namespace Azure.BlobAdapter
{
    /// <summary>
    /// refer https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-directory-file-acl-dotnet
    /// </summary>
    public partial class AzureDirectory : IDirectory
    {

        [ExcludeFromCodeCoverage]
        public IDirectoryInfo GetParent(string path)
        {
            throw new NotImplementedException();
            //String s = _azureBlobAdapter.Path.GetDirectoryName(path);
            //if (s == null)
            //    return null;
            //return new DirectoryInfo(s);
        }
        [ExcludeFromCodeCoverage]
        public string GetCurrentDirectory()
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetCurrentDirectory(string path)
        {
            throw new NotImplementedException();
        }

        #region DirectorySecurity methods
        [ExcludeFromCodeCoverage]
        public IDirectoryInfo CreateDirectory(string path, DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DirectorySecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DirectorySecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetAccessControl(string path, DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Date Time Operations 
        [ExcludeFromCodeCoverage]
        public DateTime GetCreationTime(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DateTime GetCreationTimeUtc(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DateTime GetLastAccessTime(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DateTime GetLastAccessTimeUtc(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DateTime GetLastWriteTime(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public DateTime GetLastWriteTimeUtc(string path)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetCreationTime(string path, DateTime creationTime)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            throw new NotImplementedException();
        }
        [ExcludeFromCodeCoverage]
        public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
