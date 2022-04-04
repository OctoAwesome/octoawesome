using System;
using System.Collections.Generic;

namespace OctoAwesome.Database.Threading
{
    internal readonly struct DatabaseOperation : IDisposable, IEquatable<DatabaseOperation>
    {
        public static DatabaseOperation Empty = default;

        public bool IsEmpty => this == default;

        private readonly DatabaseLockMonitor lockMonitor;
        private readonly Operation currentOperation;

        public DatabaseOperation(DatabaseLockMonitor lockMonitor, Operation operation)
        {
            this.lockMonitor = lockMonitor;
            currentOperation = operation;
        }

        public void Dispose()
        {
            lockMonitor.StopOperation(currentOperation);
        }

        public override bool Equals(object? obj)
            => obj is DatabaseOperation @lock && Equals(@lock);
        public bool Equals(DatabaseOperation other)
            => EqualityComparer<DatabaseLockMonitor>.Default.Equals(lockMonitor, other.lockMonitor)
            && currentOperation == other.currentOperation;

        public override int GetHashCode()
        {
            var hashCode = 1919164243;
            hashCode = hashCode * -1521134295 + EqualityComparer<DatabaseLockMonitor>.Default.GetHashCode(lockMonitor);
            hashCode = hashCode * -1521134295 + currentOperation.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DatabaseOperation left, DatabaseOperation right)
            => left.Equals(right);
        public static bool operator !=(DatabaseOperation left, DatabaseOperation right)
            => !(left == right);
    }
}
