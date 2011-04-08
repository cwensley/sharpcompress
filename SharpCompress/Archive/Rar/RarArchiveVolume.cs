using System.IO;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Rar;
using SharpCompress.Common.Rar;

namespace SharpCompress.Archive.Rar
{
    public abstract class RarArchiveVolume : RarVolume
    {
        internal RarArchiveVolume(StreamingMode mode, ReaderOptions options)
            : base(mode, options)
        {
        }

#if !PORTABLE
        /// <summary>
        /// File that backs this volume, if it not stream based
        /// </summary>
        public abstract FileInfo VolumeFile
        {
            get;
        }
#endif
    }
}
