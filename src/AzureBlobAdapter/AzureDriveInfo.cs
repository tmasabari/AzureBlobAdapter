using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Text;

namespace Azure.BlobAdapter
{
    public class AzureDriveInfo : IDriveInfo
    {
        protected AzureBlobAdapter _azureBlobAdapter;
        public IFileSystem FileSystem
        {
            get
            {
                return _azureBlobAdapter;
            }
        }

        public AzureDriveInfo(AzureBlobAdapter azureBlobAdapter)
        {
            _azureBlobAdapter = azureBlobAdapter;
        }


        public virtual string ContainerName { get; set; }


        #region Implemented
        public virtual string Name { get; set; }

        public bool IsReady => true;

        #endregion


        [ExcludeFromCodeCoverage]
        public DriveType DriveType => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public long AvailableFreeSpace => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public string DriveFormat => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public IDirectoryInfo RootDirectory => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public long TotalFreeSpace => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public long TotalSize => throw new NotImplementedException();

        [ExcludeFromCodeCoverage]
        public string VolumeLabel { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
