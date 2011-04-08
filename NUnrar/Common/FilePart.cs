using System.IO;

namespace NUnrar.Common
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
