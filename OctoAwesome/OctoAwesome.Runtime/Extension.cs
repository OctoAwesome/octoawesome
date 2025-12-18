using engenious;

using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// The base extension implementation.
    /// </summary>
    internal sealed class RuntimeExtension : IExtension
    {
        /// <inheritdoc />
        public string Description => "OctoAwesome Runtime";

        /// <inheritdoc />
        public string Name => "OctoAwesome.Runtime";

        /// <inheritdoc />
        public void Register(OctoAwesome.Extension.ExtensionService extensionLoader)
        {
           
        }

        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer)
        {
        }

        /// <inheritdoc />
        public void RegisterTypes(ExtensionService extensionLoader)
        {
            extensionLoader.RegisterTypesWithSerializationId(typeof(RuntimeExtension).Assembly);
        }
    }
}
