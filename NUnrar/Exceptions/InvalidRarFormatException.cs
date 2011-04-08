using System;

namespace SharpCompress
{
    public class InvalidRarFormatException : RarException
    {
        public InvalidRarFormatException(string message)
            : base(message)
        {
        }

        public InvalidRarFormatException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
