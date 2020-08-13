using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Azure.BlobAdapter
{
    public static class StreamExtensions
    {
        //https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/streamreader.cs
        public static int DefaultBufferSize { get; } = 1024; //1 kb

        public static Encoding noBOM
        {
            get
            {
                UTF8Encoding noBOM = new UTF8Encoding(false, true);
                return noBOM;
            }
        }

        public static String ReadAllText(this Stream stream, Encoding encodingTobeUsedIfNotDetected, bool leaveStreamOpen)
        {
            //Contract.Requires(path != null);
            //Contract.Requires(encoding != null);
            //Contract.Requires(path.Length > 0);

            using (StreamReader sr = GetStreamReader(stream, encodingTobeUsedIfNotDetected, leaveStreamOpen))
                return sr.ReadToEnd();
        }

        public static StreamWriter GetStreamWriter(this Stream stream, Encoding encoding, bool leaveStreamOpen)
        {
            return new StreamWriter(stream, encoding, DefaultBufferSize, leaveStreamOpen);
        }

        // Get encoding is not required Streamreader already has detection implementation
        // refer DetectEncoding method in https://github.com/Microsoft/referencesource/blob/master/mscorlib/system/io/streamreader.cs
        //  it detects UTF8, UTF32, Unicode and any other new encodings not listed in the reference code
        //detailed code https://gist.github.com/TaoK/945127
        ///// <summary
        ///// Get File's Encoding
        ///// </summary>
        ///// <param name="filename">The path to the file
        //protected static Encoding GetEncoding(Stream stream)
        //{
        //    // This is a direct quote from MSDN:  
        //    // The CurrentEncoding value can be different after the first
        //    // call to any Read method of StreamReader, since encoding
        //    // autodetection is not done until the first call to a Read method.

        //    using (var reader = new StreamReader(stream, Encoding.Default, true))
        //    {
        //        if (reader.Peek() >= 0) // you need this!
        //            reader.Read();

        //        return reader.CurrentEncoding;
        //    }
        //}

        /// <summary>
        ///Different .net framework use different encoding by default
        // https://jeremylindsayni.wordpress.com/2019/01/23/correctly-reading-encoded-text-with-the-streamreader-in-net/
        // https://www.arclab.com/en/kb/csharp/read-write-text-file-ansi-utf8-unicode.html
        /// The encoding of the text file is important. Common encodings are:
        ///     Encoding.Default: Operation system current ANSI codepage US language Windows-1252
        ///     Encoding.UTF8: utf-8 format (e.g.used for html pages)
        ///     Encoding.Unicode: Unicode format(utf-16 little endian encoding, a.k.a.UCS-2 LE)
        ///     
        //https://www.codeproject.com/Articles/96707/Detect-Encoding-from-ByteOrderMarks-BOM
        ///  The StreamReader object attempts to detect the encoding by looking at the first four bytes of the stream. 
        ///  It automatically recognizes UTF-8, little-endian Unicode, big-endian Unicode, little-endian UTF-32, and big-endian UTF-32 text 
        ///     if the file starts with the appropriate byte order marks. 
        ///     Otherwise, the user-provided encoding is used.
        ///     
        //  https://weblog.west-wind.com/posts/2007/nov/28/detecting-text-encoding-for-streamreader
        /// Encoding.UTF8 and Encoding.Unicode adds a BOM (Byte Order Mark) to the file.
        ///     The byte order mark(BOM) is a unicode character(at start), which signals the encoding of the text stream(file).
        /// StreamReader() specifically has an overload that's supposed to help with detection of byte order marks 
        ///     and based on that is supposed to sniff the document's encoding. It actually works but only if the content is encoded as UTF-8/16/32 
        ///     ie. when it actually has a byte order mark. It doesn't revert back to Encoding.Default if it can't find a byte order mark 
        ///     the default without a byte order mark is UTF-8 IMO a suboptimal choice, since Utf8 can be detected 
        ///     which usually will result in invalid text parsing.
        ///     for any random TXT file, - ANSI is usually a good default
        ///     but for XML files - regardless of the BOM, their default encoding should be Utf8, so defaulting to Utf8 seems quite appropriate
        /// </summary>
        /// <param name="path"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static StreamReader GetStreamReader(this Stream stream, Encoding encodingTobeUsedIfNotDetected, bool leaveStreamOpen)
        {
            return new StreamReader(stream,
                    encoding: encodingTobeUsedIfNotDetected,
                    detectEncodingFromByteOrderMarks: true,
                    bufferSize: DefaultBufferSize,
                    leaveOpen: leaveStreamOpen);
        }

    }
}
