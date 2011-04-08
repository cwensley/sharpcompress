using System.IO;
using NUnrar.Common;
#if THREEFIVE
using NUnrar.Headers;
#endif

namespace NUnrar.Archive
{

    public static class RarArchiveEntryExtensions
    {
#if !PORTABLE
        /// <summary>
        /// Extract to specific directory, retaining filename
        /// </summary>
        public static void WriteToDirectory(this RarArchiveEntry entry, string destinationDirectory,
            IExtractionListener listener,
            ExtractOptions options = ExtractOptions.Overwrite)
        {
            string destinationFileName = string.Empty;
            string file = Path.GetFileName(entry.FilePath);


            if (options.HasFlag(ExtractOptions.ExtractFullPath))
            {

                string folder = Path.GetDirectoryName(entry.FilePath);
                string destdir = Path.Combine(destinationDirectory, folder);
                if (!Directory.Exists(destdir))
                {
                    Directory.CreateDirectory(destdir);
                }
                destinationFileName = Path.Combine(destdir, file);
            }
            else
            {
                destinationFileName = Path.Combine(destinationDirectory, file);
            }

            entry.WriteToFile(destinationFileName, listener, options);
        }

        /// <summary>
        /// Extract to specific directory, retaining filename
        /// </summary>
        public static void WriteToDirectory(this RarArchiveEntry entry, string destinationPath,
            ExtractOptions options = ExtractOptions.Overwrite)
        {
            entry.WriteToDirectory(destinationPath, new NullExtractionListener(), options);
        }

        /// <summary>
        /// Extract to specific file
        /// </summary>
        public static void WriteToFile(this RarArchiveEntry entry, string destinationFileName,
                        IExtractionListener listener,
            ExtractOptions options = ExtractOptions.Overwrite)
        {
            FileMode fm = FileMode.Create;

            if (!options.HasFlag(ExtractOptions.Overwrite))
            {
                fm = FileMode.CreateNew;
            }
            using (FileStream fs = File.Open(destinationFileName, fm))
            {
                entry.WriteTo(fs, listener);
            }
        }

        /// <summary>
        /// Extract to specific file
        /// </summary>
        public static void WriteToFile(this RarArchiveEntry entry, string destinationFileName,
           ExtractOptions options = ExtractOptions.Overwrite)
        {
            entry.WriteToFile(destinationFileName, new NullExtractionListener(), options);
        }
#endif

        public static void WriteTo(this RarArchiveEntry entry, Stream stream)
        {
            entry.WriteTo(stream, new NullExtractionListener());
        }
    }
}
