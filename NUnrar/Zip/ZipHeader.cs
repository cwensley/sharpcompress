using SharpCompress.IO;

namespace SharpCompress.Zip
{
    public abstract class ZipHeader
    {
        internal abstract void Read(MarkingBinaryReader reader);
    }
}
