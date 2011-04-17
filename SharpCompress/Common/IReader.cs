using System.IO;

namespace SharpCompress.Common
{
    public interface IReader
    {
        ReaderType ReaderType
        {
            get;
        }

        IEntry Entry
        {
            get;
        }
        void WriteEntryTo(Stream writableStream);
        bool MoveToNextEntry();
    }
}
