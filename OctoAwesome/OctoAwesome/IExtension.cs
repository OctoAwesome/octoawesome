using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Interface for all Mod Plugin Extensions.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets the Extension Name.
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Gets the Extension Description.
        /// </summary>
        string Description { get; }
        /// <summary>
        /// Load needed Definitions
        /// </summary>
        /// <param name="extensionloader">ExtensionsLoader</param>
        void LoadDefinitions(IExtensionLoader extensionloader);
        /// <summary>
        /// Register the Components in the ExtensionsLoader
        /// </summary>
        /// <param name="extensionloader">ExtensionsLoader</param>
        void Extend(IExtensionLoader extensionloader);
        
    }
}
