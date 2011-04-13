﻿using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Reader.Rar;
using SharpCompress.Reader.Zip;

#if THREEFIVE || PORTABLE
using SharpCompress.Common.Rar.Headers;
#endif

namespace SharpCompress.Reader
{
    /// <summary>
    /// A generic push reader that reads unseekable comrpessed streams.
    /// </summary>
    public abstract class CompressedStreamReader : IDisposable
    {
        private readonly ReaderOptions options;
        private IEnumerator<Entry> entriesForCurrentReadStream;
        private bool wroteCurrentEntry;
        private bool completed;

        internal CompressedStreamReader(ReaderOptions options, IExtractionListener listener)
        {
            Listener = listener;
            this.options = options;
            listener.CheckNotNull("listener");
        }

        public abstract ReaderType ReaderType { get; }

        protected IExtractionListener Listener
        {
            get;
            private set;
        }

        public void Dispose()
        {
            if (entriesForCurrentReadStream != null)
            {
                entriesForCurrentReadStream.Dispose();
            }
            if (Volume.Stream != null && !options.HasFlag(ReaderOptions.KeepStreamsOpen))
            {
                Volume.Stream.Dispose();
            }
        }

        /// <summary>
        /// Current volume that the current entry resides in
        /// </summary>
        public abstract Volume Volume
        {
            get;
        }

        /// <summary>
        /// Current file entry 
        /// </summary>
        public Entry Entry
        {
            get
            {
                return entriesForCurrentReadStream.Current;
            }
        }

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
                    + Entry.FilePath + "'. A new readable stream is required.  Use Cancel if it was intended.");
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

        internal abstract IEnumerable<Entry> GetEntries(Stream stream, ReaderOptions options);


        #region Open
        /// <summary>
        /// Opens a ZipReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CompressedStreamReader OpenStream(Stream stream, IExtractionListener listener,
            ReaderOptions options = ReaderOptions.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");

            RewindableStream rewindableStream = new RewindableStream(stream);
            rewindableStream.Recording = true;
            if (ZipArchive.IsZipFile(rewindableStream))
            {
                return ZipReader.Open(rewindableStream, listener, options);
            }
            rewindableStream.Rewind();
            rewindableStream.Recording = true;
            if (RarArchive.IsRarFile(rewindableStream))
            {
                rewindableStream.Rewind();
                return RarReader.Open(rewindableStream, listener, options);
            }
            throw new InvalidOperationException("Cannot determine compressed stream type.");
        }

        /// <summary>
        /// Opens a ZipReader for Non-seeking usage with a single volume
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CompressedStreamReader OpenStream(Stream stream, ReaderOptions options = ReaderOptions.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return OpenStream(stream, new NullExtractionListener(), options);
        }
        #endregion
    }
}
