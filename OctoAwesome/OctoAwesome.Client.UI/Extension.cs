using engenious;

using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.UI
{
    //TODO: Perhaps outsource

    /// <summary>
    /// The base extension implementation.
    /// </summary>
    internal sealed class ClientUiExtension : IExtension
    {
        /// <inheritdoc />
        public string Description => "OctoAwesome Client Ui";

        /// <inheritdoc />
        public string Name => "OctoAwesome.Client.Ui";

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
            extensionLoader.RegisterTypesWithSerializationId(typeof(ClientUiExtension).Assembly);
        }
    }
}
