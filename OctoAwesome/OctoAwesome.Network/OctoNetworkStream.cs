using System;

namespace OctoAwesome.Network
{
    public class OctoNetworkStream
    {
        public int Length => writeBuffer.Length;

        private byte[] readBuffer;
        private byte[] writeBuffer;

        private readonly byte[] bufferA;
        private readonly byte[] bufferB;

        private readonly object readLock;
        private readonly object writeLock;

        private readonly int writeLength;
        private readonly int readLength;

        private int maxReadCount;

        private int readPosition;
        private int writePosition;

        private bool writingProcess;

        public OctoNetworkStream(int capacity = 1024)
        {
            bufferA = new byte[capacity];
            bufferB = new byte[capacity];
            readBuffer = bufferA;
            writeBuffer = bufferB;
            readLength = capacity;
            writeLength = capacity;
            readPosition = 0;
            writePosition = 0;
            readLock = new object();
            writeLock = new object();
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            writingProcess = true;

            SwapBuffer();

            var maxCopy = writeLength - writePosition;

            if (maxCopy < count)
                count = maxCopy;

            if (maxCopy < 1)
            {
                writingProcess = false;
                return maxCopy;
            }

            lock (writeLock)
                Buffer.BlockCopy(buffer, offset, writeBuffer, writePosition, count);

            writePosition += count;

            writingProcess = false;

            return count;
        }

        public int Write(byte data)
        {
            writingProcess = true;

            SwapBuffer();

            if (writeLength == writePosition)
            {
                writingProcess = false;
                return 0;
            }

            lock (writeLock)
                writeBuffer[writePosition++] = data;

            writingProcess = false;

            return 1;
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            if (!writingProcess)
                SwapBuffer();

            var maxCopy = maxReadCount - readPosition;

            if (maxCopy < 1)
                return maxCopy;

            if (maxCopy < count)
                count = maxCopy;

            lock (readLock)
                Buffer.BlockCopy(readBuffer, readPosition, buffer, offset, count);

            readPosition += count;

            return count;
        }

        private void SwapBuffer()
        {
            lock (readLock)
                lock (writeLock)
                {
                    if (readPosition > maxReadCount)
                        throw new IndexOutOfRangeException("ReadPositin is greater than MaxReadCount in OctoNetworkStream");
                    else if (readPosition < maxReadCount)
                        return;

                    var refBuf = writeBuffer;
                    writeBuffer = readBuffer;
                    readBuffer = refBuf;
                    maxReadCount = writePosition;
                    writePosition = 0;
                    readPosition = 0;
                }
        }

    }
}
