﻿using System;
using System.IO;
using SharpCompress.Archive.Rar;
using SharpCompress.Archive.Zip;
using SharpCompress.Common;
using SharpCompress.IO;
using SharpCompress.Reader.Rar;
using SharpCompress.Reader.Zip;

namespace SharpCompress.Reader
{
    public static class CompressedStreamFactory
    {
        /// <summary>
        /// Opens a CompressedStreamReader for Non-seeking usage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="listener"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CompressedStreamReader OpenReader(Stream stream, IExtractionListener listener,
            ReaderOptions options = ReaderOptions.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");

            RewindableStream rewindableStream = new RewindableStream(stream);
            rewindableStream.Recording = true;
            if (ZipArchive.IsZipFile(rewindableStream))
            {
                return ZipReader.Open(rewindableStream, listener, options);
            }
            rewindableStream.Rewind();
            rewindableStream.Recording = true;
            if (RarArchive.IsRarFile(rewindableStream))
            {
                rewindableStream.Rewind();
                return RarReader.Open(rewindableStream, listener, options);
            }
            throw new InvalidOperationException("Cannot determine compressed stream type.");
        }

        /// <summary>
        /// Opens a CompressedStreamReader for Non-seeking usage
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static CompressedStreamReader OpenReader(Stream stream, ReaderOptions options = ReaderOptions.KeepStreamsOpen)
        {
            stream.CheckNotNull("stream");
            return OpenReader(stream, new NullExtractionListener(), options);
        }
    }
}