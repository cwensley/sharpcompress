using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Tar;
using SharpCompress.Common.Tar.Headers;

namespace SharpCompress.Reader.Tar
{
    public class TarReader : AbstractReader<TarEntry, TarVolume>
    {
        private readonly TarVolume volume;

        internal TarReader(Stream stream, Options options, IExtractionListener listener)
            : base(options, listener)
        {
            volume = new TarVolume(stream, options);
        }

        public override TarVolume Volume
        {
            get { return volume; }
        }

        public override ReaderType ReaderType
        {
            get { return ReaderType.Tar; }
        }

        #region Open

        /// <summary>
        /// Opens a TarReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TarReader Open(Stream stream, IExtractionListener listener,
                                     Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new TarReader(stream, options, listener);
        }

        /// <summary>
        /// Opens a TarReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TarReader Open(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return Open(stream, new NullExtractionListener(), options);
        }

        #endregion

        internal override IEnumerable<TarEntry> GetEntries(Stream stream, Options options)
        {
            return TarEntry.GetEntries(stream);
        }

        internal static bool IsTarFile(Stream stream)
        {
            try
            {
                TarHeader tar = new TarHeader();
                tar.Read(new BinaryReader(stream));
                return tar.Name.Length > 0;
            }
            catch
            {
            }
            return false;
        }
    }
}
