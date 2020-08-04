using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Azure.BlobAdapter
{
    public class AzureDriveInfoFactory : IDriveInfoFactory
    {
        protected AzureBlobAdapter _azureBlobAdapter;
        protected List<IDriveInfo> _azureDrives = new List<IDriveInfo>();

        //public IFileSystem FileSystem
        //{
        //    get
        //    {
        //        return _azureBlobAdapter;
        //    }
        //}

        public AzureDriveInfoFactory(AzureBlobAdapter azureBlobAdapter)
        {
            _azureBlobAdapter = azureBlobAdapter;

            foreach (var driveSetting in azureBlobAdapter.AzureBlobSettingsData.MountPoints)
            {
                //make the mount points upper for case in sensitive comparison
                _azureDrives.Add(new AzureDriveInfo(azureBlobAdapter) { ContainerName = driveSetting.ContainerName, Name = driveSetting.Name.ToUpper() });
            }
        }

        #region Implemented methods
        public IDriveInfo FromDriveName(string driveName)
        {
            //make the mount points upper for case in sensitive comparison
            return _azureDrives.First(driveInfo => driveInfo.Name == driveName.ToUpper());
        }

        public IDriveInfo[] GetDrives()
        {
            return _azureDrives.ToArray();
        }
        #endregion

    }
}
