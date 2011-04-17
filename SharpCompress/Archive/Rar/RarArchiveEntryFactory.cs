using System.Collections.Generic;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Archive.Rar
{
    internal static class RarArchiveEntryFactory
    {
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
    }
}
