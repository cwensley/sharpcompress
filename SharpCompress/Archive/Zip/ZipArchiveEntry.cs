using System.IO;
using System.Linq;
using SharpCompress.Common;
using SharpCompress.Common.Zip;

namespace SharpCompress.Archive.Zip
{
    public class ZipArchiveEntry : ZipEntry, IArchiveEntry
    {
        public ZipArchiveEntry(ZipFilePart part)
            : base(part)
        {
        }

        #region IArchiveEntry Members

        public void WriteTo(Stream streamToWriteTo, IExtractionListener listener)
        {
            if (IsEncrypted)
            {
                throw new RarExtractionException("Entry is password protected and cannot be extracted.");
            }

            if (IsDirectory)
            {
                throw new RarExtractionException("Entry is a file directory and cannot be extracted.");
            }

            listener.CheckNotNull("listener");
            listener.OnFileEntryExtractionInitialized(FilePath, CompressedSize);

            using (Stream s = Parts.Single().GetStream())
            {
                s.TransferTo(streamToWriteTo);
            }
        }

        #endregion
    }
}