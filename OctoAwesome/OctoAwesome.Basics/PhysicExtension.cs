using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.EntityComponents;
namespace OctoAwesome.Basics.Extensions
{
    public sealed class PhysicExtension : IExtension
    {
        public string Name => Languages.OctoBasics.PhysicExtensionName;

        public string Description => Languages.OctoBasics.PhysicExtensionDescription;

        public void LoadDefinitions(IExtensionLoader extensionloader)
        {
        }
        public void Extend(IExtensionLoader extensionLoader)
        {
            extensionLoader.RegisterEntityExtender<Player>((player, service) =>
            {
                player.Components.AddComponent(new GroundPhysicComponent(player, service, 60f, 400f, 0.8f, 3.5f));
                InventoryComponent inventory = new InventoryComponent(player, service);
                player.Components.AddComponent(inventory);
                player.Components.AddComponent(new ToolBarComponent(player, service));
            });
            extensionLoader.RegisterEntityExtender<WauziEntity>((entity, service) =>
            {
                entity.Components.AddComponent(new GroundPhysicComponent(entity, service, 30f, 300f, 1.2f, 1.6f));
            });
        }
    }
}
