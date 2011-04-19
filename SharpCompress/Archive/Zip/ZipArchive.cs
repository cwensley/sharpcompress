using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;

namespace SharpCompress.Archive.Zip
{
    public class ZipArchive : AbstractArchive<ZipEntry, ZipVolume>
    {
#if !PORTABLE
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
                return Enum.IsDefined(typeof(ZipHeaderType), header.ZipHeaderType);
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
        /// <param name="streams"></param>
        /// <param name="options"></param>
        internal ZipArchive(Stream stream, Options options)
            : base(stream.AsEnumerable(), options)
        {
        }

        protected override IEnumerable<ZipVolume> LoadVolumes(IEnumerable<Stream> streams, Common.Options options)
        {
            return new ZipVolume(streams.First(), options).AsEnumerable();
        }

        protected override IEnumerable<ZipEntry> LoadEntries(IEnumerable<ZipVolume> volumes)
        {
            throw new NotImplementedException();
        }
    }
}
