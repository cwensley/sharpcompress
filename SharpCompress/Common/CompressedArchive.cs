using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SharpCompress.Common
{
    public abstract class CompressedArchive<TEntry, TVolume>
        where TEntry : Entry
        where TVolume : Volume
    {
        private LazyReadOnlyCollection<TVolume> lazyVolumes;
        private LazyReadOnlyCollection<TEntry> lazyEntries;


#if !PORTABLE
        /// <summary>
        /// Constructor with a FileInfo object to an existing file.
        /// </summary>
        /// <param name="fileInfo"></param>
        /// <param name="options"></param>
        internal CompressedArchive(FileInfo fileInfo, Options options)
        {
            if (!fileInfo.Exists)
            {
                throw new ArgumentException("File does not exist: " + fileInfo.FullName);
            }

            lazyVolumes = new LazyReadOnlyCollection<TVolume>(LoadVolumes(fileInfo, options));
            lazyEntries = new LazyReadOnlyCollection<TEntry>(LoadEntries(Volumes));
        }


        protected abstract IEnumerable<TVolume> LoadVolumes(FileInfo file, Options options);
#endif

        /// <summary>
        /// Takes multiple seekable Streams for a multi-part archive
        /// </summary>
        /// <param name="streams"></param>
        /// <param name="options"></param>
        internal CompressedArchive(IEnumerable<Stream> streams, Options options)
        {
            lazyVolumes = new LazyReadOnlyCollection<TVolume>(LoadVolumes(streams, options));
            lazyEntries = new LazyReadOnlyCollection<TEntry>(LoadEntries(Volumes));
        }

        /// <summary>
        /// Returns an ReadOnlyCollection of all the RarArchiveEntries across the one or many parts of the RarArchive.
        /// </summary>
        /// <returns></returns>
        public ICollection<TEntry> Entries
        {
            get
            {
                return lazyEntries;
            }
        }

        /// <summary>
        /// Returns an ReadOnlyCollection of all the RarArchiveVolumes across the one or many parts of the RarArchive.
        /// </summary>
        /// <returns></returns>
        public ICollection<TVolume> Volumes
        {
            get
            {
                return lazyVolumes;
            }
        }

        /// <summary>
        /// The total size of the files compressed in the archive.
        /// </summary>
        public long TotalSize
        {
            get
            {
                return Entries.Aggregate(0L, (total, cf) => total + cf.CompressedSize);
            }
        }

        protected abstract IEnumerable<TVolume> LoadVolumes(IEnumerable<Stream> streams, Options options);
        protected abstract IEnumerable<TEntry> LoadEntries(IEnumerable<TVolume> volumes);
    }
}
