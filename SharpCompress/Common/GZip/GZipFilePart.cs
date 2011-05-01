using System;
using System.IO;
using SharpCompress.Compressor.Deflate;
using SharpCompress.IO;

namespace SharpCompress.Common.GZip
{
    public class GZipFilePart : FilePart
    {
        private string name;
        private Stream stream;

        internal GZipFilePart(Stream stream)
        {
            RewindableStream rewind = new RewindableStream(stream);
            rewind.Recording = true;
            ReadAndValidateGzipHeader(rewind);
            rewind.Rewind();
            this.stream = new GZipStream(rewind, CompressionMode.Decompress);
        }

        internal DateTime? DateModified { get; private set; }

        internal override string FilePartName
        {
            get { return name; }
        }

        internal override Stream GetStream()
        {
            return stream;
        }


        private void ReadAndValidateGzipHeader(Stream stream)
        {
            int totalBytesRead = 0;
            // read the header on the first read
            byte[] header = new byte[10];
            int n = stream.Read(header, 0, header.Length);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
                return;

            if (n != 10)
                throw new ZlibException("Not a valid GZIP stream.");

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                throw new ZlibException("Bad GZIP header.");

            Int32 timet = BitConverter.ToInt32(header, 4);
            DateModified = GZipStream._unixEpoch.AddSeconds(timet);
            totalBytesRead += n;
            if ((header[3] & 0x04) == 0x04)
            {
                // read and discard extra field
                n = stream.Read(header, 0, 2); // 2-byte length field
                totalBytesRead += n;

                Int16 extraLength = (Int16)(header[0] + header[1] * 256);
                byte[] extra = new byte[extraLength];
                n = stream.Read(extra, 0, extra.Length);
                if (n != extraLength)
                    throw new ZlibException("Unexpected end-of-file reading GZIP header.");
                totalBytesRead += n;
            }
            if ((header[3] & 0x08) == 0x08)
                name = ReadZeroTerminatedString(stream);
            if ((header[3] & 0x10) == 0x010)
                ReadZeroTerminatedString(stream);
            if ((header[3] & 0x02) == 0x02)
                stream.ReadByte(); // CRC16, ignore
        }


        private string ReadZeroTerminatedString(Stream stream)
        {
            byte[] buf1 = new byte[1];
            var list = new System.Collections.Generic.List<byte>();
            bool done = false;
            do
            {
                // workitem 7740
                int n = stream.Read(buf1, 0, 1);
                if (n != 1)
                    throw new ZlibException("Unexpected EOF reading GZIP header.");
                else
                {
                    if (buf1[0] == 0)
                        done = true;
                    else
                        list.Add(buf1[0]);
                }
            } while (!done);
            byte[] a = list.ToArray();
            return GZipStream.iso8859dash1.GetString(a, 0, a.Length);
        }
    }
}
