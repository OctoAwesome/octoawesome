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
    //TODO:Eventuell auslagern

    internal sealed class CoreExtension : IExtension
    {
        public string Description => "OctoAwesome";

        public string Name => "OctoAwesome";

        public void Register(ExtensionService extensionLoader)
        {
            extensionLoader.Extend<Player>((player) =>
            {
                player.Components.AddComponent(new ControllableComponent());
                player.Components.AddComponent(new HeadComponent() { Offset = new Vector3(0, 0, 3.2f) });
                player.Components.AddComponent(new InventoryComponent());
                player.Components.AddComponent(new ToolBarComponent());
            });
        }

        public void Register(ITypeContainer typeContainer) { }
    }
}
