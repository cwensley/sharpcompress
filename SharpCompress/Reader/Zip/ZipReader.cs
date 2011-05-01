using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;

namespace SharpCompress.Reader.Zip
{
    public class ZipReader : AbstractReader<ZipEntry, Volume>
    {
        private readonly ZipVolume volume;

        internal ZipReader(Stream stream, Options options, IExtractionListener listener)
            : base(options, listener)
        {
            volume = new ZipVolume(stream, options);
        }

        public override Volume Volume
        {
            get { return volume; }
        }

        public override ReaderType ReaderType
        {
            get { return ReaderType.Zip; }
        }

        #region Open

        /// <summary>
        /// Opens a ZipReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ZipReader Open(Stream stream, IExtractionListener listener,
                                     Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new ZipReader(stream, options, listener);
        }

        /// <summary>
        /// Opens a ZipReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static ZipReader Open(Stream stream, Options options = Options.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return Open(stream, new NullExtractionListener(), options);
        }

        #endregion

        internal override IEnumerable<ZipEntry> GetEntries(Stream stream, Options options)
        {
            foreach (ZipHeader h in ZipHeaderFactory.ReadHeaderNonseekable(stream))
            {
                if (h != null)
                {
                    switch (h.ZipHeaderType)
                    {
                        case ZipHeaderType.LocalEntry:
                            {
                                yield return new ZipEntry(new ZipFilePart(h as LocalEntryHeader));
                            }
                            break;
                        case ZipHeaderType.DirectoryEnd:
                            {
                                yield break;
                            }
                    }
                }
            }
        }
    }
}