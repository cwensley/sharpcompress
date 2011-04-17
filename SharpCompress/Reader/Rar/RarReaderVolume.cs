using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.IO;

namespace SharpCompress.Reader.Rar
{
    public class RarReaderVolume : RarVolume
    {
        private bool streamOwner;
        private Stream stream;

        internal RarReaderVolume(Stream stream, Options options)
            : base(StreamingMode.Streaming, options)
        {
            this.stream = stream;
            this.streamOwner = !options.HasFlag(Options.KeepStreamsOpen);
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
