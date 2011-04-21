using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;

namespace SharpCompress.Archive.Zip
{
    public class ZipArchive : AbstractArchive<ZipArchiveEntry, ZipVolume>
    {
#if !PORTABLE
        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        public static ZipArchive Open(string filePath)
        {
            return Open(filePath, Options.None);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        public static ZipArchive Open(FileInfo fileInfo)
        {
            return Open(fileInfo, Options.None);
        }

        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        public static ZipArchive Open(string filePath, Options options)
        {
            filePath.CheckNotNullOrEmpty("filePath");
            return Open(new FileInfo(filePath), options);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        public static ZipArchive Open(FileInfo fileInfo, Options options)
        {
            fileInfo.CheckNotNull("fileInfo");
            return new ZipArchive(fileInfo, options);
        }
#endif

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        public static ZipArchive Open(Stream stream)
        {
            stream.CheckNotNull("stream");
            return Open(stream, Options.None);
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static ZipArchive Open(Stream stream, Options options)
        {
            stream.CheckNotNull("stream");
            return new ZipArchive(stream, options);
        }

#if !PORTABLE
        public static void ExtractToDirectory(string sourceArchive, string destinationDirectoryName)
        {
            ZipArchive archive = Open(sourceArchive);
            foreach (ZipArchiveEntry entry in archive.Entries)
            {
                string path = Path.Combine(destinationDirectoryName, Path.GetFileName(entry.FilePath));
                using (FileStream output = File.OpenWrite(path))
                {
                    entry.WriteTo(output);
                }
            }
        }

        public static bool IsZipFile(string filePath)
        {
            return IsZipFile(new FileInfo(filePath));
        }

        public static bool IsZipFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return false;
            }
            using (Stream stream = fileInfo.OpenRead())
            {
                return IsZipFile(stream);
            }
        }
#endif

        public static bool IsZipFile(Stream stream)
        {
            try
            {
                ZipHeader header = ZipHeaderFactory.ReadHeaderNonseekable(stream).FirstOrDefault();
                if (header == null)
                {
                    return false;
                }
                return Enum.IsDefined(typeof (ZipHeaderType), header.ZipHeaderType);
            }
            catch
            {
                return false;
            }
        }

#if !PORTABLE
        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        internal ZipArchive(FileInfo fileInfo, Options options)
            : base(fileInfo, options)
        {
        }

        protected override IEnumerable<ZipVolume> LoadVolumes(FileInfo file, Options options)
        {
            return new ZipVolume(file, options).AsEnumerable();
        }
#endif

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        internal ZipArchive(Stream stream, Options options)
            : base(stream.AsEnumerable(), options)
        {
        }

        protected override IEnumerable<ZipVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return new ZipVolume(streams.First(), options).AsEnumerable();
        }

        protected override IEnumerable<ZipArchiveEntry> LoadEntries(IEnumerable<ZipVolume> volumes)
        {
            foreach (ZipHeader h in ZipHeaderFactory.ReadHeaderNonseekable(volumes.Single().Stream))
            {
                if (h != null)
                {
                    switch (h.ZipHeaderType)
                    {
                        case ZipHeaderType.LocalEntry:
                            {
                                yield return new ZipArchiveEntry(new ZipFilePart(h as LocalEntryHeader));
                            }
                            break;
                        case ZipHeaderType.DirectoryEnd:
                            {
                                yield break;
                            }
                    }
                }
            }
        }
    }
}