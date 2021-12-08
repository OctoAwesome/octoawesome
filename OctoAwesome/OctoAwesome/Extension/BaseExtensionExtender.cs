
using System;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public abstract class BaseExtensionExtender<TExtensionType> : IExtensionExtender<TExtensionType>
    {
        private readonly ISettings settings;

        public List<IExtension> LoadedExtensions { get; }
        public List<IExtension> ActiveExtensions { get; }

        public BaseExtensionExtender(ISettings settings)
        {
            this.settings = settings;
        }

        /// <summary>
        /// Activate the Extenisons
        /// </summary>
        /// <param name="disabledExtensions">List of Extensions</param>
        public void Apply(IList<IExtension> disabledExtensions)
        {
            var types = disabledExtensions.Select(e => e.GetType().FullName).ToArray();
            settings.Set(IExtensionLoader.SETTINGSKEY, types);
        }

        public abstract void RegisterExtender<T>(Action<T> extenderDelegate) where T : TExtensionType;

        public abstract void Extend<T>(T instance) where T : TExtensionType;
    }
}
