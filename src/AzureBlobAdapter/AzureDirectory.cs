using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using System.Diagnostics;

using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;
using Azure.Storage.Files.DataLake;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake.Models;
using System.Runtime.InteropServices;
using System.Linq;

using SearchOption = System.IO.SearchOption;
using System.Threading;
using System.Text.RegularExpressions;

namespace Azure.BlobAdapter
{
    /// <summary>
    /// refer https://docs.microsoft.com/en-us/azure/storage/blobs/data-lake-storage-directory-file-acl-dotnet
    /// </summary>
    public partial class AzureDirectory : IDirectory
    {
        readonly AzureBlobAdapter _azureBlobAdapter;

        public IFileSystem FileSystem
        {
            get
            {
                return _azureBlobAdapter;
            }
        }

        public AzureDirectory(AzureBlobAdapter azureBlobAdapter)
        {
            _azureBlobAdapter = azureBlobAdapter;
        }

        #region Helper methods

        protected DataLakeFileSystemClient GetFileSystemClient(string fileSystemName)
        {
            //public BlobContainerClient BlobContainerClientObject { get; private set; }
            var fileSystemClient = _azureBlobAdapter.DataLakeServiceClientObject.GetFileSystemClient(fileSystemName);
            return fileSystemClient;
        }

        public DataLakeDirectoryClient GetDirectoryClient(string directoryFullPath)
        {
            var Names = _azureBlobAdapter.ExtractContainerBlobPortions(directoryFullPath);
            var fileSystemName = _azureBlobAdapter.ExtractContainerName(Names.Item1);

            var fileSystemClient = GetFileSystemClient(fileSystemName);
            DataLakeDirectoryClient directoryClient =
                fileSystemClient.GetDirectoryClient(Names.Item2);

            return directoryClient;
        }

        public virtual string NormalizeToBlobPath(string directoryPath)
        {
            //change the directory separators and directory name cannot contain trailing slash
            var blobDirectoryPath = directoryPath.Replace(_azureBlobAdapter.DirectorySeparator, _azureBlobAdapter.BlobPathSeparator);
            blobDirectoryPath = blobDirectoryPath.TrimEnd(_azureBlobAdapter.DirectorySeparator.Take(1).First());
            return blobDirectoryPath;
        }

        public virtual string ConvertWildcardPatternToRegex(string wildCardPattern)
        {
            return "^" //beginning of pattern
                + Regex.Escape(wildCardPattern).
               //convert * to regex
               Replace("\\*", ".*").
               //convert ? to regex $
               Replace("\\?", ".") 
               + "$"; //end of pattern
        }

        public virtual IEnumerable<string> InternalEnumerate(string path, string searchPattern, 
            SearchOption searchOption, bool Directories, bool Files)
        {
            var Names = _azureBlobAdapter.ExtractContainerBlobPortions(path);
            var rootPath = Names.Item1;
            var fileSystemName = _azureBlobAdapter.ExtractContainerName(rootPath);

            var fileSystemClient = GetFileSystemClient(fileSystemName);

            bool isRecursive = searchOption == SearchOption.AllDirectories;
            var directoryName = NormalizeToBlobPath(Names.Item2);

            IEnumerable<PathItem> names = fileSystemClient.GetPaths(directoryName, isRecursive);
            if (!string.IsNullOrEmpty(searchPattern))
            {
                searchPattern = ConvertWildcardPatternToRegex(searchPattern);
                names = names.Where(pathItem => Regex.Match(
                    _azureBlobAdapter.Path.GetFileName( pathItem.Name), searchPattern).Success ) ;
            }

            if (Directories && !Files)
                names = names.Where(pathItem => pathItem.IsDirectory ?? false);
            else if (!Directories && Files)
                names = names.Where(pathItem => !(pathItem.IsDirectory ?? false));
            //if both flag set or not set return all names

            return names.Select(pathItem =>
                    _azureBlobAdapter.Path.Combine(rootPath, pathItem.Name.Replace(
                        _azureBlobAdapter.BlobPathSeparator, _azureBlobAdapter.DirectorySeparator))
                );
        }
        #endregion

        public IDirectoryInfo CreateDirectory(string directoryFullPath)
        {
            DataLakeDirectoryClient directoryClient = GetDirectoryClient(directoryFullPath);
            directoryClient.CreateIfNotExists();
            //var properties = directoryClient.GetProperties().Value;

            //IDirectoryInfo directoryInfo = new AzureDirectoryInfo(_azureBlobAdapter)
            //{
            //    LastWriteTime = properties.LastModified.DateTime
            //    //, Name = properties.
            //};

            return null;  //class for IDirectoryInfo is not implemented
        }

        public void Delete(string path)
        {
            Delete(path, false);
        }

        public void Delete(string path, bool recursive)
        {
            DataLakeDirectoryClient directoryClient = GetDirectoryClient(path);
            directoryClient.Delete(recursive);
        }

        //There is no difference between moving and renaming; you should simply call Directory.Move
        public void Move(string sourceDirName, string destDirName)
        {
            DataLakeDirectoryClient directoryClient = GetDirectoryClient(sourceDirName);
            var Names = _azureBlobAdapter.ExtractContainerBlobPortions(destDirName);
            directoryClient.Rename( NormalizeToBlobPath(Names.Item2) );
        }

        public bool Exists(string path)
        {
            DataLakeDirectoryClient directoryClient = GetDirectoryClient(path);
            return directoryClient.Exists();
        }

        #region Enumerate and list drives, directories, files. both

        public IEnumerable<string> EnumerateDirectories(string path)
        {
            return EnumerateDirectories(path, searchPattern: null);
        }
        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern)
        {
            return EnumerateDirectories(path, searchPattern: searchPattern, searchOption: SearchOption.TopDirectoryOnly);
        }
        public IEnumerable<string> EnumerateDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return InternalEnumerate(path, searchPattern, searchOption, Directories:true, Files:false);
        }

        public IEnumerable<string> EnumerateFiles(string path)
        {
            return EnumerateFiles(path, searchPattern: null);
        }
        public IEnumerable<string> EnumerateFiles(string path, string searchPattern)
        {
            return EnumerateFiles(path, searchPattern: searchPattern, searchOption: SearchOption.TopDirectoryOnly);
        }
        public IEnumerable<string> EnumerateFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return InternalEnumerate(path, searchPattern, searchOption, Directories: false, Files: true);
        }

        public IEnumerable<string> EnumerateFileSystemEntries(string path)
        {
            return EnumerateFileSystemEntries(path, searchPattern: null);
        }
        public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern)
        {
            return EnumerateFileSystemEntries(path, searchPattern: searchPattern, searchOption: SearchOption.TopDirectoryOnly);
        }
        public IEnumerable<string> EnumerateFileSystemEntries(string path, string searchPattern, SearchOption searchOption)
        {
            return InternalEnumerate(path, searchPattern, searchOption, Directories: true, Files: true);
        }

        public string[] GetDirectories(string path)
        {
            return GetDirectories(path, searchPattern: null);
        }
        public string[] GetDirectories(string path, string searchPattern)
        {
            return GetDirectories(path, searchPattern: searchPattern, searchOption: SearchOption.TopDirectoryOnly);
        }
        public string[] GetDirectories(string path, string searchPattern, SearchOption searchOption)
        {
            return EnumerateDirectories(path, searchPattern, searchOption).ToArray();
        }

        public string[] GetFiles(string path)
        {
            return GetFiles(path, searchPattern: null);
        }
        public string[] GetFiles(string path, string searchPattern)
        {
            return GetFiles(path, searchPattern: searchPattern, searchOption: SearchOption.TopDirectoryOnly);
        }
        public string[] GetFiles(string path, string searchPattern, SearchOption searchOption)
        {
            return EnumerateFiles(path, searchPattern, searchOption).ToArray();
        }

        public string[] GetFileSystemEntries(string path)
        {
            return GetFileSystemEntries(path, searchPattern: null);
        }
        public string[] GetFileSystemEntries(string path, string searchPattern)
        {
            return EnumerateFileSystemEntries(path, searchPattern: searchPattern, searchOption: SearchOption.TopDirectoryOnly).ToArray();
        }
 
        public string[] GetLogicalDrives()
        {
            var azureDriveFactory = (AzureDriveInfoFactory) _azureBlobAdapter.DriveInfo;
            return azureDriveFactory.GetDrivesOnly()
                .Select(driveInfo => driveInfo.Name + _azureBlobAdapter.DirectorySeparator ).ToArray();
        }
        #endregion


        public string GetDirectoryRoot(string path)
        {
            return _azureBlobAdapter.Path.GetPathRoot(path); 
        }

        public void UploadLocalFolder(string localFolderPath, string AzureFolderPath)
        {
            //if target directory not exists create it
            if (!this.Exists(AzureFolderPath))
                this.CreateDirectory(AzureFolderPath);

            // Copy each file into the new directory.
            var filePaths = System.IO.Directory.EnumerateFiles(localFolderPath);
            foreach (var filePath in filePaths)
            {
                string filenameWithExt = new System.IO.FileInfo(filePath).Name;
                // filenameWithExt = filePath.Replace(localFolderPath, "");
                string azurePath = this.FileSystem.Path.Combine(AzureFolderPath, filenameWithExt);
                Debug.WriteLine(@"Copying {0} to azure {1}", filePath, azurePath);
                AzureFile azureFile = (AzureFile)this.FileSystem.File;
                azureFile.UploadToBlob(filePath, azurePath, isOverWrite: true);
            }
            // Copy each subdirectory using recursion.
            var subFolderPaths = System.IO.Directory.EnumerateDirectories(localFolderPath);
            foreach ( var subFolderPath in subFolderPaths )
            {
                string folderNameOnly = new System.IO.DirectoryInfo(subFolderPath).Name; //subFolderPath.Replace(localFolderPath, "");
                string azureSubFolderPath = this.FileSystem.Path.Combine(AzureFolderPath, folderNameOnly);
                UploadLocalFolder(subFolderPath, azureSubFolderPath);
            }
        }
    }
}
