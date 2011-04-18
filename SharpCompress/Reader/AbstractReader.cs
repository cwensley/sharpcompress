using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common;
#if THREEFIVE || PORTABLE
using SharpCompress.Common.Rar.Headers;
#endif

namespace SharpCompress.Reader
{
    /// <summary>
    /// A generic push reader that reads unseekable comrpessed streams.
    /// </summary>
    public abstract class AbstractReader<TEntry, TVolume> : IDisposable, IReader
        where TEntry : Entry
        where TVolume : Volume
    {
        private readonly Options options;
        private bool completed;
        private IEnumerator<TEntry> entriesForCurrentReadStream;
        private bool wroteCurrentEntry;

        internal AbstractReader(Options options, IExtractionListener listener)
        {
            Listener = listener;
            this.options = options;
            listener.CheckNotNull("listener");
        }

        public abstract ReaderType ReaderType
        {
            get;
        }

        protected IExtractionListener Listener
        {
            get;
            private set;
        }

        /// <summary>
        /// Current volume that the current entry resides in
        /// </summary>
        public abstract TVolume Volume
        {
            get;
        }

        /// <summary>
        /// Current file entry 
        /// </summary>
        public TEntry Entry
        {
            get
            {
                return entriesForCurrentReadStream.Current;
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (entriesForCurrentReadStream != null)
            {
                entriesForCurrentReadStream.Dispose();
            }
            if (Volume.Stream != null && !options.HasFlag(Options.KeepStreamsOpen))
            {
                Volume.Stream.Dispose();
            }
        }

        #endregion

        /// <summary>
        /// Moves to the next entry by reading more data from the underlying stream.  This skips if data has not been read.
        /// </summary>
        /// <returns></returns>
        public bool MoveToNextEntry()
        {
            if (completed)
            {
                return false;
            }
            if (entriesForCurrentReadStream == null)
            {
                LoadStreamForReading(RequestInitialStream());
                return true;
            }
            if (!wroteCurrentEntry)
            {
                SkipEntry();
            }
            wroteCurrentEntry = false;
            if (NextEntryForCurrentStream())
            {
                return true;
            }
            completed = true;
            return false;
        }

        internal bool LoadStreamForReading(Stream stream)
        {
            if (entriesForCurrentReadStream != null)
            {
                entriesForCurrentReadStream.Dispose();
            }
            if ((stream == null) || (!stream.CanRead))
            {
                throw new MultipartStreamRequiredException("File is split into multiple archives: '"
                                                           + Entry.FilePath +
                                                           "'. A new readable stream is required.  Use Cancel if it was intended.");
            }
            entriesForCurrentReadStream = GetEntries(stream, options).GetEnumerator();
            if (!entriesForCurrentReadStream.MoveNext())
            {
                return false;
            }
            return true;
        }

        internal abstract IEnumerable<FilePart> CreateFilePartEnumerableForCurrentEntry();
        internal abstract Stream RequestInitialStream();

        internal virtual bool NextEntryForCurrentStream()
        {
            return entriesForCurrentReadStream.MoveNext();
        }

        internal abstract IEnumerable<TEntry> GetEntries(Stream stream, Options options);

        #region Entry Skip/Write

        private void SkipEntry()
        {
            Listener.OnInformation("Skipping Entry");
            if (!Entry.IsDirectory)
            {
                Skip(CreateFilePartEnumerableForCurrentEntry());
            }
        }

        internal abstract void Skip(IEnumerable<FilePart> parts);

        /// <summary>
        /// Decompresses the current entry to the stream.  This cannot be called twice for the current entry.
        /// </summary>
        /// <param name="writableStream"></param>
        public void WriteEntryTo(Stream writableStream)
        {
            if ((writableStream == null) || (!writableStream.CanWrite))
            {
                throw new ArgumentNullException(
                    "A writable Stream was required.  Use Cancel if that was intended.");
            }
            Listener.OnInformation("Writing Entry to Stream");
            Write(CreateFilePartEnumerableForCurrentEntry(), writableStream);
            wroteCurrentEntry = true;
        }

        internal abstract void Write(IEnumerable<FilePart> parts, Stream writeStream);

        #endregion

        IEntry IReader.Entry
        {
            get
            {
                return Entry;
            }
        }
    }
}