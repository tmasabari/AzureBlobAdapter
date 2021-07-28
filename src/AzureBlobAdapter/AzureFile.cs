using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text;

namespace Azure.BlobAdapter
{
    /// <summary>
    /// Reference .NET implementation of System.IO calls https://github.com/microsoft/referencesource/blob/master/mscorlib/system/io/file.cs
    /// </summary>
    public partial class AzureFile : AzureBlob, IFile
    {
        public IFileSystem FileSystem
        {
            get
            {
                return _azureBlobAdapter;
            }
        }

        public AzureFile(AzureBlobAdapter azureBlobAdapter)
        {
            _azureBlobAdapter = azureBlobAdapter;
        }

        #region Helper methods


        #endregion

        #region Implemented Methods

        public Stream OpenRead(string path)
        {
            MemoryStream stream = new MemoryStream();
            DownloadFromBlobToStream(path, stream);
            //the Azure SDK writes the data from internet to stream and stream pointer will be at the end of the stream
            stream.Position = 0;    //stream.Seek(0, SeekOrigin.Begin);
            return stream;
        }
        public byte[] ReadAllBytes(string path)
        {
            using (var stream = OpenRead(path))
            {
                byte[] dataArray = new byte[stream.Length];
                stream.Read(dataArray, 0, dataArray.Length);
                return dataArray;
            }
        }
        public void WriteAllBytes(string path, byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            {
                CreateReplaceBlobFromStream(path, stream);
            }
        }

        /// <summary>
        /// using statement of reader  closes underliying stream as well 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public StreamReader OpenText(string path)
        {
            using (var stream = OpenRead(path))
            {
                //this is inline with reference .net implementation call default constructor of streamreader
                //https://docs.microsoft.com/en-us/dotnet/api/system.io.streamreader.-ctor?view=netframework-4.8#System_IO_StreamReader__ctor_System_IO_Stream_
                var reader = new StreamReader(stream);
                return reader;
            }
        }

        public string ReadAllText(string path, Encoding encoding)
        {
            using (var stream = OpenRead(path))
            {
                return stream.ReadAllText(encodingTobeUsedIfNotDetected: encoding, leaveStreamOpen: true);
            } //this will close the stream
        }

        public string ReadAllText(string path)
        {
            return ReadAllText(path, encoding: Encoding.UTF8);
        }

        public void WriteAllText(string path, string contents, Encoding encoding)
        {
            Debug.WriteLine("Called to write all text to blob:\n\t {0}\n", path);
            using (var stream = new MemoryStream())
            {
                InternalWriteAllText(stream, contents, encoding, true); //stream must be open after write so that it can be used by blob sdk
                CreateReplaceBlobFromStream(path, stream);
            }
        }

        public void WriteAllText(string path, string contents)
        {
            //old code always use utf8. //WriteAllBytes(path, Encoding.UTF8.GetBytes(contents ?? ""));
            //enoding inline with https://referencesource.microsoft.com/#mscorlib/system/io/streamwriter.cs,5516ce201dc06b5f,references
            WriteAllText(path, contents, StreamExtensions.noBOM);
        }

        //2020 08 04 appendblob feature is not yet supported for hierarchical namespace unitl then use below
        public override void AppendAllText(string path, string contents, Encoding encoding)
        {
            string existingText = ReadAllText(path, encoding);
            string newText = existingText + contents;
            WriteAllText(path, newText);
        }

        public void AppendAllText(string path, string contents)
        {
            //enoding inline with https://referencesource.microsoft.com/#mscorlib/system/io/streamwriter.cs,5516ce201dc06b5f,references
            AppendAllText(path, contents, StreamExtensions.noBOM);
        }


        public IEnumerable<string> ReadLines(string path, Encoding encoding)
        {
            using (var stream = OpenRead(path))
            {
                using (var reader = stream.GetStreamReader(encodingTobeUsedIfNotDetected: encoding, leaveStreamOpen: true))
                {
                    while (!reader.EndOfStream)
                    {
                        yield return reader.ReadLine();
                    }
                }
            }
        }
        public IEnumerable<string> ReadLines(string path)
        {
            //encoding is inline with .net framework https://referencesource.microsoft.com/#mscorlib/system/io/file.cs,019a819484daf2d2
            return ReadLines(path, Encoding.UTF8);
        }
        public string[] ReadAllLines(string path)
        {
            return ReadLines(path).ToArray();
        }
        public string[] ReadAllLines(string path, Encoding encoding)
        {
            return ReadLines(path, encoding).ToArray();
        }

        public void WriteAllLines(string path, IEnumerable<string> contents)
        {
            //enoding inline with https://referencesource.microsoft.com/#mscorlib/system/io/streamwriter.cs,5516ce201dc06b5f,references
            WriteAllLines(path, contents, StreamExtensions.noBOM);
        }

        public void WriteAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            WriteAllText(path, GetStringFromIEnumerable(contents), encoding);
        }

        public void WriteAllLines(string path, string[] contents)
        {
            //enoding inline with https://referencesource.microsoft.com/#mscorlib/system/io/streamwriter.cs,5516ce201dc06b5f,references
            WriteAllLines(path, contents, StreamExtensions.noBOM);
        }

        public void WriteAllLines(string path, string[] contents, Encoding encoding)
        {
            WriteAllLines(path, contents.AsEnumerable(), StreamExtensions.noBOM);
        }


        protected string GetStringFromIEnumerable(IEnumerable<string> contents)
        {
            // convert lines to string
            var sb = new StringBuilder();
            foreach (var item in contents)
            {
                sb.Append(item).Append(Environment.NewLine);
            }
            return sb.ToString();
        }

        public void AppendAllLines(string path, IEnumerable<string> contents, Encoding encoding)
        {
            AppendAllText(path, GetStringFromIEnumerable(contents), encoding);
        }

        public void AppendAllLines(string path, IEnumerable<string> contents)
        {
            //enoding inline with https://referencesource.microsoft.com/#mscorlib/system/io/streamwriter.cs,5516ce201dc06b5f,references
            AppendAllLines(path, contents, StreamExtensions.noBOM);
        }




        public DateTime GetLastWriteTime(string path)
        {
            return GetLastModified(path);
        }

        public void Copy(string sourceFileName, string destFileName)
        {
            Copy(sourceFileName, destFileName, overwrite: false);
        }

        #endregion

    }
}
