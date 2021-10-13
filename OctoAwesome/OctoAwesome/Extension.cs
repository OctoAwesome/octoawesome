using engenious;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    //TODO:Eventuell auslagern

    public sealed class Extension : IExtension
    {
        public string Description => "OctoAwesome";

        public string Name => "OctoAwesome";

        public void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer)
        {
            extensionLoader.RegisterEntityExtender<Player>((player) =>
            {
                var p = (Player)player;
                p.Components.AddComponent(new ControllableComponent());
                p.Components.AddComponent(new HeadComponent() { Offset = new Vector3(0, 0, 3.2f) });
                p.Components.AddComponent(new InventoryComponent());
                p.Components.AddComponent(new ToolBarComponent());
            });
        }

        public void Register(ITypeContainer typeContainer) { }
    }
}
