using System;

namespace OctoAwesome.Network
{
    /// <summary>
    /// Double buffered network stream implementation.
    /// </summary>
    public class OctoNetworkStream
    {
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

        /// <summary>
        /// Initializes a new instance of the <see cref="OctoNetworkStream"/> class.
        /// </summary>
        /// <param name="capacity">The buffer capacity per buffer.</param>
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

        /// <summary>
        /// Writes a given range from a byte buffer to the stream.
        /// </summary>
        /// <param name="buffer">The buffer array to write to the stream.</param>
        /// <param name="offset">The buffer slice offset to get from <paramref name="buffer"/>.</param>
        /// <param name="count">The buffer slice count to get from <paramref name="buffer"/>.</param>
        /// <returns>The number of bytes written.</returns>
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

        /// <summary>
        /// Writes a single byte to the stream.
        /// </summary>
        /// <param name="data">The single byte to write.</param>
        /// <returns>The number of bytes that where written.</returns>
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

        /// <summary>
        /// Reads from the stream into a buffer.
        /// </summary>
        /// <param name="buffer">The buffer to read into.</param>
        /// <param name="offset">The slice buffer offset to start reading into.</param>
        /// <param name="count">The number of bytes to read.</param>
        /// <returns>The actually read number of bytes.</returns>
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

        /// <summary>
        /// Gets the number of bytes available with a maximum value of <paramref name="count"/>.
        /// </summary>
        /// <param name="count">The maximum data to make available.</param>
        /// <returns>The available number of readable bytes in the stream.</returns>
        public int DataAvailable(int count)
        {
            if (!writingProcess)
                SwapBuffer();

            var maxCopy = maxReadCount - readPosition;

            if (maxCopy < 1)
                return maxCopy;

            if (maxCopy < count)
                count = maxCopy;

            return count;
        }

        private void SwapBuffer()
        {
            lock (readLock)
                lock (writeLock)
                {
                    if (readPosition > maxReadCount)
                        throw new IndexOutOfRangeException("ReadPosition is greater than MaxReadCount in OctoNetworkStream");
                    if (readPosition < maxReadCount)
                        return;

                    (writeBuffer, readBuffer) = (readBuffer, writeBuffer);
                    maxReadCount = writePosition;
                    writePosition = 0;
                    readPosition = 0;
                }
        }


    }
}
