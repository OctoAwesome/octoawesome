using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace OctoAwesome.Network.Tests
{
    [TestClass]
    public class AsyncStreamExperiments
    {
        [TestMethod]
        public void ReadWriteSwap()
        {
            TestAsyncStream test = new TestAsyncStream();
            byte[] writeData = new byte[400];
            byte[] readData = new byte[400];
            Random r = new Random();

            r.NextBytes(writeData);

            test.Write(writeData, 0, writeData.Length);
            test.Read(readData, 0, readData.Length);

            Assert.IsTrue(writeData.SequenceEqual(readData));
        }

        [TestMethod]
        public async Task ReadWriteSwapAsync()
        {
            TestAsyncStream test = new TestAsyncStream();
            byte[] writeData = new byte[400];
            byte[] readData = new byte[400];
            Random r = new Random();

            r.NextBytes(writeData);
            Task readTask = new Task(() => {
                while (test.Read(readData, 0, readData.Length) == 0)
                {
                }
            });
            readTask.Start();

            Thread.Sleep(1000);

            test.Write(writeData, 0, writeData.Length);
            //Task writeTask = new Task(() => {
            //    for (int i = 0; i < 5; i++)
            //    {
            //        Thread.Sleep(1000);
            //    }

            //});
            Assert.IsTrue(writeData.SequenceEqual(readData));

        }

        private class TestAsyncStream
        {
            private byte[] readBuffer;
            private byte[] writeBuffer;

            private readonly byte[] bufferA;
            private readonly byte[] bufferB;

            private readonly int writeLength;
            private readonly int readLength;

            private int maxReadCount;

            private int readPosition;
            private int writePosition;

            private bool readingProcess;
            private bool writingProcess;

            public TestAsyncStream(int capacity = 1024)
            {
                bufferA = new byte[capacity];
                bufferB = new byte[capacity];
                readBuffer = bufferA;
                writeBuffer = bufferB;
                readLength = capacity;
                writeLength = capacity;
                readPosition = 0;
                writePosition = 0;
            }

            public int Write(byte[] buffer, int offset, int count)
            {
                writingProcess = true;

                if (!readingProcess)
                    SwapBuffer();

                var maxCopy = writeLength - writePosition;

                if (maxCopy < count)
                    count = maxCopy;

                Buffer.BlockCopy(buffer, offset, writeBuffer, writePosition, count);
                writePosition += count;

                writingProcess = false;

                return count;
            }

            public int Read(byte[] buffer, int offset, int count)
            {
                readingProcess = true;

                if (!writingProcess)
                    SwapBuffer();

                var maxCopy = maxReadCount - readPosition;

                if (maxCopy < count)
                    count = maxCopy;

                Array.Copy(readBuffer, readPosition, buffer, offset, count);
                readPosition += count;

                readingProcess = false;

                return count;
            }

            private void SwapBuffer()
            {
                readingProcess = true;
                writingProcess = true;

                var refBuf = writeBuffer;
                writeBuffer = readBuffer;
                readBuffer = refBuf;
                maxReadCount = writePosition;
                readPosition = 0;
                writePosition = 0;
            }
        }
    }
}
