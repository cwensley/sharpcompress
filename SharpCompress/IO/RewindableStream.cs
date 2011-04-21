using System.IO;

namespace SharpCompress.IO
{
    internal class RewindableStream : Stream
    {
        private readonly Stream stream;
        private readonly MemoryStream bufferStream = new MemoryStream();
        private bool isRewound;

        public RewindableStream(Stream stream)
        {
            this.stream = stream;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (disposing)
            {
                stream.Dispose();
            }
        }

        public void Rewind()
        {
            isRewound = true;
            bufferStream.Position = 0;
        }

        public bool Recording
        {
            get;
            set;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void Flush()
        {
            throw new System.NotImplementedException();
        }

        public override long Length
        {
            get { throw new System.NotImplementedException(); }
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
            int read;
            if (isRewound && bufferStream.Position != bufferStream.Length)
            {
                read = bufferStream.Read(buffer, offset, count);
                if (read < count)
                {
                    int tempRead = stream.Read(buffer, offset + read, count - read);
                    if (Recording)
                    {
                        bufferStream.Write(buffer, offset + read, tempRead);
                    }
                    read += tempRead;
                }
                return read;
            }

            read = stream.Read(buffer, offset, count);
            if (Recording)
            {
                bufferStream.Write(buffer, offset, read);
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
