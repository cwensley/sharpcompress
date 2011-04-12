using System.IO;
using System.Text;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    public class LocalEntryHeader : ZipHeader
    {
        private readonly static Encoding DefaultEncoding = Encoding.GetEncoding("IBM437");

        public LocalEntryHeader()
            : base(ZipHeaderType.LocalEntry)
        {
        }

        internal override void Read(MarkingBinaryReader reader)
        {
            Version = reader.ReadUInt16();
            Flags = reader.ReadUInt16();
            CompressionMethod = reader.ReadUInt16();
            LastModifiedTime = reader.ReadUInt16();
            LastModifiedDate = reader.ReadUInt16();
            Crc = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            UncompressedSize = reader.ReadUInt32();
            ushort nameLength = reader.ReadUInt16();
            ushort extraLength = reader.ReadUInt16();
            byte[] name = reader.ReadBytes(nameLength);
            Extra = reader.ReadBytes(extraLength);
            Name = DefaultEncoding.GetString(name, 0, name.Length);
        }

        internal ushort Version { get; private set; }

        public ushort Flags { get; set; }

        public ushort CompressionMethod { get; set; }

        public ushort LastModifiedTime { get; set; }

        public ushort LastModifiedDate { get; set; }

        public uint Crc { get; set; }

        public uint CompressedSize { get; set; }

        public uint UncompressedSize { get; set; }

        public string Name { get; private set; }

        internal Stream PackedStream { get; set; }

        public byte[] Extra { get; set; }
    }
}
