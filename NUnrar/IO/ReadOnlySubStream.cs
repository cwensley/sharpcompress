using System.IO;

namespace NUnrar.IO
{
    internal class ReadOnlySubStream : Stream
    {
        private bool leaveOpen;

        public ReadOnlySubStream(Stream stream, long bytesToRead, bool leaveOpen)
        {
            Stream = stream;
            BytesLeftToRead = bytesToRead;
            this.leaveOpen = leaveOpen;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !leaveOpen)
            {
                Stream.Dispose();
            }
        }

        private long BytesLeftToRead
        {
            get;
            set;
        }

        public Stream Stream
        {
            get;
            private set;
        }

        public override bool CanRead
        {
            get
            {
                return true;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override long Length
        {
            get
            {
                throw new System.NotImplementedException();
            }
        }

        public override long Position
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
                throw new System.NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (BytesLeftToRead < count)
            {
                count = (int)BytesLeftToRead;
            }
            int read = Stream.Read(buffer, offset, count);
            if (read > 0)
            {
                BytesLeftToRead -= read;
            }
            return read;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new System.NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new System.NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new System.NotImplementedException();
        }
    }
}
