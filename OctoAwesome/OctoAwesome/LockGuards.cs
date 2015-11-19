using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace OctoAwesome
{
    // Autor: http://www.codeproject.com/Tips/661975/ReaderWriterLockSlim-and-the-IDisposable-Wrapper
    public class ReadGuard : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLock;
        public ReadGuard(ReaderWriterLockSlim readerWriterLock)
        {
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterReadLock();
        }
        public void Dispose()
        {
            _readerWriterLock.ExitReadLock();
        }
    }

    public class UpgradeableGuard : IDisposable
    {
        private readonly ReaderWriterLockSlim _readerWriterLock;
        private UpgradedGuard _upgradedLock;
        public UpgradeableGuard(ReaderWriterLockSlim readerWriterLock)
        {
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterUpgradeableReadLock();
        }
        public IDisposable UpgradeToWriterLock()
        {
            if (_upgradedLock == null)
            {
                _upgradedLock = new UpgradedGuard(this);
            }
            return _upgradedLock;
        }
        public void Dispose()
        {
            if (_upgradedLock != null)
            {
                _upgradedLock.Dispose();
            }
            _readerWriterLock.ExitUpgradeableReadLock();
        }

        private class UpgradedGuard : IDisposable
        {
            private UpgradeableGuard _parentGuard;
            private WriteGuard _writerLock;
            public UpgradedGuard(UpgradeableGuard parentGuard)
            {
                _parentGuard = parentGuard;
                _writerLock = new WriteGuard(_parentGuard._readerWriterLock);
            }
            public void Dispose()
            {
                _writerLock.Dispose();
                _parentGuard._upgradedLock = null;
            }
        }
    }

    public class WriteGuard : IDisposable
    {
        private ReaderWriterLockSlim _readerWriterLock;
        private bool IsDisposed { get { return _readerWriterLock == null; } }
        public WriteGuard(ReaderWriterLockSlim readerWriterLock)
        {
            _readerWriterLock = readerWriterLock;
            _readerWriterLock.EnterWriteLock();
        }
        public void Dispose()
        {
            if (IsDisposed)
                throw new ObjectDisposedException(this.ToString());
            _readerWriterLock.ExitWriteLock();
            _readerWriterLock = null;
        }
    }
}
