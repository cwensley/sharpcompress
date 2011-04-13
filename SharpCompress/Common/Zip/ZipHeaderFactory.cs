using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Zip.Headers;
using SharpCompress.IO;

namespace SharpCompress.Common.Zip
{
    internal static class ZipHeaderFactory
    {
        private const uint ENTRY_HEADER_BYTES = 0x04034b50;
        private const uint POST_DATA_DESCRIPTOR = 0x08074b50;
        private const uint DIRECTORY_START_HEADER_BYTES = 0x02014b50;
        private const uint DIRECTORY_END_HEADER_BYTES = 0x06054b50;
        private const uint DIGITAL_SIGNATURE = 0x05054b50;

        private const uint ZIP64_END_OF_CENTRAL_DIRECTORY = 0x07064b50;

        internal static IEnumerable<ZipHeader> ReadHeaderNonseekable(Stream stream)
        {
            while (true)
            {
                ZipHeader header = null;
                try
                {
                    MarkingBinaryReader reader = new MarkingBinaryReader(stream);

                    uint headerBytes = reader.ReadUInt32();
                    switch (headerBytes)
                    {
                        case ENTRY_HEADER_BYTES:
                            {
                                var entry = new LocalEntryHeader();
                                entry.Read(reader);
                                if (entry.CompressedSize > 0)
                                {
                                    entry.PackedStream = new ReadOnlySubStream(stream, entry.CompressedSize, true);
                                }
                                header = entry;
                            }
                            break;
                        case DIRECTORY_START_HEADER_BYTES:
                            {
                                var entry = new DirectoryEntryHeader();
                                entry.Read(reader);
                                header = entry;
                            }
                            break;
                        case POST_DATA_DESCRIPTOR:
                        case DIGITAL_SIGNATURE:
                            break;
                        case DIRECTORY_END_HEADER_BYTES:
                            {
                                var entry = new DirectoryEndHeader();
                                entry.Read(reader);
                                header = entry;
                            }
                            break;
                        case ZIP64_END_OF_CENTRAL_DIRECTORY:
                        default:
                            break;
                    }
                }
                catch
                {
                    header = null;
                }
                yield return header;
            }

        }
    }
}
