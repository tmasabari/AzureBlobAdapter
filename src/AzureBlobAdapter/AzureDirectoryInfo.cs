using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;

namespace Azure.BlobAdapter
{
    public class AzureDirectoryInfo : IDirectoryInfo
    {
        protected AzureBlobAdapter _azureBlobAdapter;
        public IFileSystem FileSystem
        {
            get
            {
                return _azureBlobAdapter;
            }
        }



        #region Implemented

        #endregion


        public IDirectoryInfo Parent => throw new NotImplementedException();

        public IDirectoryInfo Root => throw new NotImplementedException();

        public FileAttributes Attributes { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime CreationTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime CreationTimeUtc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public bool Exists => throw new NotImplementedException();

        public string Extension => throw new NotImplementedException();

        public string FullName => throw new NotImplementedException();

        public DateTime LastAccessTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime LastAccessTimeUtc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime LastWriteTime { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public DateTime LastWriteTimeUtc { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public string Name => throw new NotImplementedException();

        public AzureDirectoryInfo(AzureBlobAdapter azureBlobAdapter)
        {
            _azureBlobAdapter = azureBlobAdapter;
        }

        public void Create()
        {
            throw new NotImplementedException();
        }

        public void Create(DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public IDirectoryInfo CreateSubdirectory(string path)
        {
            throw new NotImplementedException();
        }

        public void Delete(bool recursive)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDirectoryInfo> EnumerateDirectories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDirectoryInfo> EnumerateDirectories(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileInfo> EnumerateFiles()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileInfo> EnumerateFiles(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IFileSystemInfo> EnumerateFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public DirectorySecurity GetAccessControl()
        {
            throw new NotImplementedException();
        }

        public DirectorySecurity GetAccessControl(AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        public IDirectoryInfo[] GetDirectories()
        {
            throw new NotImplementedException();
        }

        public IDirectoryInfo[] GetDirectories(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IDirectoryInfo[] GetDirectories(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IFileInfo[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public IFileInfo[] GetFiles(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IFileInfo[] GetFiles(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public IFileSystemInfo[] GetFileSystemInfos()
        {
            throw new NotImplementedException();
        }

        public IFileSystemInfo[] GetFileSystemInfos(string searchPattern)
        {
            throw new NotImplementedException();
        }

        public IFileSystemInfo[] GetFileSystemInfos(string searchPattern, SearchOption searchOption)
        {
            throw new NotImplementedException();
        }

        public void MoveTo(string destDirName)
        {
            throw new NotImplementedException();
        }

        public void SetAccessControl(DirectorySecurity directorySecurity)
        {
            throw new NotImplementedException();
        }

        public void Delete()
        {
            throw new NotImplementedException();
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }


    }
}
