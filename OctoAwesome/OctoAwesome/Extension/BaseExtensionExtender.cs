
using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Extension
{
    /// <summary>
    /// The base class for instance extender
    /// </summary>
    /// <typeparam name="TExtensionType">The base type for the instances</typeparam>
    public abstract class BaseExtensionExtender<TExtensionType> : IExtensionExtender<TExtensionType>
    {
        ///  <inheritdoc/>
        public abstract void RegisterExtender<T>(Action<T> extenderDelegate) where T : TExtensionType;

        ///  <inheritdoc/>
        public abstract void Execute<T>(T instance) where T : TExtensionType;
    }
}
