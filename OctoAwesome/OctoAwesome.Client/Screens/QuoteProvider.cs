using OctoAwesome.Threading;
using System;
using System.IO;

namespace OctoAwesome.Client.Screens
{
    /// <summary>
    /// Provider that holds quotes and can provide random ones.
    /// </summary>
    public sealed class QuoteProvider
    {
        private readonly FileInfo fileInfo;
        private readonly Random random;
        private bool loaded;
        private string[] quotes;

        private readonly LockSemaphore semaphoreExtended;

        /// <summary>
        /// Initializes a new instance of the <see cref="QuoteProvider"/> class.
        /// </summary>
        /// <param name="fileInfo">File info of the file to load the quotes from.</param>
        public QuoteProvider(FileInfo fileInfo)
        {
            this.fileInfo = fileInfo;
            random = new Random();
            semaphoreExtended = new LockSemaphore(1, 1);
            quotes = Array.Empty<string>();
        }

        /// <summary>
        /// Gets a random quote.
        /// </summary>
        /// <returns>The random quote.</returns>
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
