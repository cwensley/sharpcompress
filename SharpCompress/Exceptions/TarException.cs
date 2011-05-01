using System;

namespace SharpCompress
{
#if SILVERLIGHT || PORTABLE
    public class TarException : Exception
#else
    public class TarException : ApplicationException
#endif
    {
        public TarException(string message)
            : base(message)
        {
        }
    }
}