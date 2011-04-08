using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    public abstract class ZipHeader
    {
        internal abstract void Read(MarkingBinaryReader reader);
    }
}
