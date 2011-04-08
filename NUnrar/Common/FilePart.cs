using System.IO;

namespace SharpCompress.Common
{
    public abstract class FilePart
    {

        internal bool StreamOwner
        {
            get;
            set;
        }

        internal abstract string FilePartName
        {
            get;
        }

        internal abstract Stream GetStream();
    }
}
