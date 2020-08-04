using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.IO.Abstractions;
using System.Security.AccessControl;
using System.Text;

namespace Azure.BlobAdapter
{
    /// <summary>
    /// Reference .NET implementation of System.IO calls https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/file.cs
    /// </summary>
    public partial class AzureFile
    {
        [ExcludeFromCodeCoverage]
        public StreamWriter AppendText(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream Create(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream Create(string path, int bufferSize)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream Create(string path, int bufferSize, FileOptions options)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public StreamWriter CreateText(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void Decrypt(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void Encrypt(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public FileSecurity GetAccessControl(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public FileSecurity GetAccessControl(string path, AccessControlSections includeSections)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public FileAttributes GetAttributes(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public DateTime GetCreationTime(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public DateTime GetCreationTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public DateTime GetLastAccessTime(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public DateTime GetLastAccessTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public DateTime GetLastWriteTimeUtc(string path)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream Open(string path, FileMode mode)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream Open(string path, FileMode mode, FileAccess access)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream Open(string path, FileMode mode, FileAccess access, FileShare share)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public Stream OpenWrite(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Replace is more related to the File system properties and security related retention. This is not replacable in Azure.
        /// This is not directy equivalent to Delet + Move + Backup as well.
        /// Refer: https://stackoverflow.com/questions/5989987/difference-between-file-replace-and-file-deletefile-move-in-c-sharp
        /// </summary>
        /// <param name="sourceFileName"></param>
        /// <param name="destinationFileName"></param>
        /// <param name="destinationBackupFileName"></param>
        [ExcludeFromCodeCoverage]
        public void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void Replace(string sourceFileName, string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetAccessControl(string path, FileSecurity fileSecurity)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetAttributes(string path, FileAttributes fileAttributes)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetCreationTime(string path, DateTime creationTime)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetCreationTimeUtc(string path, DateTime creationTimeUtc)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetLastAccessTime(string path, DateTime lastAccessTime)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetLastAccessTimeUtc(string path, DateTime lastAccessTimeUtc)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetLastWriteTime(string path, DateTime lastWriteTime)
        {
            throw new NotImplementedException();
        }

        [ExcludeFromCodeCoverage]
        public void SetLastWriteTimeUtc(string path, DateTime lastWriteTimeUtc)
        {
            throw new NotImplementedException();
        }
    }
}
