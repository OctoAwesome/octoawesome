
using System;

namespace OctoAwesome.Extension
{
    /// <summary>
    /// The base extension extender interface
    /// </summary>
    public interface IExtensionExtender
    {

    }

    /// <summary>
    /// The base extension extender with a <typeparamref name="TExtensionType"/> type and more base methods
    /// </summary>
    /// <typeparam name="TExtensionType"></typeparam>
    public interface IExtensionExtender<TExtensionType> : IExtensionExtender
    {
        /// <summary>
        /// Extend a <typeparamref name="TExtensionType"/>.
        /// </summary>
        /// <param name="instance">The instance to extend</param>
        void Execute<T>(T instance) where T : TExtensionType;

        /// <summary>
        /// Adds a new Extender for the <typeparamref name="TExtensionType"/>.
        /// </summary>
        /// <param name="extenderDelegate">The action that will be invoked for an instance of <typeparamref name="T"/></param>
        void RegisterExtender<T>(Action<T> extenderDelegate) where T : TExtensionType;
    }
}
