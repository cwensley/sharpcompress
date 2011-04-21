using System;
using System.IO;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;

namespace SharpCompress.Archive
{
    public class ArchiveFactory
    {   /// <summary>
        /// Opens a Reader for Non-seeking usage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IArchive OpenArchive(Stream stream, IExtractionListener listener,
            Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            if (!stream.CanRead || !stream.CanSeek)
            {
                throw new ArgumentException("Stream should be readable and seekable");
            }

            if (ZipArchive.IsZipFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return ZipArchive.Open(stream, options);
            }
            stream.Seek(0, SeekOrigin.Begin);
            if (RarArchive.IsRarFile(stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                return RarArchive.Open(stream, options);
            }
            throw new InvalidOperationException("Cannot determine compressed stream type.");
        }

        /// <summary>
        /// Opens a CompressedStreamReader for Non-seeking usage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IArchive OpenArchive(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return OpenArchive(stream, new NullExtractionListener(), options);
        }

#if !PORTABLE
        public static void ExtractToDirectory(string sourceArchive, string destinationDirectoryName)
        {
            using (FileStream stream = File.OpenRead(sourceArchive))
            {
                IArchive archive = OpenArchive(stream);
                foreach (IArchiveEntry entry in archive.Entries)
                {
                    string path = Path.Combine(destinationDirectoryName, Path.GetFileName(entry.FilePath));
                    using (FileStream output = File.OpenWrite(path))
                    {
                        entry.WriteTo(output);
                    }
                }
            }
        }
#endif
    }
}
