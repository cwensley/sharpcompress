
namespace SharpCompress.Common
{
    public interface IExtractionListener
    {
        void OnFileEntryExtractionInitialized(string entryFileName, long? totalEntryCompressedBytes);

        void OnFilePartExtractionInitialized(string partFileName, long totalPartCompressedBytes);

        void OnCompressedBytesRead(long currentPartCompressedBytes, long currentEntryCompressedBytes);

        void OnInformation(string message);
    }
}
