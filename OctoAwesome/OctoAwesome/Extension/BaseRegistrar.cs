using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome
{
    public abstract class BaseRegistrar<TRegister, TUnregister> : IExtensionRegistrar<TRegister, TUnregister>
    {
        private readonly ISettings settings;

        public List<IExtension> LoadedExtensions { get; }
        public List<IExtension> ActiveExtensions { get; }

        public BaseRegistrar(ISettings settings)
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

        public abstract void Register(TRegister value);
        public abstract void Unregister(TUnregister value);
    }
}
