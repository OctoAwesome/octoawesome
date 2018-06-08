using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Network
{
    public class OctoNetworkStream
    {
        public bool CanRead => throw new NotImplementedException();

        public bool CanSeek => false;

        public bool CanWrite => throw new NotImplementedException();

        public long Length => throw new NotImplementedException();

        public long Position { get; set; }

        private byte[] internalBuffer;
        private int bufferIndex;
        private int bufferSize;
        private int writePosition;
        private int readPosition;

        public OctoNetworkStream()
        {
            bufferSize = 1000;
            internalBuffer = new byte[1000];
            bufferIndex = 0;
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            var tmpCount = count;

            if (buffer.Length - offset < count)
                tmpCount = buffer.Length - offset;

            if (readPosition + tmpCount > internalBuffer.Length)
            {
                var ex = new IndexOutOfRangeException("Dont't worry, Shit happens");
                ex.Data.Add("Internal Length", internalBuffer.Length);
                ex.Data.Add("Count", tmpCount);
                ex.Data.Add("Offset", offset);
                throw ex;
            }

            Array.Copy(internalBuffer, readPosition, buffer, offset, tmpCount);
            Interlocked.Exchange(ref readPosition, readPosition + tmpCount);

            return count;
        }


        public void Write(byte[] buffer, int offset, int count)
        {
            if (buffer.Length < count + offset)
            {
                var ex = new IndexOutOfRangeException("Dont't worry, Shit happens");
                ex.Data.Add("Buffer Length", buffer.Length);
                ex.Data.Add("Count", count);
                ex.Data.Add("Offset", offset);
                throw ex;
            }

            int toWrite = count;
            int bufferPostion = offset;

            if (writePosition + toWrite > internalBuffer.Length)
                toWrite = internalBuffer.Length - writePosition;

            Array.Copy(buffer, bufferPostion, internalBuffer, writePosition, toWrite);

            if (writePosition + toWrite > internalBuffer.Length)
            {
                bufferPostion += toWrite;
                toWrite = count - toWrite;
                writePosition = 0;

                if (writePosition + toWrite > readPosition)
                {
                    var ex = new IndexOutOfRangeException("Dont't worry, Shit happens");
                    ex.Data.Add("Buffer Length", buffer.Length);
                    ex.Data.Add("Count", count);
                    ex.Data.Add("Offset", offset);
                    ex.Data.Add("Readpos", readPosition);
                    ex.Data.Add("Writepos", writePosition);
                    throw ex;
                }

                Array.Copy(buffer, bufferPostion, internalBuffer, writePosition, toWrite);
            }

            Interlocked.Exchange(ref writePosition, writePosition + toWrite);
        }
    }
}
