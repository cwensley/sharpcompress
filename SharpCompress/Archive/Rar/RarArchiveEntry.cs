using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Compressor.Rar;

namespace SharpCompress.Archive.Rar
{
    public class RarArchiveEntry : RarEntry, IArchiveEntry
    {
        private readonly ICollection<RarFilePart> parts;

        internal RarArchiveEntry(RarArchive archive, IEnumerable<RarFilePart> parts)
        {
            this.parts = parts.ToList();
            Archive = archive;
        }

        private RarArchive Archive
        {
            get;
            set;
        }

        internal override IEnumerable<FilePart> Parts
        {
            get
            {
                return parts.Cast<FilePart>();
            }
        }

        internal override FileHeader FileHeader
        {
            get
            {
                return parts.First().FileHeader;
            }
        }

        public override uint Crc
        {
            get
            {
                return parts.Select(fp => fp.FileHeader)
                    .Where(fh => !fh.FileFlags.HasFlag(FileFlags.SPLIT_AFTER))
                    .Single().FileCRC;
            }
        }

        public override long Size
        {
            get
            {
                return parts.Aggregate(0L, (total, fp) =>
                {
                    return total + fp.FileHeader.UncompressedSize;
                });
            }
        }

        public override long CompressedSize
        {
            get
            {
                return parts.Aggregate(0L, (total, fp) =>
                {
                    return total + fp.FileHeader.CompressedSize;
                });
            }
        }

        public void WriteTo(Stream streamToWriteTo, IExtractionListener listener)
        {
            if (IsEncrypted)
            {
                throw new RarExtractionException("Entry is password protected and cannot be extracted.");
            }

            if (IsDirectory)
            {
                throw new RarExtractionException("Entry is a file directory and cannot be extracted.");
            }

            listener.CheckNotNull("listener");
            listener.OnFileEntryExtractionInitialized(FilePath, CompressedSize);
            using (Stream input = new MultiVolumeReadOnlyStream(parts, listener))
            {
                var pack = new Unpack(FileHeader, input, streamToWriteTo);
                pack.doUnpack(Archive.IsSolidArchive());
            }
        }
    }
}