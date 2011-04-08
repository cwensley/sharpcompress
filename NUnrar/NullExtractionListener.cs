
using NUnrar.Common;

namespace NUnrar
{
    public class NullExtractionListener : IExtractionListener
    {
        public void OnFileEntryExtractionInitialized(string entryFileName, long? totalEntryCompressedBytes)
        {
        }

        public void OnFilePartExtractionInitialized(string partFileName, long totalPartCompressedBytes)
        {
        }

        public void OnCompressedBytesRead(long currentPartCompressedBytes, long currentEntryCompressedBytes)
        {
        }

        public void OnInformation(string message)
        {
        }
    }
}
