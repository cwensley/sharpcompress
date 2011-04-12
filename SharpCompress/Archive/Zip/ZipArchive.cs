using System;
using System.IO;
using System.Linq;
using SharpCompress.Common.Zip;
using SharpCompress.Common.Zip.Headers;

namespace SharpCompress.Archive.Zip
{
    public static class ZipArchive
    {
#if !PORTABLE
        public static bool IsZipFile(string filePath)
        {
            return IsZipFile(new FileInfo(filePath));
        }

        public static bool IsZipFile(FileInfo fileInfo)
        {
            if (!fileInfo.Exists)
            {
                return false;
            }
            using (Stream stream = fileInfo.OpenRead())
            {
                return IsZipFile(stream);
            }
        }
#endif
        public static bool IsZipFile(Stream stream)
        {
            try
            {
                ZipHeader header = ZipHeaderFactory.ReadHeaderNonseekable(stream).FirstOrDefault();
                if (header == null)
                {
                    return false;
                }
                return Enum.IsDefined(typeof(ZipHeaderType), header.ZipHeaderType);
            }
            catch
            {
                return false;
            }
        }
    }
}
