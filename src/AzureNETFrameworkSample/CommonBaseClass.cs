using Azure.BlobAdapter;
using AzureBlobNETFramework;
using System.IO.Abstractions;
using System.Configuration;

namespace AzureNETFrameworkSample
{
    public class CommonBaseClass
    {
        private IFileSystem fileSystem ;
        public IFile File;
        public IDirectory Directory;
        public IPath Path;
        public IDriveInfoFactory DriveInfo;

        public CommonBaseClass()
        {
            fileSystem = FileSystemProxy.GetFileSystem(
                        ConfigurationManager.AppSettings["FileSystemType"] == "AzureBlob" ? FileSystemTypes.AzureBlob : FileSystemTypes.SystemIO,
                        AzureBlobConfiguration.GetAzureBlobSettings());

            File = fileSystem.File;
            Directory = fileSystem.Directory;
            Path = fileSystem.Path;
            DriveInfo = fileSystem.DriveInfo;
        }

        //azureBlobSettings.StorageAccountName = "avenublobpoc";
        //azureBlobSettings.Key = "e6oAhYVGH45nYLVacLCPLQq1vGosiy+fiO/VX+PASSI2bd6atqSuWw6rcLF+JAQ8MrNyT+qudXKJ7uKCWNNNrQ==";
        //azureBlobSettings.ConnectionString = "DefaultEndpointsProtocol=https;AccountName=avenublobpoc;AccountKey=e6oAhYVGH45nYLVacLCPLQq1vGosiy+fiO/VX+PASSI2bd6atqSuWw6rcLF+JAQ8MrNyT+qudXKJ7uKCWNNNrQ==;EndpointSuffix=core.windows.net";

        //List<AzureMountPointsSetting> azureMountPoints = new List<AzureMountPointsSetting>();
        //azureMountPoints.Add(new AzureMountPointsSetting()
        //{ ContainerName = "poccontainer1", Name = "\\\\hostname\\shared" });
        //azureMountPoints.Add(new AzureMountPointsSetting()
        //{ ContainerName = "d-drive", Name = "D:" });
        //azureBlobSettings.MountPoints = azureMountPoints.ToArray();
    }
}
