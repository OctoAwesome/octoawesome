using System;
using System.Collections.Generic;
using System.Threading;

namespace OctoAwesome.Threading
{
    /// <summary>
    /// Semaphore for shared recursive scoped locking.
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    ///   <item>Can be exclusively locked.</item>
    ///   <item>Non exclusive locks can be locked recursively.</item>
    ///   <item>Multiple non exclusive locks are allowed on different threads.</item>
    /// </list>
    /// </remarks>
    public class CountedScopeSemaphore : IDisposable
    {
        private readonly ManualResetEventSlim exclusiveLock;
        private readonly ManualResetEventSlim mainLock;

        private readonly object lockObject;
        private readonly object countLockObject;
        private int counter;
        /// <summary>
        /// Initializes a new instance of the <see cref="CountedScopeSemaphore"/> class.
        /// </summary>
        public CountedScopeSemaphore()
        {
            mainLock = new ManualResetEventSlim(true);
            exclusiveLock = new ManualResetEventSlim(true);
            lockObject = new object();
            countLockObject = new object();
        }

        /// <summary>
        /// Enters an exclusive lock scope, which can be left by disposing the returned instance.
        /// </summary>
        /// <returns>An exclusive scope wrapper used for leaving the scope.</returns>
        /// <remarks>using(#.EnterExclusiveScope())...</remarks>
        public ExclusiveScope EnterExclusiveScope()
        {
            lock (lockObject)
            {
                mainLock.Wait();
                exclusiveLock.Wait();
                exclusiveLock.Reset();
            }
            return new ExclusiveScope(this);
        }

        /// <summary>
        /// Enters a counted lock scope, which can be left by disposing the returned instance.
        /// </summary>
        /// <returns>A counting scope wrapper used for leaving the scope.</returns>
        /// <remarks>using(#.EnterCountScope())...</remarks>
        public CountScope EnterCountScope()
        {
            lock (lockObject)
            {
                exclusiveLock.Wait();
                lock (countLockObject)
                {
                    counter++;
                    if (counter > 0)
                        mainLock.Reset();
                }
            }

            return new CountScope(this);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            exclusiveLock.Dispose();
            mainLock.Dispose();
        }

        private void LeaveMainScope()
        {
            lock (countLockObject)
            {
                counter--;
                if (counter == 0)
                    mainLock.Set();
            }
        }

        private void LeaveExclusiveScope()
        {
            exclusiveLock.Set();
        }

        /// <summary>
        /// Counted scope wrapper struct for leaving counted scope of <see cref="CountedScopeSemaphore"/> on dispose.
        /// </summary>
        public readonly struct CountScope : IDisposable, IEquatable<CountScope>
        {
            private readonly CountedScopeSemaphore internalSemaphore;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExclusiveScope"/> struct.
            /// </summary>
            /// <param name="countingSemaphore">The locked semaphore to leave counted scope on dispose.</param>
            public CountScope(CountedScopeSemaphore countingSemaphore)
            {
                internalSemaphore = countingSemaphore;
            }

            /// <summary>
            /// Leaves the counted scope of the <see cref="CountedScopeSemaphore"/>.
            /// </summary>
            public void Dispose()
            {
                internalSemaphore.LeaveMainScope();
            }

            /// <inheritdoc />
            public override bool Equals(object? obj)
                => obj is CountScope scope
                  && Equals(scope);

            /// <inheritdoc />
            public bool Equals(CountScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);

            /// <inheritdoc />
            public override int GetHashCode()
                => 37286538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);

            /// <summary>
            /// Compares whether two <see cref="CountScope"/> structs are equal.
            /// </summary>
            /// <param name="left">The first scope to compare to.</param>
            /// <param name="right">The second scope to compare with.</param>
            /// <returns>A value indicating whether the two scopes are equal.</returns>
            public static bool operator ==(CountScope left, CountScope right)
                => left.Equals(right);

            /// <summary>
            /// Compares whether two <see cref="CountScope"/> structs are unequal.
            /// </summary>
            /// <param name="left">The first scope to compare to.</param>
            /// <param name="right">The second scope to compare with.</param>
            /// <returns>A value indicating whether the two scopes are unequal.</returns>
            public static bool operator !=(CountScope left, CountScope right)
                => !(left == right);
        }

        /// <summary>
        /// Exclusive scope wrapper struct for leaving exclusive scope of <see cref="CountedScopeSemaphore"/> on dispose.
        /// </summary>
        public readonly struct ExclusiveScope : IDisposable, IEquatable<ExclusiveScope>
        {
            private readonly CountedScopeSemaphore internalSemaphore;

            /// <summary>
            /// Initializes a new instance of the <see cref="ExclusiveScope"/> struct.
            /// </summary>
            /// <param name="semaphore">The locked semaphore to leave exclusive scope on dispose.</param>
            public ExclusiveScope(CountedScopeSemaphore semaphore)
            {
                internalSemaphore = semaphore;
            }

            /// <summary>
            /// Leaves the exclusive scope of the <see cref="CountedScopeSemaphore"/>.
            /// </summary>
            public void Dispose()
            {
                internalSemaphore?.LeaveExclusiveScope();
            }

            /// <inheritdoc />
            public override bool Equals(object? obj) => obj is ExclusiveScope scope && Equals(scope);

            /// <inheritdoc />
            public bool Equals(ExclusiveScope other)
                => EqualityComparer<CountedScopeSemaphore>.Default.Equals(internalSemaphore, other.internalSemaphore);

            /// <inheritdoc />
            public override int GetHashCode()
                => 37296538 + EqualityComparer<CountedScopeSemaphore>.Default.GetHashCode(internalSemaphore);

            /// <summary>
            /// Compares whether two <see cref="ExclusiveScope"/> structs are equal.
            /// </summary>
            /// <param name="left">The first scope to compare to.</param>
            /// <param name="right">The second scope to compare with.</param>
            /// <returns>A value indicating whether the two scopes are equal.</returns>
            public static bool operator ==(ExclusiveScope left, ExclusiveScope right) => left.Equals(right);

            /// <summary>
            /// Compares whether two <see cref="ExclusiveScope"/> structs are unequal.
            /// </summary>
            /// <param name="left">The first scope to compare to.</param>
            /// <param name="right">The second scope to compare with.</param>
            /// <returns>A value indicating whether the two scopes are unequal.</returns>
            public static bool operator !=(ExclusiveScope left, ExclusiveScope right) => !(left == right);
        }
    }
}
