
namespace SharpCompress
{
    public class MultipartStreamRequiredException : RarException
    {
        public MultipartStreamRequiredException(string message)
            : base(message)
        {
        }
    }
}
