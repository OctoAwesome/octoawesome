
using System.Collections.Generic;

namespace OctoAwesome.Extension
{
    public interface IExtensionRegistrar
    {
        string ChannelName { get; }

    }

    /// <summary>
    /// Interface for the generic Extension Loader.
    /// </summary>
    public interface IExtensionRegistrar<T> : IExtensionRegistrar
    {

        void Register(T value);

        void Unregister(T value);

        IReadOnlyCollection<T> Get();
    }
}
