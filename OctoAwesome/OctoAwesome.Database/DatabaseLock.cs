using System;
using System.Collections.Generic;
using System.Text;

namespace OctoAwesome.Database
{
    public readonly struct DatabaseLock : IDisposable
    {
        public void Dispose() => throw new NotImplementedException();
    }
}
