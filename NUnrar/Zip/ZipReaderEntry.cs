using System;
using NUnrar.Common;

namespace NUnrar.Zip
{
    public class ZipReaderEntry : Entry
    {
        private ZipFilePart filePart;
        private bool directory;
        private DateTime? lastModifiedTime;

        internal ZipReaderEntry(ZipFilePart filePart)
        {
            this.filePart = filePart;
            directory = filePart.Header.Name.EndsWith("/");
            lastModifiedTime = Utility.DosDateToDateTime(filePart.Header.LastModifiedDate,
                                                         filePart.Header.LastModifiedTime);
        }

        #region IEntry Members

        public override uint Crc
        {
            get { return filePart.Header.Crc; }
        }

        public override string FilePath
        {
            get { return filePart.Header.Name; }
        }

        public override long CompressedSize
        {
            get { return filePart.Header.CompressedSize; }
        }

        public override long Size
        {
            get { return filePart.Header.UncompressedSize; }
        }

        public override DateTime? LastModifiedTime
        {
            get { return lastModifiedTime; }
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
            get { throw new NotImplementedException(); }
        }

        public override bool IsDirectory
        {
            get { return directory; }
        }

        public override bool IsSplit
        {
            get { throw new NotImplementedException(); }
        }

        #endregion

        internal override System.Collections.Generic.IEnumerable<FilePart> Parts
        {
            get { return this.filePart.AsEnumerable(); }
        }
    }
}
