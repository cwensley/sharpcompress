
using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archive.Rar;
using SharpCompress.Common;
using SharpCompress.Reader.Rar;
namespace SharpCompress.Test
{
    static class Program
    {
        static void Main()
        {
            TestRar2();
        }

        public static void TestRar()
        {
            var archive = RarArchive.Open(@"C:\Git\sharpcompress\TestArchives\sharpcompress.rar");
            foreach (var entry in archive.Entries)
            {
                entry.WriteToDirectory(@"C:\temp");
            }
        }


        public static void TestRar2()
        {
            var reader = RarReader.Open(TestRar2Streams());
            while (reader.MoveToNextEntry())
            {
                if (!reader.Entry.IsDirectory)
                {
                    Console.WriteLine(reader.Entry.FilePath);
                    reader.WriteEntryToDirectory(@"C:\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                }
            }
        }

        private static IEnumerable<Stream> TestRar2Streams()
        {
            string file = @"C:\Git\sharpcompress\TestArchives\sharpcompress2.part001.rar";
            int count = 1;
            while (File.Exists(file))
            {
                Console.WriteLine(file);
                yield return File.OpenRead(file);

                count++;
                file = @"C:\Git\sharpcompress\TestArchives\sharpcompress2.part";
                if (count < 10)
                {
                    file += "00";
                }
                else if (count < 100)
                {
                    file += "0";
                }
                file += count + ".rar";
            }
        }
    }
}
