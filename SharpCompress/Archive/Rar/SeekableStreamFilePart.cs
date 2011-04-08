using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Rar;
using SharpCompress.Common.Rar;

namespace SharpCompress.Archive.Rar
{
    internal class SeekableStreamFilePart : RarFilePart
    {
        internal SeekableStreamFilePart(MarkHeader mh, FileHeader fh, Stream stream, bool streamOwner)
            : base(mh, fh, streamOwner)
        {
            Stream = stream;
        }

        internal Stream Stream
        {
            get;
            private set;
        }

        internal override Stream GetStream()
        {
            Stream.Position = FileHeader.DataStartPosition;
            return Stream;
        }

        internal override string FilePartName
        {
            get
            {
                return "Unknown Stream - File Entry: " + base.FileHeader.FileName;
            }
        }
    }
}
