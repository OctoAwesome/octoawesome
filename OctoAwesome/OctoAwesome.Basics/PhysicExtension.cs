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
            extensionLoader.RegisterEntityExtender<Player>((player) =>
            {
                player.Components.AddComponent(new GroundPhysicComponent(60f, 400f, 0.8f, 3.5f));
            });
            extensionLoader.RegisterEntityExtender<WauziEntity>((entity) =>
            {
                entity.Components.AddComponent(new GroundPhysicComponent(30f, 300f, 1.2f, 1.6f));
            });
        }
    }
}
