
using System;

namespace OctoAwesome
{
    public interface IExtensionExtender<TExtensionType> : IExtensionLoader
    {
        void Extend<T>(T instance) where T : TExtensionType;
        void RegisterExtender<T>(Action<T> extenderDelegate) where T : TExtensionType;
    }
}
