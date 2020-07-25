using OctoAwesome.Threading;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Screens
{
    public sealed class QuoteProvider
    {
        private readonly FileInfo fileInfo;
        private readonly Random random;
        private bool loaded;
        private string[] quotes;

        private readonly LockSemaphore semaphoreExtended;

        public QuoteProvider(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            random = new Random();
            semaphoreExtended = new LockSemaphore(1, 1);
        }

        public string GetRandomQuote()
        {
            using (semaphoreExtended.Wait())
            {
                Load();
                return quotes[random.Next(0, quotes.Length)];
            }
        }

        private void Load()
        {
            if (loaded)
                return;

            loaded = true;
            quotes = File.ReadAllLines(fileInfo.FullName);
        }
    }
}
