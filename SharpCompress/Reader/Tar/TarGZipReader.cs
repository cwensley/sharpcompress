using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Tar;
using SharpCompress.Common.Tar.Headers;
using SharpCompress.Compressor.Deflate;

namespace SharpCompress.Reader.Tar
{
    public class TarGZipReader : AbstractReader<TarEntry, TarVolume>
    {
        private readonly TarVolume volume;

        internal TarGZipReader(Stream stream, Options options, IExtractionListener listener)
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
            get { return ReaderType.TarGZip; }
        }

        #region Open

        /// <summary>
        /// Opens a TarGZipReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TarGZipReader Open(Stream stream, IExtractionListener listener,
                                     Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new TarGZipReader(new GZipStream(stream, CompressionMode.Decompress), options, listener);
        }

        /// <summary>
        /// Opens a TarGZipReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static TarGZipReader Open(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return Open(stream, new NullExtractionListener(), options);
        }

        #endregion

        internal override IEnumerable<TarEntry> GetEntries(Stream stream, Options options)
        {
            foreach (TarHeader h in TarHeaderFactory.ReadHeaderNonseekable(stream))
            {
                if (h != null)
                {
                    yield return new TarEntry(new TarFilePart(h));
                }
            }
        }
    }
}
