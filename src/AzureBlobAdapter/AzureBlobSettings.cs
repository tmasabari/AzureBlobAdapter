﻿namespace Azure.BlobAdapter
{
    public class AzureMountPointsSetting
    {
        public virtual string Name { get; set; }
        public virtual string ContainerName { get; set; }
    }

    public interface IAzureBlobSettings
    {
        string StorageAccountName { get; set; }
        /// <summary>
        /// StorageAccountUrl = "https://" + StorageAccountName + ".dfs.core.windows.net";
        /// </summary>
        string StorageAccountUrl { get; }
        string Key { get; set; }
        string ConnectionString { get; set; }
        //string ContainerName { get; set; }

        AzureMountPointsSetting[] MountPoints { get; set; }
    }

    public class AzureBlobSettings : IAzureBlobSettings
    {
        public string StorageAccountUrl => "https://" + StorageAccountName + ".dfs.core.windows.net";
        public string StorageAccountName { get; set; }
        public string Key { get; set; }
        public string ConnectionString { get; set; }
        //public string ContainerName { get; set; }

        public AzureMountPointsSetting[] MountPoints { get; set; }
    }
}
