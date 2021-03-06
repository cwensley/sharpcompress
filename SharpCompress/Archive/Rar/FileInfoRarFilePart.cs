﻿using System.IO;
using SharpCompress.Common.Rar;
using SharpCompress.Common.Rar.Headers;

namespace SharpCompress.Archive.Rar
{
    internal class FileInfoRarFilePart : RarFilePart
    {
        internal FileInfoRarFilePart(MarkHeader mh, FileHeader fh, FileInfo fi)
            : base(mh, fh, true)
        {
            FileInfo = fi;
        }

        internal FileInfo FileInfo
        {
            get;
            private set;
        }

        internal override Stream GetStream()
        {
            Stream stream = FileInfo.OpenRead();
            stream.Position = FileHeader.DataStartPosition;
            return stream;
        }

        internal override string FilePartName
        {
            get
            {
                return "Rar File: " + FileInfo.FullName
                    + " File Entry: " + FileHeader.FileName;
            }
        }
    }
}
