using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Common.Tar.Headers;

namespace SharpCompress.Common.Tar
{
    public class TarEntry : Entry
    {
        private readonly TarFilePart filePart;

        internal TarEntry(TarFilePart filePart)
        {
            this.filePart = filePart;
        }

        public override uint Crc
        {
            get { return 0; }
        }

        public override string FilePath
        {
            get { return filePart.Header.Name; }
        }

        public override long CompressedSize
        {
            get { return filePart.Header.Size; }
        }

        public override long Size
        {
            get { return filePart.Header.Size; }
        }

        public override DateTime? LastModifiedTime
        {
            get { return filePart.Header.LastModifiedTime; }
        }

        public override DateTime? CreatedTime
        {
            get { return null; }
        }

        public override DateTime? LastAccessedTime
        {
            get { return null; }
        }

        public override DateTime? ArchivedTime
        {
            get { return null; }
        }

        public override bool IsEncrypted
        {
            get { return false; }
        }

        public override bool IsDirectory
        {
            get { return filePart.Header.FileType == 5; }
        }

        public override bool IsSplit
        {
            get { return false; }
        }

        internal override IEnumerable<FilePart> Parts
        {
            get { return filePart.AsEnumerable<FilePart>(); }
        }

        internal static IEnumerable<TarEntry> GetEntries(Stream stream)
        {
            foreach (TarHeader h in TarHeaderFactory.ReadHeaderNonseekable(stream))
            {
                if (h != null)
                {
                    yield return new TarEntry(new TarFilePart(h));
                }
            }
        }
    }
}