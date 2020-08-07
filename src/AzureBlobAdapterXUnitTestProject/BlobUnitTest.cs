using System;
using Xunit;
using Xunit.Extensions.Ordering;

using Azure.BlobAdapter;
using Microsoft.Extensions.Configuration;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace AzureBlobAdapterXUnitTestProject
{
    public class BlobUnitTest
    {
        AzureBlobSettings azureBlobSettings = new AzureBlobSettings();
        const string blobFileName = @"\\hostname\shared\sampledirectory\temp.json";
        const string localPermFileName = "TestData/temp.json";

        public BlobUnitTest()
        {
            //Install-Package Microsoft.Extensions.Configuration
            //Install-Package Microsoft.Extensions.Configuration.Json
            //Install-Package Microsoft.Extensions.Configuration.Binder 

            IConfiguration Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              //.AddEnvironmentVariables() //.AddCommandLine(args)
              .Build();
            Configuration.Bind("AzureBlobSettings", azureBlobSettings); 
        }

        [Fact]
        public void BlobAdapterTest()
        {
            AzureBlobAdapter azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
            Assert.True( ! string.IsNullOrEmpty( azureBlobAdapter.BlobServicePropertiesObject.Logging.Version) );
        }

        //// List all blobs in the container
        //await foreach (BlobItem blobItem in containerClient.GetBlobsAsync())
        //{
        //    Debug.WriteLine("\t" + blobItem.Name);
        //}

        [Fact, Order(1)]
        public void BlobTextFileCreateReadDeleteTest()
        {
            var blobFilename = @"d:\temptest\randomfile.tmp";
            var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
            var azureFile = azureBlobAdapter.File;

            var contentsWrite = System.IO.File.ReadAllText(localPermFileName);
            azureFile.WriteAllText(blobFilename, contentsWrite);

            string[] lines = { "line1", "line2" };
            azureFile.AppendAllLines(blobFilename, lines);

            var modifiedData = contentsWrite;
            foreach (var item in lines)
                modifiedData += item + Environment.NewLine;

            var readDataSB = new StringBuilder();
            string[] readLines = azureFile.ReadAllLines(blobFilename);
            foreach (var line in readLines)
            {
                readDataSB.Append(line).Append(Environment.NewLine);
            }
            Assert.Equal(modifiedData, readDataSB.ToString());

            azureFile.Delete(blobFilename);

            var ExistsResult = azureFile.Exists(blobFilename);
            Assert.False(ExistsResult);
        }

        [Fact, Order(1)]
        public void BlobBinaryFileCreateReadDeleteTest()
        {
            var blobFilename = @"d:\top1\sub1\temptest\multipage_tiff_example.tif";
            var localFileName = @"TestData/multipage_tiff_example.tif";
            var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
            var azureFile = azureBlobAdapter.File;

            var contentsWrite = System.IO.File.ReadAllBytes(localFileName);
            azureFile.WriteAllBytes(blobFilename, contentsWrite);

            var readData = azureFile.ReadAllBytes(blobFilename);
            Assert.Equal(contentsWrite, readData);

            azureFile.Delete(blobFilename);

            var ExistsResult = azureFile.Exists(blobFilename);
            Assert.False(ExistsResult);
        }

        //[Fact]
        //public void BlobFileReadTest()
        //{
        //    var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
        //    var azureFile = azureBlobAdapter.File
        //    var actualData = azureFile.ReadAllText(blobFileName);

        //    var expectedData = System.IO.File.ReadAllText(localFileName);

        //    Assert.Equal(expectedData, actualData);
        //}

        [Fact, Order(2)]
        public void BlobFileExistsTest()
        {
            var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
            var azureFile = azureBlobAdapter.File;
            //var notExistsResult = azureFile.Exists(new Guid().ToString()); //not exists already covered in delete test
            //Assert.False(notExistsResult);

            var existsResult = azureFile.Exists(blobFileName);
            Assert.True(existsResult);
        }

        [Fact, Order(3)]
        public void BlobFileGetLastWriteTimeTest()
        {
            var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
            var azureFile = azureBlobAdapter.File;
            var actualData = azureFile.GetLastWriteTime(blobFileName);

            var expectedData = DateTime.Parse("31-07-2020 10:34:59");

            Assert.Equal(actualData, actualData);
        }

        [Fact, Order(4)]
        public void BlobCopyMoveTests()
        {
            var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
            var azureFile = azureBlobAdapter.File;
            var localContent = System.IO.File.ReadAllText(localPermFileName);

            var copiedBlobFilename = @"\\hostname\shared\newtop\newsub\copied.json";
            azureFile.Copy( blobFileName, copiedBlobFilename, true );
            var copiedData = azureFile.ReadAllText(copiedBlobFilename);
            Assert.Equal(localContent, copiedData);

            var movedBlobFilename = @"\\hostname\shared\movedtop\movedsub\moved.json";
            azureFile.Move( copiedBlobFilename, movedBlobFilename );
            var movedData = azureFile.ReadAllText(movedBlobFilename);
            Assert.Equal(localContent, movedData); 

            var ExistsResult = azureFile.Exists(copiedBlobFilename);
            Assert.False(ExistsResult);

            //clean up so next time unit test will run
            azureFile.Delete(movedBlobFilename);
        }


        [Fact, Order(11)]
        public void BlobDriveTests()
        {
            var azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);

            var driveInfos = azureBlobAdapter.DriveInfo.GetDrives();
            Assert.True(driveInfos[0].IsReady); //always true

            Assert.Equal(azureBlobAdapter, driveInfos[0].FileSystem);

            //GetDrives
            Assert.Equal(driveInfos.Count(), azureBlobSettings.MountPoints.Length);

        }
    }
}
