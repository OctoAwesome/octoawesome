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

        void Register(IExtensionLoader extensionLoader);
    }
}
