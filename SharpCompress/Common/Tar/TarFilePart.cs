using System.IO;
using SharpCompress.Common.Tar.Headers;

namespace SharpCompress.Common.Tar
{
    public class TarFilePart : FilePart
    {
        internal TarFilePart(TarHeader header)
        {
            this.Header = header;
        }
        internal TarHeader Header { get; private set; }

        internal override string FilePartName
        {
            get { return Header.Name; }
        }

        internal override Stream GetStream()
        {
            return Header.PackedStream;
        }
    }
}
