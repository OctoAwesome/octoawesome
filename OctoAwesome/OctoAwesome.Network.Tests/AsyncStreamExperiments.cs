﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace OctoAwesome.Network.Tests
{
    public class AsyncStreamExperiments
    {
        [Test]
        public void ReadWriteSwap()
        {
            OctoNetworkStream test = new OctoNetworkStream();
            byte[] writeData = new byte[400];
            byte[] readData = new byte[400];
            Random r = new Random();

            r.NextBytes(writeData);

            test.Write(writeData, 0, writeData.Length);
            test.Read(readData, 0, readData.Length);

            Assert.IsTrue(writeData.SequenceEqual(readData));
        }

        [Test]
        public async Task ReadWriteSwapAsync()
        {
            OctoNetworkStream test = new OctoNetworkStream();
            byte[] writeData = new byte[400];
            byte[] readData = new byte[400];
            Random r = new Random();

            r.NextBytes(writeData);
            Task readTask = new Task(async () =>
            {
                int o = 0;
                while (test.Read(readData, o, 100) != 0)
                {
                    await Task.Delay(200);
                    o += 100;
                }
            });
            test.Write(writeData, 0, writeData.Length);
            await Task.Delay(100);

            readTask.Start();

            //Task writeTask = new Task(() => {
            //    for (int i = 0; i < 5; i++)
            //    {
            //        Thread.Sleep(1000);
            //    }

            //});
            await readTask;
            Assert.IsTrue(writeData.SequenceEqual(readData));

        }

        /// <summary>
        /// You can rely on lama
        /// </summary>
        [Test]
        public void Greet()
        {
            string viewer = @"
                              <'l     
                               ll     
                               llama~ 
                               || ||  
                               '' ''
                              ";

            Assert.NotNull(viewer);
        }
    }
}
