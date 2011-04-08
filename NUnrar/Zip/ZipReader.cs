using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;

namespace SharpCompress.Zip
{
    public class ZipReader : SharpCompress.Common.CompressedStreamReader
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

        public static ZipReader Open(Stream stream, IExtractionListener listener,
           ReaderOptions options = ReaderOptions.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return new ZipReader(stream, options, listener);
        }

        internal override IEnumerable<Common.FilePart> CreateFilePartEnumerableForCurrentEntry()
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
                s.CopyTo(writeStream);
            }
        }

        internal override IEnumerable<Common.Entry> GetEntries(Stream stream, Common.ReaderOptions options)
        {
            foreach (var h in ZipHeaderFactory.ReadHeaderNonseekable(stream))
            {
                if (h != null)
                {
                    if (h is LocalEntryHeader)
                    {
                        yield return new ZipReaderEntry(new ZipFilePart(h as LocalEntryHeader));
                    }
                }
            }
        }
    }
}
