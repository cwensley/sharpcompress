using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using SharpCompress.Common;
using SharpCompress.Headers;
using SharpCompress.IO;
using SharpCompress.Rar;

namespace SharpCompress.Archive
{
    /// <summary>
    /// A rar part based on a FileInfo object
    /// </summary>
    internal class FileInfoRarArchiveVolume : RarArchiveVolume
    {
        internal FileInfoRarArchiveVolume(FileInfo fileInfo, ReaderOptions options)
            : base(StreamingMode.Seekable, FixOptions(options))
        {
            FileInfo = fileInfo;
            FileParts = base.GetVolumeFileParts().ToReadOnly();
        }

        private static ReaderOptions FixOptions(ReaderOptions options)
        {
            //make sure we're closing streams with fileinfo
            if (options.HasFlag(ReaderOptions.KeepStreamsOpen))
            {
                options = (ReaderOptions)FlagUtility.SetFlag(options, ReaderOptions.KeepStreamsOpen, false);
            }
            return options;
        }

        internal ReadOnlyCollection<RarFilePart> FileParts
        {
            get;
            private set;
        }

        internal FileInfo FileInfo
        {
            get;
            private set;
        }

        internal override RarFilePart CreateFilePart(FileHeader fileHeader, MarkHeader markHeader)
        {
            return new FileInfoRarFilePart(markHeader, fileHeader, FileInfo);
        }

        internal override Stream Stream
        {
            get
            {
                return FileInfo.OpenRead();
            }
        }

        internal override IEnumerable<RarFilePart> ReadFileParts()
        {
            return FileParts;
        }

        public override FileInfo VolumeFile
        {
            get
            {
                return FileInfo;
            }
        }
    }
}
