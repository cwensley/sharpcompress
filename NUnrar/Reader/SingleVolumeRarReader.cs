using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Rar;

namespace SharpCompress.Reader
{
    internal class SingleVolumeRarReader : RarReader
    {
        private readonly Stream stream;

        internal SingleVolumeRarReader(Stream stream, ReaderOptions options, IExtractionListener listener)
            : base(options, listener)
        {
            this.stream = stream;
        }

        internal override void ValidateArchive(RarVolume archive)
        {
            if (archive.IsMultiVolume)
            {
                throw new RarExtractionException("Streamed archive is a Multi-volume archive.  Use different RarReader method to extract.");
            }
        }

        internal override Stream RequestInitialStream()
        {
            return stream;
        }

        internal override IEnumerable<FilePart> CreateFilePartEnumerableForCurrentEntry()
        {
            return Entry.Parts;
        }
    }
}
