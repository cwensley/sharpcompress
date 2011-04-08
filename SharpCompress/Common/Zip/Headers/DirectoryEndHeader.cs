using SharpCompress.IO;

namespace SharpCompress.Common.Zip.Headers
{
    public class DirectoryEndHeader : ZipHeader
    {
        internal override void Read(MarkingBinaryReader reader)
        {
            VolumeNumber = reader.ReadUInt16();
            FirstVolumeWithDirectory = reader.ReadUInt16();
            TotalNumberOfEntriesInDisk = reader.ReadUInt16();
            TotalNumberOfEntries = reader.ReadUInt16();
            DirectorySize = reader.ReadUInt32();
            DirectoryStartOffsetRelativeToDisk = reader.ReadUInt32();
            CommentLength = reader.ReadUInt16();
            Comment = reader.ReadBytes(CommentLength);
        }

        public ushort VolumeNumber { get; private set; }

        public ushort FirstVolumeWithDirectory { get; private set; }

        public ushort TotalNumberOfEntriesInDisk { get; private set; }

        public uint DirectorySize { get; private set; }

        public uint DirectoryStartOffsetRelativeToDisk { get; private set; }

        public ushort CommentLength { get; private set; }

        public byte[] Comment { get; private set; }

        public ushort TotalNumberOfEntries { get; private set; }
    }
}
