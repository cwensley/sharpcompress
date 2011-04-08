using NUnrar.IO;

namespace NUnrar.Zip
{
    public abstract class ZipHeader
    {
        internal abstract void Read(MarkingBinaryReader reader);
    }
}
