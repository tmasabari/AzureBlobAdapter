using System;
using System.Collections.Generic;
using System.Configuration;
using Azure.BlobAdapter;

namespace AzureBlobNETFramework
{
    public class AzureBlobConfiguration : ConfigurationSection
    {
        [ConfigurationProperty("MountPoints")]
        public MountPointCollection MountPoints
        {
            get { return (MountPointCollection)this["MountPoints"]; }
        }

        [ConfigurationProperty("StorageAccountName", IsRequired = true)]
        public string StorageAccountName
        {
            get { return (string)this["StorageAccountName"]; }
        }

        [ConfigurationProperty("Key", IsRequired = true)]
        public string Key
        {
            get { return (string)this["Key"]; }
        }

        [ConfigurationProperty("ConnectionString", IsRequired = true)]
        public string ConnectionString
        {
            get { return (string)this["ConnectionString"]; }
        }


        public static AzureBlobSettings GetAzureBlobSettings()
        {
            var section = ConfigurationManager.GetSection("AzureBlobSettings");
            if (section == null)
            {
                throw new InvalidOperationException("AzureBlobSettings section not found in application configuration");
            }
            var azureBlobConfiguration = (section as AzureBlobConfiguration);

            var azureBlobSettings = new AzureBlobSettings();
            azureBlobSettings.Key = azureBlobConfiguration.Key;
            azureBlobSettings.ConnectionString = azureBlobConfiguration.ConnectionString;
            azureBlobSettings.StorageAccountName = azureBlobConfiguration.StorageAccountName;
            List<AzureMountPointsSetting> azureMountPoints = new List<AzureMountPointsSetting>();
            foreach (MountPointElement item in azureBlobConfiguration.MountPoints)
            {
                azureMountPoints.Add(new AzureMountPointsSetting()
                {
                    Name = item.Name, ContainerName=item.ContainerName
                });
            }
            azureBlobSettings.MountPoints = azureMountPoints.ToArray(); 
            return azureBlobSettings;
        }
    }

    public class MountPointCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new MountPointElement();
        }
        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((MountPointElement)element).Name;
        }
    }

    public class MountPointElement : ConfigurationElement
    {
        [ConfigurationProperty("Name", IsKey = true, IsRequired = true)]
        public string Name
        {
            get { return (string)this["Name"]; }
        }

        [ConfigurationProperty("ContainerName", IsKey = false, IsRequired = true)]
        public string ContainerName
        {
            get { return (string)this["ContainerName"]; }
        }
    }
}
