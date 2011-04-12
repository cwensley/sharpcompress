using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    public abstract class ZipHeader
    {
        protected ZipHeader(ZipHeaderType type)
        {
            ZipHeaderType = type;
        }

        internal ZipHeaderType ZipHeaderType { get; private set; }

        internal abstract void Read(MarkingBinaryReader reader);
    }

    public enum ZipHeaderType
    {
        LocalEntry,
        DirectoryEntry,
        DirectoryEnd,
    }
}
