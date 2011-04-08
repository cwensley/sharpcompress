using System.IO;
using SharpCompress.Common;
using SharpCompress.Headers;
using SharpCompress.Rar;

namespace SharpCompress.Reader
{
    internal class NonSeekableStreamFilePart : RarFilePart
    {
        internal NonSeekableStreamFilePart(MarkHeader mh, FileHeader fh, bool streamOwner)
            : base(mh, fh, streamOwner)
        {
        }

        internal override Stream GetStream()
        {
            return FileHeader.PackedStream;
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
