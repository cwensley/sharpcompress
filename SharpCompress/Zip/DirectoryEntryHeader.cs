using System.IO;
using SharpCompress.IO;

namespace SharpCompress.Zip
{
    public class DirectoryEntryHeader : ZipHeader
    {
        internal override void Read(MarkingBinaryReader reader)
        {
            Version = reader.ReadUInt16();
            VersionNeededToExtract = reader.ReadUInt16();
            Flags = reader.ReadUInt16();
            CompressionMethod = reader.ReadUInt16();
            LastModifiedTime = reader.ReadUInt16();
            LastModifiedDate = reader.ReadUInt16();
            Crc = reader.ReadUInt32();
            CompressedSize = reader.ReadUInt32();
            UncompressedSize = reader.ReadUInt32();
            ushort nameLength = reader.ReadUInt16();
            ushort extraLength = reader.ReadUInt16();
            ushort commentLength = reader.ReadUInt16();
            DiskNumberStart = reader.ReadUInt16();
            InternalFileAttributes = reader.ReadUInt16();
            ExternalFileAttributes = reader.ReadUInt32();
            RelativeOffsetOfEntryHeader = reader.ReadUInt32();

            byte[] name = reader.ReadBytes(nameLength);
            byte[] extra = reader.ReadBytes(extraLength);
            byte[] commment = reader.ReadBytes(commentLength);
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

        public ushort VersionNeededToExtract { get; set; }

        public uint RelativeOffsetOfEntryHeader { get; set; }

        public uint ExternalFileAttributes { get; set; }

        public ushort InternalFileAttributes { get; set; }

        public ushort DiskNumberStart { get; set; }
    }
}
