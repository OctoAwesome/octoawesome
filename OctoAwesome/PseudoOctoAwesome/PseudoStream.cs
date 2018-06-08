using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PseudoOctoAwesome
{
    class PseudoStream
    {
        private byte[] internalBuffer;
        private int position;
        private int readPosition;
        private int writePosition;

        public PseudoStream()
        {
            internalBuffer = new byte[1000];
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
