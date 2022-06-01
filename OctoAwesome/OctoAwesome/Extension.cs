using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    //TODO: Perhaps outsource

    /// <summary>
    /// The base extension implementation.
    /// </summary>
    internal sealed class CoreExtension : IExtension
    {
        /// <inheritdoc />
        public string Description => "OctoAwesome";

        /// <inheritdoc />
        public string Name => "OctoAwesome";

        /// <inheritdoc />
        public void Register(OctoAwesome.Extension.ExtensionService extensionLoader)
        {
            extensionLoader.Extend<Player>((player) =>
            {
                player.Components.AddComponent(new ControllableComponent());
                player.Components.AddComponent(new HeadComponent() { Offset = new Vector3(0, 0, 3.2f) });
                player.Components.AddComponent(new InventoryComponent(true, 40));
                player.Components.AddComponent(new ToolBarComponent());
            });
        }

        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer) { }
    }
}
