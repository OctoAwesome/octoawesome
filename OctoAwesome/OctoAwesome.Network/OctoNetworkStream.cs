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
        public bool CanRead => true;

        public bool CanSeek => false;

        public bool CanWrite => true;

        public long Length => internalBuffer.LongLength;

        public int WritePosition => writePosition;
        public int ReadPosition => readPosition;


        private byte[] internalBuffer;
        private int writePosition;
        private int readPosition;

        public OctoNetworkStream(int capacity = 1024)
        {
            internalBuffer = new byte[capacity];
        }

        public void Flush()
        {
            throw new NotImplementedException();
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public void SetLength(int value)
            => Array.Resize(ref internalBuffer, value);

        public int Read(byte[] buffer, int offset, int count)
        {
            var tmpCount = count;

            if (buffer.Length > offset + count)
                throw new IndexOutOfRangeException();

            if (buffer.Length - offset < count)
                tmpCount = buffer.Length - offset;

            if (readPosition + tmpCount > internalBuffer.Length)
            {
                tmpCount = internalBuffer.Length - readPosition;
                Array.Copy(internalBuffer, readPosition, buffer, offset, tmpCount);
                offset = tmpCount;
                tmpCount = count - tmpCount;
                readPosition = 0;

                if (readPosition + tmpCount > writePosition)
                {
                    var ex = new IndexOutOfRangeException("Dont't worry, Shit happens");
                    ex.Data.Add("Writepos", writePosition);
                    ex.Data.Add("Count", count);
                    ex.Data.Add("Offset", offset);
                    throw ex;
                }
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

            int toWrite = count, bufferPostion = offset;

            if (writePosition + toWrite > internalBuffer.Length)
                toWrite = internalBuffer.Length - writePosition;

            Array.Copy(buffer, bufferPostion, internalBuffer, writePosition, toWrite);

            if (writePosition + count > internalBuffer.Length)
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

        public void Write(byte data)
        {
     
            int toWrite = 1;

            //if (writePosition + toWrite > readPosition)
            //{
            //    var ex = new IndexOutOfRangeException("Dont't worry, Shit happens");
            //    ex.Data.Add("Readpos", readPosition);
            //    ex.Data.Add("Writepos", writePosition);
            //    throw ex;
            //}

            if (writePosition + toWrite > internalBuffer.Length)
                toWrite = internalBuffer.Length - writePosition;

            if(toWrite == 0)
                writePosition %= internalBuffer.Length;

            internalBuffer[writePosition++] = data;
           //Interlocked.Exchange(ref writePosition, writePosition + toWrite);
        }
    }
}
