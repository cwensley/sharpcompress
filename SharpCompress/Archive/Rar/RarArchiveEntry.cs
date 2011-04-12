using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Compressor.Rar;

namespace SharpCompress.Archive.Rar
{
    public class RarArchiveEntry : RarEntry
    {
        private readonly ICollection<RarFilePart> parts;

        internal RarArchiveEntry(RarArchive archive, IEnumerable<RarFilePart> parts)
        {
            this.parts = parts.ToList();
            Archive = archive;
        }

        private RarArchive Archive { get; set; }

        internal override IEnumerable<FilePart> Parts
        {
            get { return parts.Cast<FilePart>(); }
        }

        internal override FileHeader FileHeader
        {
            get { return parts.First().FileHeader; }
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
            get { return parts.Aggregate(0L, (total, fp) => { return total + fp.FileHeader.UncompressedSize; }); }
        }

        public override long CompressedSize
        {
            get { return parts.Aggregate(0L, (total, fp) => { return total + fp.FileHeader.CompressedSize; }); }
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

        #region Internal work

        private static IEnumerable<RarFilePart> GetFileParts(IEnumerable<RarArchiveVolume> parts)
        {
            foreach (RarVolume rarPart in parts)
            {
                foreach (RarFilePart fp in rarPart.ReadFileParts())
                {
                    yield return fp;
                }
            }
        }

        private static IEnumerable<IEnumerable<RarFilePart>> GetMatchedFileParts(IEnumerable<RarArchiveVolume> parts)
        {
            var groupedParts = new List<RarFilePart>();
            foreach (RarFilePart fp in GetFileParts(parts))
            {
                groupedParts.Add(fp);

                if (!fp.FileHeader.FileFlags.HasFlag(FileFlags.SPLIT_AFTER))
                {
                    yield return groupedParts;
                    groupedParts = new List<RarFilePart>();
                }
            }
        }

        internal static IEnumerable<RarArchiveEntry> GetEntries(RarArchive archive,
                                                                IEnumerable<RarArchiveVolume> rarParts)
        {
            foreach (var groupedParts in GetMatchedFileParts(rarParts))
            {
                yield return new RarArchiveEntry(archive, groupedParts);
            }
        }

        #endregion
    }
}