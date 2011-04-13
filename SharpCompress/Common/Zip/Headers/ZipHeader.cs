using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    internal abstract class ZipHeader
    {
        protected ZipHeader(ZipHeaderType type)
        {
            ZipHeaderType = type;
        }

        internal ZipHeaderType ZipHeaderType { get; private set; }

        internal abstract void Read(MarkingBinaryReader reader);
    }

    internal enum ZipHeaderType
    {
        LocalEntry,
        DirectoryEntry,
        DirectoryEnd,
    }
}
