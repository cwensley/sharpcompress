using System.IO;
using System.IO.Compression;
using NUnrar.Common;

namespace NUnrar.Zip
{
    public class ZipFilePart : FilePart
    {
        internal ZipFilePart(LocalEntryHeader header)
        {
            this.Header = header;
        }
        internal LocalEntryHeader Header { get; private set; }

        internal override string FilePartName
        {
            get { return Header.Name; }
        }

        internal override Stream GetStream()
        {
            if (Header.CompressionMethod == 8)
            {
                return new DeflateStream(Header.PackedStream, CompressionMode.Decompress, true);
            }
            return Header.PackedStream;
        }
    }
}
