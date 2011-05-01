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
            if (index > 0)
            {
                Name = Name.Substring(0, index);
            }
            Mode = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 100, 7).Trim(), 8);
            UserId = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 108, 7).Trim(), 8);
            GroupId = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 116, 7).Trim(), 8);
            if ((buffer[124] & 0x80) == 0x80) // if size in binary
            {
                long sizeBigEndian = BitConverter.ToInt64(buffer, 0x80);
                Size = IPAddress.NetworkToHostOrder(sizeBigEndian);
            }
            else
            {
                Size = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 124, 11).Trim(), 8);
            }
            long unixTimeStamp = Convert.ToInt64(Encoding.ASCII.GetString(buffer, 136, 11).Trim());
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

            UserName = Encoding.ASCII.GetString(buffer, 0x109, 32).Trim();
            GroupName = Encoding.ASCII.GetString(buffer, 0x129, 32).Trim();
            NamePrefix = Encoding.ASCII.GetString(buffer, 347, 157).Trim();
            return true;
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
