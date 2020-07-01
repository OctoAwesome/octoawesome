using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database.Threading
{
    public readonly struct DatabaseLock : IDisposable, IEquatable<DatabaseLock>
    {
        public static DatabaseLock Empty = default;

        public bool IsEmpty => this == default;

        private readonly DatabaseLockMonitor lockMonitor;
        private readonly Operation currentOperation;

        public DatabaseLock(DatabaseLockMonitor lockMonitor, Operation operation)
        {
            this.lockMonitor = lockMonitor;
            currentOperation = operation;
        }

        public void Enter()
        {
            lockMonitor.SetLock(currentOperation);
        }

        public void Dispose()
        {
            lockMonitor.ReleaseLock(currentOperation);
        }

        public override bool Equals(object obj) 
            => obj is DatabaseLock @lock && Equals(@lock);
        public bool Equals(DatabaseLock other) 
            => EqualityComparer<DatabaseLockMonitor>.Default.Equals(lockMonitor, other.lockMonitor) 
            && currentOperation == other.currentOperation;

        public override int GetHashCode()
        {
            var hashCode = 1919164243;
            hashCode = hashCode * -1521134295 + EqualityComparer<DatabaseLockMonitor>.Default.GetHashCode(lockMonitor);
            hashCode = hashCode * -1521134295 + currentOperation.GetHashCode();
            return hashCode;
        }

        public static bool operator ==(DatabaseLock left, DatabaseLock right) 
            => left.Equals(right);
        public static bool operator !=(DatabaseLock left, DatabaseLock right) 
            => !(left == right);
    }
}
