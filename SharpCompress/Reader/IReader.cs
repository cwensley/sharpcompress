using System.IO;
using SharpCompress.Common;

namespace SharpCompress.Reader
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
