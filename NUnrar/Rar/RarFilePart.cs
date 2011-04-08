using NUnrar.Common;
using NUnrar.Headers;

namespace NUnrar.Rar
{
    /// <summary>
    /// This represents a single file part that exists in a rar volume.  A compressed file is one or many file parts that are spread across one or may rar parts.
    /// </summary>
    public abstract class RarFilePart : FilePart
    {
        internal RarFilePart(MarkHeader mh, FileHeader fh, bool streamOwner)
        {
            MarkHeader = mh;
            FileHeader = fh;
            StreamOwner = streamOwner;
        }

        internal MarkHeader MarkHeader
        {
            get;
            private set;
        }

        internal FileHeader FileHeader
        {
            get;
            private set;
        }
    }
}
