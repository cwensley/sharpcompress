using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;

namespace SharpCompress.Reader.Zip
{
    public class ZipReader : CompressedStreamReader
    {
        private readonly Stream stream;

        internal ZipReader(Stream stream, ReaderOptions options, IExtractionListener listener)
            : base(options, listener)
        {
            this.stream = stream;
        }

        public override Volume Volume
        {
            get { throw new NotImplementedException(); }
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
            ReaderOptions options = ReaderOptions.KeepStreamsOpen)
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
        public static ZipReader Open(Stream stream, ReaderOptions options = ReaderOptions.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return Open(stream, new NullExtractionListener(), options);
        }
        #endregion

        internal override IEnumerable<FilePart> CreateFilePartEnumerableForCurrentEntry()
        {
            return base.Entry.Parts;
        }

        internal override Stream RequestInitialStream()
        {
            return stream;
        }

        internal override void Skip(IEnumerable<Common.FilePart> parts)
        {
            byte[] buffer = new byte[4096];
            using (Stream s = parts.First().GetStream())
            {
                while (s.Read(buffer, 0, buffer.Length) > 0)
                {
                }
            }
        }

        internal override void Write(IEnumerable<Common.FilePart> parts, Stream writeStream)
        {
            using (Stream s = parts.First().GetStream())
            {
                s.TransferTo(writeStream);
            }
        }

        internal override IEnumerable<Common.Entry> GetEntries(Stream stream, Common.ReaderOptions options)
        {
            foreach (var h in ZipHeaderFactory.ReadHeaderNonseekable(stream))
            {
                if (h != null)
                {
                    switch (h.ZipHeaderType)
                    {
                        case ZipHeaderType.LocalEntry:
                            {
                                yield return new ZipReaderEntry(new ZipFilePart(h as LocalEntryHeader));
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
