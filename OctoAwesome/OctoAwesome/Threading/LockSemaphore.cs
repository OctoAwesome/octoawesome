using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OctoAwesome.Threading
{
    /// <summary>
    /// Semaphore wrapping <see cref="SemaphoreSlim"/> with convenient dispose release pattern.
    /// </summary>
    public sealed class LockSemaphore : IDisposable
    {
        private readonly SemaphoreSlim semaphoreSlim;

        /// <summary>
        /// Initializes a new instance of the <see cref="LockSemaphore"/> class.
        /// </summary>
        /// <param name="initialCount">The initial number of requests for the semaphore that can be granted
        /// concurrently.</param>
        /// <param name="maxCount">The maximum number of requests for the semaphore that can be granted
        /// concurrently.</param>
        /// <exception cref="System.ArgumentOutOfRangeException"> <paramref name="initialCount"/>
        /// is less than 0. -or-
        /// <paramref name="initialCount"/> is greater than <paramref name="maxCount"/>. -or-
        /// <paramref name="maxCount"/> is less than 0.</exception>
        public LockSemaphore(int initialCount, int maxCount)
        {
            semaphoreSlim = new SemaphoreSlim(initialCount, maxCount);
        }

        /// <summary>
        /// Blocks the current thread until it can enter the <see cref="LockSemaphore"/>.
        /// </summary>
        /// <returns><see cref="SemaphoreLock"/> that can be disposed to release the lock.</returns>
        /// <exception cref="System.ObjectDisposedException">The current instance has already been
        /// disposed.</exception>
        public SemaphoreLock Wait()
        {
            semaphoreSlim.Wait();
            return new SemaphoreLock(this);
        }
        /// <summary>
        /// Asynchronously waits to enter the <see cref="LockSemaphore"/>, while observing a
        /// <see cref="System.Threading.CancellationToken"/>.
        /// </summary>
        /// <returns>A task that will complete when the semaphore has been entered.</returns>
        /// <param name="token">
        /// The <see cref="System.Threading.CancellationToken"/> token to observe.
        /// </param>
        /// <exception cref="System.ObjectDisposedException">
        /// The current instance has already been disposed.
        /// </exception>
        public async Task<SemaphoreLock> WaitAsync(CancellationToken token)
        {
            await semaphoreSlim.WaitAsync(token);
            return new SemaphoreLock(this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            semaphoreSlim.Dispose();
        }

        private void Release()
        {
            semaphoreSlim.Release();
        }

        /// <summary>
        /// Semaphore lock wrapper struct for releasing <see cref="LockSemaphore"/> on dispose.
        /// </summary>
        public readonly struct SemaphoreLock : IDisposable, IEquatable<SemaphoreLock>
        {
            private readonly LockSemaphore internalSemaphore;

            /// <summary>
            /// Initializes a new instance of the <see cref="SemaphoreLock"/> struct.
            /// </summary>
            /// <param name="semaphoreExtended">The locked semaphore to be released on dispose.</param>
            public SemaphoreLock(LockSemaphore semaphoreExtended)
            {
                internalSemaphore = semaphoreExtended;
            }

            /// <summary>
            /// Releases the <see cref="LockSemaphore"/>.
            /// </summary>
            public void Dispose()
            {
                internalSemaphore.Release();
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
                => obj is SemaphoreLock @lock
                   && Equals(@lock);

            /// <inheritdoc />
            public bool Equals(SemaphoreLock other)
                => EqualityComparer<LockSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);

            /// <inheritdoc />
            public override int GetHashCode()
                => 37286538 + (internalSemaphore == null ? 0 : EqualityComparer<LockSemaphore>.Default.GetHashCode(internalSemaphore));

            /// <summary>
            /// Compares whether two <see cref="SemaphoreLock"/> structs are equal.
            /// </summary>
            /// <param name="left">The first lock to compare to.</param>
            /// <param name="right">The second lock to compare with.</param>
            /// <returns>A value indicating whether the two locks are equal.</returns>
            public static bool operator ==(SemaphoreLock left, SemaphoreLock right)
                => left.Equals(right);

            /// <summary>
            /// Compares whether two <see cref="SemaphoreLock"/> structs are unequal.
            /// </summary>
            /// <param name="left">The first lock to compare to.</param>
            /// <param name="right">The second lock to compare with.</param>
            /// <returns>A value indicating whether the two locks are unequal.</returns>
            public static bool operator !=(SemaphoreLock left, SemaphoreLock right)
                => !(left == right);
        }
    }
}
