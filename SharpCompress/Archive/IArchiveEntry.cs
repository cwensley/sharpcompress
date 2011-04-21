using System.IO;
using SharpCompress.Common;

namespace SharpCompress.Archive
{
    public interface IArchiveEntry : IEntry
    {
        void WriteTo(Stream stream, IExtractionListener listener);
    }
}
