
using System.Collections.Generic;

namespace OctoAwesome.Extension
{
    /// <summary>
    /// Base interface for extension registrars
    /// </summary>
    public interface IExtensionRegistrar
    {
        /// <summary>
        /// The name to listen on
        /// </summary>
        public string ChannelName => "";

    }

    /// <summary>
    /// Interface for the generic Extension Loader.
    /// </summary>
    public interface IExtensionRegistrar<T> : IExtensionRegistrar
    {

        /// <summary>
        /// Adds a new <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Instance of <typeparamref name="T"/> to be added</param>
        void Register(T value);

        /// <summary>
        /// Removes an existing <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">Intance of <typeparamref name="T"/> to remove</param>
        void Unregister(T value);

        /// <summary>
        /// Return a List of <typeparamref name="T"/>
        /// </summary>
        /// <returns>List of <typeparamref name="T"/></returns>
        IReadOnlyCollection<T> Get();
    }
}
