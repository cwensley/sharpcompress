using System.Collections.Generic;

namespace SharpCompress.Common
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
