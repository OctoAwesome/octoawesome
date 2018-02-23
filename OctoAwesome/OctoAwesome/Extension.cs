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
        public string Description => "OctoAwesome default Extions.";

        public string Name => "OctoAwesome Extension";

        public void Register(IExtensionLoader extensionLoader)
        {
            extensionLoader.RegisterEntityExtender<Player>((p) =>
            {
                p.Components.AddComponent(new ControllerComponent(p));
                p.Components.AddComponent(new HeadComponent(p) { Offset = new Vector3(0, 0, 3.2f) });
                p.Components.AddComponent(new InventoryComponent(p));
                p.Components.AddComponent(new ToolBarComponent(p));
            });
        }
    }
}
