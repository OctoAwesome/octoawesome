using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    /// <summary>
    /// Base registrar for extension loading
    /// </summary>
    /// <typeparam name="T">The type of the instance to be held</typeparam>
    public abstract class BaseRegistrar<T> : IExtensionRegistrar<T>
    {
        /// <summary>
        /// The name to listen on
        /// </summary>
        public virtual string ChannelName => "";


        /// <summary>
        /// Get a List of <typeparamref name="T"/>
        /// </summary>
        /// <returns>List of <typeparamref name="T"/></returns>
        public abstract IReadOnlyCollection<T> Get();

        /// <summary>
        /// Registers a new <typeparamref name="T"/> to be extended.
        /// </summary>
        /// <param name="value"><typeparamref name="T"/> to register</param>
        public abstract void Register(T value);

        /// <summary>
        /// Removes an existing <typeparamref name="T"/>.
        /// </summary>
        /// <param name="value">The <typeparamref name="T"/> to unregister</param>
        public abstract void Unregister(T value);
    }
}
