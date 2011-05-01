using System;
using System.IO;
using System.Net;
using System.Text;

namespace SharpCompress.Common.Tar.Headers
{
    internal class TarHeader
    {
        protected readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0);

        internal string Name { get; set; }
        internal int Mode { get; set; }
        internal int UserId { get; set; }
        internal string UserName { get; set; }
        internal int GroupId { get; set; }
        internal string GroupName { get; set; }
        internal long Size { get; set; }
        internal DateTime LastModifiedTime { get; set; }
        internal string NamePrefix { get; set; }
        internal int FileType { get; set; }
        internal Stream PackedStream { get; set; }

        internal bool Read(BinaryReader reader)
        {
            byte[] buffer = reader.ReadBytes(512);
            if (buffer.Length == 0)
            {
                return false;
            }
            else if (buffer.Length < 512)
            {
                throw new InvalidOperationException();
            }
            Name = Encoding.ASCII.GetString(buffer, 0, 100);
            int index = Name.IndexOf('\0');
            if (index >= 0)
            {
                Name = Name.Substring(0, index);
            }
            if (Name.Length == 0)
            {
                return false;
            }
            Mode = ReadASCIIInt32(buffer, 100, 7);
            UserId = ReadASCIIInt32(buffer, 108, 7);
            GroupId = ReadASCIIInt32(buffer, 116, 7);
            FileType = ReadASCIIInt32(buffer, 156, 1);
            if (FileType != 5)
            {
                if ((buffer[124] & 0x80) == 0x80) // if size in binary
                {
                    long sizeBigEndian = BitConverter.ToInt64(buffer, 0x80);
                    Size = IPAddress.NetworkToHostOrder(sizeBigEndian);
                }
                else
                {
                    Size = ReadASCIIInt64(buffer, 124, 11);
                }
            }
            long unixTimeStamp = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 136, 11));
            LastModifiedTime = Epoch.AddSeconds(unixTimeStamp);

            //int storedChecksum = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 148, 6).Trim());
            //int headerChecksum = RecalculateChecksum(buffer);
            //if (storedChecksum != headerChecksum)
            //{
            //    headerChecksum = RecalculateAltChecksum(buffer);
            //    if (storedChecksum != headerChecksum)
            //    {
            //        throw new ArgumentException();
            //    }
            //}


            FileType = ReadASCIIInt32(buffer, 156, 1);

            UserName = Encoding.ASCII.GetString(buffer, 0x109, 32).TrimNulls();
            GroupName = Encoding.ASCII.GetString(buffer, 0x129, 32).TrimNulls();
            NamePrefix = Encoding.ASCII.GetString(buffer, 347, 157).TrimNulls();
            return true;
        }

        private static int ReadASCIIInt32(byte[] buffer, int offset, int count)
        {
            string s = Encoding.ASCII.GetString(buffer, offset, count).TrimNulls();
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return Convert.ToInt32(s, 8);
        }

        private static long ReadASCIIInt64(byte[] buffer, int offset, int count)
        {
            string s = Encoding.ASCII.GetString(buffer, offset, count).TrimNulls();
            if (string.IsNullOrEmpty(s))
            {
                return 0;
            }
            return Convert.ToInt64(s, 8);
        }

        internal static int RecalculateChecksum(byte[] buf)
        {
            // Set default value for checksum. That is 8 spaces.
            Encoding.ASCII.GetBytes("        ").CopyTo(buf, 148);

            // Calculate checksum
            int headerChecksum = 0;
            foreach (byte b in buf)
            {
                headerChecksum += b;
            }
            return headerChecksum;
        }

        internal static int RecalculateAltChecksum(byte[] buf)
        {
            Encoding.ASCII.GetBytes("        ").CopyTo(buf, 148);
            int headerChecksum = 0;
            foreach (byte b in buf)
            {
                if ((b & 0x80) == 0x80)
                {
                    headerChecksum -= b ^ 0x80;
                }
                else
                {
                    headerChecksum += b;
                }
            }
            return headerChecksum;
        }
    }
}
