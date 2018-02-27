using OctoAwesome.Basics.Entities;
using System.Reflection;
using System.Linq;
using System;
using OctoAwesome.Entities;
using OctoAwesome.Basics.EntityComponents;

namespace OctoAwesome.Basics
{
    public sealed class Extension : IExtension
    {
        public string Description => Languages.OctoBasics.ExtensionDescription;

        public string Name => Languages.OctoBasics.ExtensionName;

        public void Register(IExtensionLoader extensionLoader)
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes().Where(
                t => !t.IsAbstract && typeof(IDefinition).IsAssignableFrom(t)))
            {
                extensionLoader.RegisterDefinition((IDefinition) Activator.CreateInstance(t));
            }
            extensionLoader.RegisterMapGenerator(new ComplexPlanetGenerator());
            extensionLoader.RegisterMapPopulator(new TreePopulator());
            //extensionLoader.RegisterMapPopulator(new WauziPopulator());
            extensionLoader.RegisterEntity<WauziEntity>();
            extensionLoader.RegisterDefaultEntityExtender<WauziEntity>();
            extensionLoader.RegisterEntityExtender<Player>((player, service) =>
            {
                player.Components.AddComponent(new GroundPhysicComponent(player, service, 60f, 400f, 0.8f, 3.5f));
                InventoryComponent inventory = new InventoryComponent(player, service);
                player.Components.AddComponent(inventory); // -> sinvoll
                player.Components.AddComponent(new ToolBarComponent(player, service));   // -> sinvoll
            });
            extensionLoader.RegisterEntityExtender<WauziEntity>((p, service) =>
            {
                p.Components.AddComponent(new GroundPhysicComponent(p, service, 30f, 300f, 1.2f, 1.6f));
            });

            extensionLoader.RegisterSimulationExtender((s) =>
            {
                //s.Components.AddComponent(new GroundPhysicComponent(s.ResourceManager.DefinitionManager));
                //s.Components.AddComponent(new WattMoverComponent());
                //s.Components.AddComponent(new NewtonGravitatorComponent());
                //s.Components.AddComponent(new ForceAggregatorComponent());
                //s.Components.AddComponent(new PowerAggregatorComponent());
                //s.Components.AddComponent(new AccelerationComponent());
                //s.Components.AddComponent(new MoveComponent());
                //s.Components.AddComponent(new BlockInteractionComponent(s));
                //TODO: unschön
            });
        }
    }
}
