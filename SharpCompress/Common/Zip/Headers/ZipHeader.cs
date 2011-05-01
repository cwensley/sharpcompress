using System.IO;
using System.Text;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    internal abstract class ZipHeader
    {
        protected readonly static Encoding DefaultEncoding = Encoding.GetEncoding("IBM437");

        protected ZipHeader(ZipHeaderType type)
        {
            ZipHeaderType = type;
        }

        internal ZipHeaderType ZipHeaderType
        {
            get;
            private set;
        }

        internal abstract void Read(MarkingBinaryReader reader);

        internal abstract void Write(BinaryWriter writer);
    }

    internal enum ZipHeaderType
    {
        LocalEntry,
        DirectoryEntry,
        DirectoryEnd,
    }
}
