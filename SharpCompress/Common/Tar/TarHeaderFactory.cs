using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Tar.Headers;
using SharpCompress.IO;

namespace SharpCompress.Common.Tar
{
    internal static class TarHeaderFactory
    {
        private const uint ENTRY_HEADER_BYTES = 0x04034b50;
        private const uint POST_DATA_DESCRIPTOR = 0x08074b50;
        private const uint DIRECTORY_START_HEADER_BYTES = 0x02014b50;
        private const uint DIRECTORY_END_HEADER_BYTES = 0x06054b50;
        private const uint DIGITAL_SIGNATURE = 0x05054b50;

        private const uint ZIP64_END_OF_CENTRAL_DIRECTORY = 0x07064b50;

        internal static IEnumerable<TarHeader> ReadHeaderNonseekable(Stream stream)
        {
            while (true)
            {
                TarHeader header = null;
                try
                {
                    BinaryReader reader = new BinaryReader(stream);
                    header = new TarHeader();
                    if (!header.Read(reader))
                    {
                        yield break;
                    }
                    header.PackedStream = new TarReadOnlySubStream(stream, header.Size);
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
