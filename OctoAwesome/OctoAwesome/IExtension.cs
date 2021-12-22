using OctoAwesome.Extension;

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
        /// Register the Components in the ExtensionsLoader
        /// </summary>
        /// <param name="extensionService">ExtensionsLoader</param>
        void Register(ExtensionService extensionService);
        void Register(ITypeContainer typeContainer);
    }
}
