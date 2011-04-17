using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
#if PORTABLE || THREEFIVE
using SharpCompress.Common.Rar.Headers;
#endif
using SharpCompress.Compressor.Rar;
using SharpCompress.Common.Rar;

namespace SharpCompress.Reader.Rar
{
    /// <summary>
    /// This class faciliates Reading a Rar Archive in a non-seekable forward-only manner
    /// </summary>
    public abstract class RarReader : CompressedStreamReader<RarReaderEntry, RarVolume>
    {
        private RarVolume volume;

        internal RarReader(Options options, IExtractionListener listener)
            : base(options, listener)
        {
        }

        internal abstract void ValidateArchive(RarVolume archive);

        public override RarVolume Volume
        {
            get
            {
                return volume;
            }
        }

        public override ReaderType ReaderType
        {
            get
            {
                return ReaderType.Rar;
            }
        }

        #region Open
        /// <summary>
        /// Opens a RarReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static RarReader Open(Stream stream, IExtractionListener listener,
            Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new SingleVolumeRarReader(stream, options, listener);
        }

        /// <summary>
        /// Opens a RarReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static RarReader Open(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new SingleVolumeRarReader(stream, options, new NullExtractionListener());
        }

        /// <summary>
        /// Opens a RarReader for Non-seeking usage with multiple volumes
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static RarReader Open(IEnumerable<Stream> streams, IExtractionListener listener,
            Options options = Options.KeepStreamsOpen)
        {
            streams.CheckNotNull("streams");
            return new MultiVolumeRarReader(streams, options, listener);
        }

        /// <summary>
        /// Opens a RarReader for Non-seeking usage with multiple volumes
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static RarReader Open(IEnumerable<Stream> streams, Options options = Options.KeepStreamsOpen)
        {
            streams.CheckNotNull("streams");
            return new MultiVolumeRarReader(streams, options, new NullExtractionListener());
        }
        #endregion

        internal override IEnumerable<RarReaderEntry> GetEntries(Stream stream, Options options)
        {
            volume = new RarReaderVolume(stream, options);
            foreach (RarFilePart fp in volume.ReadFileParts())
            {
                ValidateArchive(volume);
                yield return new RarReaderEntry(volume.IsSolidArchive, fp);
            }
        }


        internal override void Skip(IEnumerable<FilePart> parts)
        {
            byte[] buffer = new byte[4096];
            using (Stream s = new MultiVolumeReadOnlyStream(parts.Cast<RarFilePart>(), Listener))
            {
                while (s.Read(buffer, 0, buffer.Length) > 0)
                {
                }
            }
        }

        internal override void Write(IEnumerable<FilePart> parts, Stream writeStream)
        {
            using (Stream input = new MultiVolumeReadOnlyStream(parts.Cast<RarFilePart>(), Listener))
            {
                RarReaderEntry entry = Entry as RarReaderEntry;
                Unpack pack = new Unpack(entry.FileHeader, input, writeStream);
                pack.doUnpack(entry.IsSolid);
            }
        }
    }
}
