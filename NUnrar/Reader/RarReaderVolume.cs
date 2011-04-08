using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Headers;
using SharpCompress.IO;
using SharpCompress.Rar;

namespace SharpCompress.Reader
{
    public class RarReaderVolume : RarVolume
    {
        private bool streamOwner;
        private Stream stream;

        internal RarReaderVolume(Stream stream, ReaderOptions options)
            : base(StreamingMode.Streaming, options)
        {
            this.stream = stream;
            this.streamOwner = !options.HasFlag(ReaderOptions.KeepStreamsOpen);
        }

        internal override Stream Stream
        {
            get { return stream; }
        }

        internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
        {
            return new NonSeekableStreamFilePart(markHeader, fileHeader, streamOwner);
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return GetVolumeFileParts();
        }
    }
}
