
using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public abstract class BaseExtensionExtender<TExtensionType> : IExtensionExtender<TExtensionType>
    {
        public abstract void RegisterExtender<T>(Action<T> extenderDelegate) where T : TExtensionType;

        public abstract void Execute<T>(T instance) where T : TExtensionType;
    }
}
