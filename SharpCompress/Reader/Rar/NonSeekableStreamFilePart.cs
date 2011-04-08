using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Rar.Headers;
using SharpCompress.Rar;
using SharpCompress.Common.Rar;

namespace SharpCompress.Reader.Rar
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
