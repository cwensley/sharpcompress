using System.IO;

namespace SharpCompress.Common
{
    public abstract class Volume
    {
        internal abstract Stream Stream { get; }

        /// <summary>
        /// RarArchive is the first volume of a multi-part archive.
        /// Only Rar 3.0 format and higher
        /// </summary>
        public abstract bool IsFirstVolume { get; }

        /// <summary>
        /// RarArchive is part of a multi-part archive.
        /// </summary>
        public abstract bool IsMultiVolume { get; }
    }
}
