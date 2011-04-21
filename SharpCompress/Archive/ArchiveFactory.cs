using System;
using System.IO;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;

namespace SharpCompress.Archive
{
    public class ArchiveFactory
    {
        /// <summary>
        /// Opens an Archive for random access
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IArchive Open(Stream stream, Options options = Options.KeepStreamsOpen)
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

#if !PORTABLE
        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        public static IArchive Open(string filePath)
        {
            return Open(filePath, Options.None);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        public static IArchive Open(FileInfo fileInfo)
        {
            return Open(fileInfo, Options.None);
        }

        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        public static IArchive Open(string filePath, Options options)
        {
            filePath.CheckNotNullOrEmpty("filePath");
            return Open(new FileInfo(filePath), options);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        public static IArchive Open(FileInfo fileInfo, Options options)
        {
            fileInfo.CheckNotNull("fileInfo");
            using (var stream = fileInfo.OpenRead())
            {
                if (ZipArchive.IsZipFile(stream))
                {
                    return ZipArchive.Open(fileInfo, options);
                }
                stream.Seek(0, SeekOrigin.Begin);
                if (RarArchive.IsRarFile(stream))
                {
                    return RarArchive.Open(fileInfo, options);
                }
                throw new InvalidOperationException("Cannot determine compressed stream type.");
            }
        }

        /// <summary>
        /// Extract to specific directory, retaining filename
        /// </summary>
        public static void WriteToDirectory(string sourceArchive, string destinationDirectory, IExtractionListener listener,
            ExtractOptions options = ExtractOptions.Overwrite)
        {
            IArchive archive = Open(sourceArchive);
            foreach (IArchiveEntry entry in archive.Entries)
            {
                entry.WriteToDirectory(destinationDirectory, listener, options);
            }
        }

        /// <summary>
        /// Extract to specific directory, retaining filename
        /// </summary>
        public static void WriteToDirectory(string sourceArchive, string destinationPath,
            ExtractOptions options = ExtractOptions.Overwrite)
        {
            WriteToDirectory(sourceArchive, destinationPath, new NullExtractionListener(), options);
        }
#endif
    }
}
