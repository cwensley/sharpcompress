﻿using System;
using System.Collections.Generic;
using System.IO;
using SharpCompress.Archive;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using SharpCompress.Reader;
using SharpCompress.Reader.Rar;
using SharpCompress.Reader.Zip;

namespace SharpCompress.Test
{
    static class Program
    {
        static void Main()
        {
            new RewindableStreamTest().Test();
            TestGenericGZip();
        }

        public static void TestRewind()
        {
            using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress\TestArchives\sharpcompress.rar"))
            {
                var reader = ReaderFactory.OpenReader(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(@"C:\temp",
                                                     ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
        }

        public static void TestGenericGZip()
        {
            using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress\TestArchives\sharpcompress.rar.gz"))
            {
                var reader = ReaderFactory.OpenReader(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(@"C:\temp",
                                                     ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
        }

        public static void TestGenericTgz()
        {
            using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress\TestArchives\sharpcompress.tar.gz"))
            {
                var reader = ReaderFactory.OpenReader(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(@"C:\temp",
                                                     ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
        }


        public static void TestGenericTar()
        {
            using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress\TestArchives\sharpcompress.tar"))
            {
                var reader = ReaderFactory.OpenReader(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(@"C:\temp",
                                                     ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
        }


        public static void TestRar()
        {
            var archive = RarArchive.Open(@"C:\Code\sharpcompress\TestArchives\sharpcompress.rar");
            foreach (var entry in archive.Entries)
            {
                entry.WriteToDirectory(@"C:\temp");
            }
        }

        public static void TestZip()
        {
            if (!ZipArchive.IsZipFile(@"C:\Code\sharpcompress\TestArchives\sharpcompress.zip"))
            {
                throw new InvalidOperationException();
            }
            using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress\TestArchives\sharpcompress.zip"))
            {
                var reader = ZipReader.Open(stream);
                while (reader.MoveToNextEntry())
                {
                    if (!reader.Entry.IsDirectory)
                    {
                        Console.WriteLine(reader.Entry.FilePath);
                        reader.WriteEntryToDirectory(@"C:\temp",
                                                     ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
            }
        }

        public static void TestGenericZip()
        {
            if (!ZipArchive.IsZipFile(@"C:\Code\sharpcompress\TestArchives\sharpcompress.zip"))
            {
                throw new InvalidOperationException();
            }
            using (Stream stream = File.OpenRead(@"C:\Code\sharpcompress\TestArchives\sharpcompress.zip"))
            {
                var archive = ArchiveFactory.Open(stream);
                foreach (var entry in archive.Entries)
                {
                    if (!entry.IsDirectory)
                    {
                        Console.WriteLine(entry.FilePath);
                        entry.WriteToDirectory(@"C:\temp",
                                                     ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
                    }
                }
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

        public static void TestRar3()
        {
            var archive = RarArchive.Open(@"C:\Code\sharpcompress\TestArchives\sharpcompress2.part001.rar");
            foreach (var entry in archive.Entries)
            {
                Console.WriteLine(entry.FilePath);
                entry.WriteToDirectory(@"C:\temp", ExtractOptions.ExtractFullPath | ExtractOptions.Overwrite);
            }
        }

        private static IEnumerable<Stream> TestRar2Streams()
        {
            string file = @"C:\Code\sharpcompress\TestArchives\sharpcompress2.part001.rar";
            int count = 1;
            while (File.Exists(file))
            {
                Console.WriteLine(file);
                yield return File.OpenRead(file);

                count++;
                file = @"C:\Code\sharpcompress\TestArchives\sharpcompress2.part";
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
