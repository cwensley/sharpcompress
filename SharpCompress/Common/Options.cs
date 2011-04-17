using System;

namespace SharpCompress.Common
{
    [Flags]
    public enum Options
    {
        /// <summary>
        /// No options specified
        /// </summary>
        None = 0,
        /// <summary>
        /// SharpCompress will keep the supplied streams open
        /// </summary>
        KeepStreamsOpen = 1,
        /// <summary>
        /// Check for self-extracting archives
        /// </summary>
        CheckForSFX = 2,
        /// <summary>
        /// Return entries for directories as well as files
        /// </summary>
        GiveDirectoryEntries = 4
    }
}
