using System.IO;
using SharpCompress.Common;
using SharpCompress.Common.Rar;
using SharpCompress.IO;

namespace SharpCompress.Archive.Rar
{
    public abstract class RarArchiveVolume : RarVolume
    {
        internal RarArchiveVolume(StreamingMode mode, Options options)
            : base(mode, options)
        {
        }
    }
}
