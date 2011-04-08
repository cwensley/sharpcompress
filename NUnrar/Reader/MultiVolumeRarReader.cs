using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Rar;

namespace SharpCompress.Reader
{
    internal class MultiVolumeRarReader : RarReader
    {
        private readonly IEnumerator<Stream> streams;

        internal MultiVolumeRarReader(IEnumerable<Stream> streams, ReaderOptions options, IExtractionListener listener)
            : base(options, listener)
        {
            this.streams = streams.GetEnumerator();
        }

        internal override void ValidateArchive(RarVolume archive)
        {
        }

        internal override Stream RequestInitialStream()
        {
            if (streams.MoveNext())
            {
                return streams.Current;
            }
            throw new RarExtractionException("No stream provided when requested by MultiVolumeRarReader");
        }

        internal override IEnumerable<FilePart> CreateFilePartEnumerableForCurrentEntry()
        {
            return new MultiVolumeStreamEnumerator(this, streams);
        }

        private class MultiVolumeStreamEnumerator : IEnumerable<FilePart>, IEnumerator<FilePart>
        {
            private readonly MultiVolumeRarReader reader;
            private readonly IEnumerator<Stream> nextReadableStreams;
            private bool isFirst = true;

            internal MultiVolumeStreamEnumerator(MultiVolumeRarReader r, IEnumerator<Stream> nextReadableStreams)
            {
                reader = r;
                this.nextReadableStreams = nextReadableStreams;
            }

            public IEnumerator<FilePart> GetEnumerator()
            {
                return this;
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                throw new NotImplementedException();
            }

            public FilePart Current
            {
                get;
                private set;
            }

            public void Dispose()
            {
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public bool MoveNext()
            {
                if (isFirst)
                {
                    Current = reader.Entry.Parts.First();
                    isFirst = false; //first stream already to go
                    return true;
                }

                if (!reader.Entry.IsSplit)
                {
                    return false;
                }
                if (!nextReadableStreams.MoveNext())
                {
                    throw new RarExtractionException("No stream provided when requested by MultiVolumeRarReader");
                }
                reader.LoadStreamForReading(nextReadableStreams.Current);

                Current = reader.Entry.Parts.First();
                return true;
            }

            public void Reset()
            {
            }
        }
    }
}
