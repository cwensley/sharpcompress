using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.IO;

namespace SharpCompress.Archive.Rar
{
    public class RarArchive : AbstractArchive<RarArchiveEntry, RarArchiveVolume>
    {
#if !PORTABLE
        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        internal RarArchive(FileInfo fileInfo, Options options)
            : base(fileInfo, options)
        {
        }

        protected override IEnumerable<RarArchiveVolume> LoadVolumes(FileInfo file, Options options)
        {
            return RarArchiveVolumeFactory.GetParts(file, options);
        }
#endif

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        internal RarArchive(IEnumerable<Stream> streams, Options options)
            : base(streams, options)
        {
        }

        protected override IEnumerable<RarArchiveEntry> LoadEntries(IEnumerable<RarArchiveVolume> volumes)
        {
            return RarArchiveEntryFactory.GetEntries(this, volumes);
        }

        protected override IEnumerable<RarArchiveVolume> LoadVolumes(IEnumerable<Stream> streams, Options options)
        {
            return RarArchiveVolumeFactory.GetParts(streams, options);
        }

        #region Creation

#if !PORTABLE
        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        public static RarArchive Open(string filePath)
        {
            return Open(filePath, Options.None);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        public static RarArchive Open(FileInfo fileInfo)
        {
            return Open(fileInfo, Options.None);
        }

        /// <summary>
        /// Constructor expects a filepath to an existing file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="options"></param>
        public static RarArchive Open(string filePath, Options options)
        {
            filePath.CheckNotNullOrEmpty("filePath");
            return Open(new FileInfo(filePath), options);
        }

        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        public static RarArchive Open(FileInfo fileInfo, Options options)
        {
            fileInfo.CheckNotNull("fileInfo");
            return new RarArchive(fileInfo, options);
        }
#endif
        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        public static RarArchive Open(Stream stream)
        {
            stream.CheckNotNull("stream");
            return Open(stream.AsEnumerable());
        }

        /// <summary>
        /// Takes a seekable Stream as a source
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        public static RarArchive Open(Stream stream, Options options)
        {
            stream.CheckNotNull("stream");
            return Open(stream.AsEnumerable(), options);
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        public static RarArchive Open(IEnumerable<Stream> streams)
        {
            streams.CheckNotNull("streams");
            return new RarArchive(streams, Options.KeepStreamsOpen);
        }

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        public static RarArchive Open(IEnumerable<Stream> streams, Options options)
        {
            streams.CheckNotNull("streams");
            return new RarArchive(streams, options);
        }

#if !PORTABLE
        public static void ExtractToDirectory(string sourceArchive, string destinationDirectoryName)
        {
            RarArchive archive = RarArchive.Open(sourceArchive);
            foreach (RarArchiveEntry entry in archive.Entries)
            {
                string path = Path.Combine(destinationDirectoryName, Path.GetFileName(entry.FilePath));
                using (FileStream output = File.OpenWrite(path))
                {
                    entry.WriteTo(output);
                }
            }
        }

        public static bool IsRarFile(string filePath)
        {
            return IsRarFile(new FileInfo(filePath));
        }

        public static bool IsRarFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return false;
            }
            using (Stream stream = fileInfo.OpenRead())
            {
                return IsRarFile(stream);
            }
        }
#endif
        public static bool IsRarFile(Stream stream)
        {
            try
            {

                RarHeaderFactory headerFactory = new RarHeaderFactory(StreamingMode.Seekable, Options.CheckForSFX);
                RarHeader header = headerFactory.ReadHeaders(stream).FirstOrDefault();
                if (header == null)
                {
                    return false;
                }
                return Enum.IsDefined(typeof(HeaderType), header.HeaderType);
            }
            catch
            {
                return false;
            }
        }
        #endregion
    }
}