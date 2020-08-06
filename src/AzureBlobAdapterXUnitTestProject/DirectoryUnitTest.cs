using System;
using Xunit;
using Xunit.Extensions.Ordering;

using Azure.BlobAdapter;
using Microsoft.Extensions.Configuration;
using System.IO.Abstractions;
using System.Linq;
using System.Diagnostics;

namespace AzureBlobAdapterXUnitTestProject
{
    public class DirectoryUnitTest
    {
        AzureBlobSettings azureBlobSettings = new AzureBlobSettings();
        const string blobRootFolderName = @"\\hostname\shared\";
        const string blobFolderName = @"\\hostname\shared\sampledirectory\";
        const string localPermFileName = "TestData/temp.json";
        IFileSystem azureBlobAdapter;

        public DirectoryUnitTest()
        {
            IConfiguration Configuration = new ConfigurationBuilder()
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
              //.AddEnvironmentVariables() //.AddCommandLine(args)
              .Build();
            Configuration.Bind("AzureBlobSettings", azureBlobSettings);

            azureBlobAdapter = new AzureBlobAdapter(azureBlobSettings);
        }

        [Fact, Order(1)]
        public void FolderCreateDeleteExistsTest()
        {
            //https://stackoverflow.com/questions/5748032/why-does-path-combine-produce-this-result-with-a-relative-path
            //  trim the slash off if you want to guarantee that relativePath will be treated as relative. remove leading slash of \new folder
            var newFolderName = azureBlobAdapter.Path.Combine(  blobRootFolderName , @"new folder\subfolder\leaf" );
            var azureDirectory = azureBlobAdapter.Directory;

            azureDirectory.CreateDirectory(newFolderName);
            var existsResult = azureDirectory.Exists(blobFolderName);
            Assert.True(existsResult);

            azureDirectory.Delete(newFolderName);
            var leafExistsResult = azureDirectory.Exists(newFolderName);
            Assert.False(leafExistsResult);

            var deleteRoot = azureBlobAdapter.Path.Combine(blobRootFolderName, @"new folder");
            azureDirectory.Delete(deleteRoot, recursive:true);
            var rootExistsResult = azureDirectory.Exists(deleteRoot);
            Assert.False(rootExistsResult);
        }


        [Fact, Order(2)]
        public void BlobFolderListTest()
        {
            var azureDirectory = azureBlobAdapter.Directory;

            // List all directories in the folder
            var folders = azureDirectory.GetDirectories(blobRootFolderName);
            Assert.True(folders.Count() > 0);
            foreach (var directoryName in folders)
            {
                Debug.WriteLine("Directory Name \t" + directoryName);
            }

            // List all blobs in the folder
            var files = azureDirectory.GetFiles(blobFolderName);
            Assert.True(files.Count() > 0);
            foreach (var fileName in files)
            {
                Debug.WriteLine("File Name \t" + fileName);
            }

            // List all items in the folder
            var items = azureDirectory.EnumerateFileSystemEntries(blobFolderName);
            Assert.True(files.Count() > 0);
            foreach (var fileName in files)
            {
                Debug.WriteLine("File Name \t" + fileName);
            }
        }

        [Fact, Order(3)]
        public void FolderCopyMoveTests()
        {
            var azureDirectory = azureBlobAdapter.Directory;
            var newFolderName = azureBlobAdapter.Path.Combine(blobRootFolderName, @"new folder\subfolder\leaf");
            //azureDirectory.CreateDirectory(newFolderName);
            //this tests both azureDirectory.GetFiles(blobFolderName)  azureDirectory.EnumerateFileSystemEntries(blobFolderName)
            FileFolderExtensions.CopyDirectory(azureBlobAdapter, blobFolderName, newFolderName);

            var  olditems = azureDirectory.EnumerateFileSystemEntries(blobFolderName, null, System.IO.SearchOption.AllDirectories);
            var  newitems = azureDirectory.EnumerateFileSystemEntries(newFolderName, null, System.IO.SearchOption.AllDirectories);
            foreach (var fileName in newitems)
            {
                Debug.WriteLine("File Name \t" + fileName);
            }
            Assert.Equal(newitems.Count(), olditems.Count());

            var sourceRootFolderName = azureBlobAdapter.Path.Combine(blobRootFolderName, @"new folder");

            // The parent directory of the destination path must exist.
            var movedRootFolderName = @"\\hostname\shared\movedtop\movedsub\moved";
            azureDirectory.CreateDirectory(movedRootFolderName);
            azureDirectory.Move(sourceRootFolderName, movedRootFolderName);

            var ExistsResult = azureDirectory.Exists( sourceRootFolderName );
            Assert.False(ExistsResult);
            var ExistsResultTrue = azureDirectory.Exists(@"\\hostname\shared\movedtop\movedsub\moved\subfolder\leaf");
            Assert.True(ExistsResultTrue);

            //clean up so next time unit test will run
            azureDirectory.Delete(@"\\hostname\shared\movedtop", true);
        }
    }
}
