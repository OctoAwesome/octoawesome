using engenious;

using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;
using OctoAwesome.UI.Components;

using System;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Storage Interface Item for inventories.
    /// </summary>
    public class StorageInterfaceItem : Item
    {
        private readonly Entity storageInterfaceComponentContainer;
        private readonly InventoriesManagemendComponent inventoriesManagemendComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageInterfaceItem"/> class.
        /// </summary>
        /// <param name="definition">The StorageInterfaceItem definition.</param>
        /// <param name="materialDefinition">The material definition the storage interface is made out of.</param>
        public StorageInterfaceItem(StorageInterfaceItemDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            var screenComponent = TypeContainer.Get<IScreenComponent>();

            storageInterfaceComponentContainer = new Entity();
            inventoriesManagemendComponent = new InventoriesManagemendComponent();
            storageInterfaceComponentContainer.Components.Add(inventoriesManagemendComponent);
            storageInterfaceComponentContainer.Components.Add(new UiKeyComponent("StorageInterface"));

            screenComponent.Add(storageInterfaceComponentContainer);
        }

        /// <inheritdoc />
        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            //TODO How to open screen / have access to the interactor

        }

    }

    
}
