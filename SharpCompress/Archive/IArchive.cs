using System.Collections.Generic;
using SharpCompress.Common;

namespace SharpCompress.Archive
{
    public interface IArchive
    {
        ICollection<IEntry> Entries
        {
            get;
        }
        long TotalSize
        {
            get;
        }
        ICollection<IVolume> Volumes
        {
            get;
        }
    }
}
