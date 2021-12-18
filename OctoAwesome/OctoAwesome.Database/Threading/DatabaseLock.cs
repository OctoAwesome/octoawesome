using System;
using System.Collections.Generic;

namespace OctoAwesome.Database.Threading
{
    /// <summary>
    /// Database lock wrapper struct for releasing lock of <see cref="DatabaseLockMonitor"/> on dispose.
    /// </summary>
    public readonly struct DatabaseLock : IDisposable, IEquatable<DatabaseLock>
    {
        private readonly DatabaseLockMonitor lockMonitor;
        private readonly Operation currentOperation;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseLock"/> struct.
        /// </summary>
        /// <param name="lockMonitor">The lock monitor to lock on.</param>
        /// <param name="operation">The type of <see cref="Operation"/> to use for locking.</param>
        public DatabaseLock(DatabaseLockMonitor lockMonitor, Operation operation)
        {
            this.lockMonitor = lockMonitor;
            currentOperation = operation;
        }

        /// <summary>
        /// Enters a lock of the <see cref="DatabaseLockMonitor"/>.
        /// </summary>
        public void Enter()
        {
            lockMonitor.AcquireLock(currentOperation);
        }

        /// <summary>
        /// Releases the lock of the <see cref="DatabaseLockMonitor"/>.
        /// </summary>
        public void Dispose()
        {
            lockMonitor.ReleaseLock(currentOperation);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
            => obj is DatabaseLock @lock && Equals(@lock);

        /// <inheritdoc />
        public bool Equals(DatabaseLock other)
            => EqualityComparer<DatabaseLockMonitor>.Default.Equals(lockMonitor, other.lockMonitor)
            && currentOperation == other.currentOperation;

        /// <inheritdoc />
        public override int GetHashCode()
        {
            var hashCode = 1919164243;
            hashCode = hashCode * -1521134295 + EqualityComparer<DatabaseLockMonitor>.Default.GetHashCode(lockMonitor);
            hashCode = hashCode * -1521134295 + currentOperation.GetHashCode();
            return hashCode;
        }

        /// <summary>
        /// Compares whether two <see cref="DatabaseLock"/> structs are equal.
        /// </summary>
        /// <param name="left">The first lock to compare to.</param>
        /// <param name="right">The second lock to compare with.</param>
        /// <returns>A value indicating whether the two locks are equal.</returns>
        public static bool operator ==(DatabaseLock left, DatabaseLock right)
            => left.Equals(right);

        /// <summary>
        /// Compares whether two <see cref="DatabaseLock"/> structs are unequal.
        /// </summary>
        /// <param name="left">The first lock to compare to.</param>
        /// <param name="right">The second lock to compare with.</param>
        /// <returns>A value indicating whether the two locks are unequal.</returns>
        public static bool operator !=(DatabaseLock left, DatabaseLock right)
            => !(left == right);
    }
}
