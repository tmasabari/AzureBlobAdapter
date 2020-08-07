using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Azure.BlobAdapter
{
    public class AzureDriveInfoFactory : IDriveInfoFactory
    {
        protected AzureBlobAdapter _azureBlobAdapter;
        protected List<IDriveInfo> _azureDrives = new List<IDriveInfo>();

        public AzureDriveInfoFactory(AzureBlobAdapter azureBlobAdapter)
        {
            _azureBlobAdapter = azureBlobAdapter;

            foreach (var driveSetting in azureBlobAdapter.AzureBlobSettingsData.MountPoints)
            {
                //make the mount points upper for case in sensitive comparison
                _azureDrives.Add(new AzureDriveInfo(azureBlobAdapter) 
                    { ContainerName = driveSetting.ContainerName, Name = driveSetting.Name });
            }
        }

        #region Implemented methods
        public IDriveInfo FromDriveName(string driveName)
        {
            //make the mount points upper for case in sensitive comparison
            //mount points can have both drives and mount points. There is no use case to get drives only
            //driveInfo.Name.Contains(":") && 
            return _azureDrives.First(driveInfo => 
                driveInfo.Name.Equals(driveName, StringComparison.OrdinalIgnoreCase) 
                );
        }

        public IDriveInfo[] GetDrives()
        {
            return _azureDrives.ToArray();
        }

        public IDriveInfo[] GetDrivesOnly()
        {
            return _azureDrives.Where(
                driveInfo => {
                    var match = Regex.Match(driveInfo.Name, "^([A-Z|a-z][:])");
                    return match.Success;
                    }
                ).ToArray();
        }
        #endregion

    }
}
