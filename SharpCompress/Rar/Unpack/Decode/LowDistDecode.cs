namespace SharpCompress.Rar.Unpack.decode
{
    internal class LowDistDecode : Decode
    {
        internal LowDistDecode()
            : base(new int[Compress.LDC])
        {
        }
    }
}