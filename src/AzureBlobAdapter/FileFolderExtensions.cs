using System.IO.Abstractions;

namespace Azure.BlobAdapter
{
    public static class FileFolderExtensions
    {

        public static void CopyDirectory(IFileSystem fileSystem, string sourcePath, string destinationPath)
        {
            var azureDirectory = fileSystem.Directory;
            var azureFile = fileSystem.File;

            if (!sourcePath.EndsWith(fileSystem.Path.DirectorySeparatorChar.ToString()))
            {
                sourcePath += fileSystem.Path.DirectorySeparatorChar.ToString();
            }
            if (!destinationPath.EndsWith(fileSystem.Path.DirectorySeparatorChar.ToString()))
            {
                destinationPath += fileSystem.Path.DirectorySeparatorChar.ToString();
            }

            //Create current directory first
            azureDirectory.CreateDirectory(destinationPath);

            //Copy all the files & Replaces any files with the same name
            foreach (string sourceFilePath in azureDirectory.GetFiles(sourcePath))
            {
                var newPath = sourceFilePath.Replace(sourcePath, destinationPath);
                azureFile.Copy(sourceFilePath, newPath, true);
            }

            // Copy each subdirectory using recursion.
            foreach (string sourceDirPath in azureDirectory.GetDirectories(sourcePath))
            {
                var newPath = sourceDirPath.Replace(sourcePath, destinationPath);
                CopyDirectory(fileSystem, sourceDirPath, newPath);
            }
        }
    }
}
