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
            //mount points can have both drives and mount points. There is no use case to get drives only
            //driveInfo.Name.Contains(":") && 
            return _azureDrives.First(driveInfo =>
                driveInfo.Name == driveName.ToUpper()
                );
        }

        public IDriveInfo[] GetDrives()
        {
            return _azureDrives.ToArray();
        }

        public IDriveInfo[] GetDrivesOnly()
        {
            return _azureDrives.Where(
                driveInfo => driveInfo.Name.Contains(":")
                ).ToArray();
        }
        #endregion

    }
}
