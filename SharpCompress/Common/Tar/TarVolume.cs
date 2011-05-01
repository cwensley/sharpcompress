﻿using System.IO;

namespace SharpCompress.Common.Tar
{
    public class TarVolume : Volume
    {
        private Stream stream;
#if !PORTABLE
        private FileInfo fileInfo;
#endif

        public TarVolume(Stream stream, Options options)
        {
            this.stream = stream;
            Options = options;
        }

#if !PORTABLE
        public TarVolume(FileInfo fileInfo, Options options)
        {
            this.fileInfo = fileInfo;
            this.stream = fileInfo.OpenRead();
            Options = options;
        }
#endif

        internal override Stream Stream
        {
            get
            {
                return stream;
            }
        }

        internal Options Options
        {
            get;
            private set;
        }

#if !PORTABLE
        /// <summary>
        /// File that backs this volume, if it not stream based
        /// </summary>
        public override FileInfo VolumeFile
        {
            get
            {
                return fileInfo;
            }
        }
#endif



        public override bool IsFirstVolume
        {
            get
            {
                return true;
            }
        }

        public override bool IsMultiVolume
        {
            get
            {
                return true;
            }
        }
    }
}
