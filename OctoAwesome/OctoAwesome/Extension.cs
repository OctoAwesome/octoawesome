using engenious;

using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

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
                player.Components.AddIfTypeNotExists(new ControllableComponent());
                player.Components.AddIfTypeNotExists(new HeadComponent() { Offset = new Vector3(0, 0, 3.2f) });
                player.Components.AddIfTypeNotExists(new InventoryComponent(true, 120));
                player.Components.AddIfTypeNotExists(new ToolBarComponent());
            });
        }

        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer)
        {
            var changedHandler = typeContainer.Get<ComponentChangedNotificationHandler>();
            var uh = typeContainer.Get<IUpdateHub>();
            var cc = new ComponentChangeContainer(uh);

            changedHandler.Register("PositionComponent", cc.PositionChanged);
            changedHandler.Register("AnimationComponent", cc.AnimationChanged);
            changedHandler.Register("InventoryComponent", cc.InventoryChanged);
        }

        /// <inheritdoc />
        public void RegisterTypes(ExtensionService extensionLoader)
        {
            extensionLoader.RegisterTypesWithSerializationId(typeof(CoreExtension).Assembly);
        }
    }
}
