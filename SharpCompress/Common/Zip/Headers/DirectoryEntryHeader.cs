using System.IO;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    internal class DirectoryEntryHeader : ZipHeader
    {
        public DirectoryEntryHeader()
            : base(ZipHeaderType.DirectoryEntry)
        {
        }

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

            Name = DefaultEncoding.GetString(reader.ReadBytes(nameLength));
            Extra = reader.ReadBytes(extraLength);
            Comment = reader.ReadBytes(commentLength);
        }

        internal override void Write(BinaryWriter writer)
        {
            writer.Write(Version);
            writer.Write(VersionNeededToExtract);
            writer.Write(Flags);
            writer.Write(CompressionMethod);
            writer.Write(LastModifiedTime);
            writer.Write(LastModifiedDate);
            writer.Write(Crc);
            writer.Write(CompressedSize);
            writer.Write(UncompressedSize);

            byte[] nameBytes = DefaultEncoding.GetBytes(Name);
            writer.Write((ushort)nameBytes.Length);
            writer.Write((ushort)Extra.Length);
            writer.Write((ushort)Comment.Length);

            writer.Write(DiskNumberStart);
            writer.Write(InternalFileAttributes);
            writer.Write(ExternalFileAttributes);
            writer.Write(RelativeOffsetOfEntryHeader);

            writer.Write(nameBytes);
            writer.Write(Extra);
            writer.Write(Comment);
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

        public byte[] Extra { get; private set; }

        public byte[] Comment { get; private set; }
    }
}
