
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.IO;
using SharpCompress.Rar;
using SharpCompress.Common.Rar;

namespace SharpCompress.Archive.Rar
{
    internal class StreamRarArchiveVolume : RarArchiveVolume
    {
        private bool streamOwner;
        private Stream stream;

        internal StreamRarArchiveVolume(Stream stream, ReaderOptions options)
            : base(StreamingMode.Seekable, options)
        {
            this.stream = stream;
            this.streamOwner = !options.HasFlag(ReaderOptions.KeepStreamsOpen);
        }

        internal override Stream Stream
        {
            get { return stream; }
        }
#if !PORTABLE
        public override FileInfo VolumeFile
        {
            get { return null; }
        }
#endif

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return GetVolumeFileParts();
        }

        internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
        {
            return new SeekableStreamFilePart(markHeader, fileHeader, Stream, streamOwner);
        }
    }
}
