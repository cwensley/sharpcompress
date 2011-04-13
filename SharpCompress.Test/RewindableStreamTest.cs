using System.Diagnostics;
using System.IO;
using SharpCompress.IO;

namespace SharpCompress.Test
{
    class RewindableStreamTest
    {
        public void Test()
        {
            MemoryStream ms = new MemoryStream();
            BinaryWriter bw = new BinaryWriter(ms);
            bw.Write(1);
            bw.Write(2);
            bw.Write(3);
            bw.Write(4);
            bw.Write(5);
            bw.Write(6);
            bw.Write(7);
            bw.Flush();
            ms.Position = 0;
            RewindableStream stream = new RewindableStream(ms);
            stream.Recording = true;
            BinaryReader br = new BinaryReader(stream);
            Debug.Assert(br.ReadInt32() == 1);
            Debug.Assert(br.ReadInt32() == 2);
            Debug.Assert(br.ReadInt32() == 3);
            Debug.Assert(br.ReadInt32() == 4);
            stream.Rewind();
            stream.Recording = true;
            Debug.Assert(br.ReadInt32() == 1);
            Debug.Assert(br.ReadInt32() == 2);
            Debug.Assert(br.ReadInt32() == 3);
            Debug.Assert(br.ReadInt32() == 4);
            Debug.Assert(br.ReadInt32() == 5);
            Debug.Assert(br.ReadInt32() == 6);
            Debug.Assert(br.ReadInt32() == 7);
            stream.Rewind();
            stream.Recording = true;
            Debug.Assert(br.ReadInt32() == 1);
            Debug.Assert(br.ReadInt32() == 2);
            Debug.Assert(br.ReadInt32() == 3);
            Debug.Assert(br.ReadInt32() == 4);
        }
    }
}
