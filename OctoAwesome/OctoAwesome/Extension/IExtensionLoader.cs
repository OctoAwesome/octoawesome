using OctoAwesome.Definitions.Items;

using System.Collections.Generic;
using System.Reflection.Emit;

namespace OctoAwesome
{

    /// <summary>
    /// Interface for the Extension Loader.
    /// </summary>
    public interface IExtensionLoader
    {
        public const string SETTINGSKEY = "DisabledExtensions";

        /// <summary>
        /// List of Loaded Extensions
        /// </summary>
        List<IExtension> LoadedExtensions { get; }

        /// <summary>
        /// List of active Extensions
        /// </summary>
        List<IExtension> ActiveExtensions { get; }

        /// <summary>
        /// Activate the Extenisons
        /// </summary>
        /// <param name="extensions">List of Extensions</param>
        void Apply(IList<IExtension> extensions);


    }
}
