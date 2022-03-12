
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Extension
{
    public abstract class BaseRegistrar<T> : IExtensionRegistrar<T>
    {
        public virtual string ChannelName => "";

        public abstract IReadOnlyCollection<T> Get();
        public abstract void Register(T value);
        public abstract void Unregister(T value);
    }
}
