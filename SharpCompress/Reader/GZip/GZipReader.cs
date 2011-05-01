using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.GZip;

namespace SharpCompress.Reader.GZip
{
    public class GZipReader : AbstractReader<GZipEntry, GZipVolume>
    {
        private readonly GZipVolume volume;

        internal GZipReader(Stream stream, Options options, IExtractionListener listener)
            : base(options, listener)
        {
            volume = new GZipVolume(stream, options);
        }

        public override GZipVolume Volume
        {
            get { return volume; }
        }

        public override ReaderType ReaderType
        {
            get { return ReaderType.GZip; }
        }

        #region Open

        /// <summary>
        /// Opens a TarReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static GZipReader Open(Stream stream, IExtractionListener listener,
                                     Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new GZipReader(stream, options, listener);
        }

        /// <summary>
        /// Opens a TarReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static GZipReader Open(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return Open(stream, new NullExtractionListener(), options);
        }

        #endregion

        internal override IEnumerable<GZipEntry> GetEntries(Stream stream, Options options)
        {
            return GZipEntry.GetEntries(stream);
        }

        public static bool IsGZip(Stream stream)
        {
            // read the header on the first read
            byte[] header = new byte[10];
            int n = stream.Read(header, 0, header.Length);

            // workitem 8501: handle edge case (decompress empty stream)
            if (n == 0)
                return false;

            if (n != 10)
                return false;

            if (header[0] != 0x1F || header[1] != 0x8B || header[2] != 8)
                return false;

            return true;
        }
    }
}
