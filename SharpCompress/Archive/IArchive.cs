using System.Collections.Generic;
using SharpCompress.Common;

namespace SharpCompress.Archive
{
    public interface IArchive
    {
        IEnumerable<IArchiveEntry> Entries
        {
            get;
        }
        long TotalSize
        {
            get;
        }
        IEnumerable<IVolume> Volumes
        {
            get;
        }
    }
}
