using engenious;

using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Basics.SimulationComponents;
using OctoAwesome.Basics.UI.Components;
using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using System.Reflection;
using System;
using OctoAwesome.Extension;
using OctoAwesome.Services;
using OctoAwesome.UI.Components;

namespace OctoAwesome.Basics
{
    /// <summary>
    /// Extension implementation for this library(<see cref="OctoAwesome.Basics"/>).
    /// </summary>
    public sealed class Extension : IExtension
    {
        /// <inheritdoc />
        public string Description => Languages.OctoBasics.ExtensionDescription;

        /// <inheritdoc />
        public string Name => Languages.OctoBasics.ExtensionName;


        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<IPlanet, ComplexPlanet>();
        }

        /// <inheritdoc />
        public void Register(ExtensionService extensionLoader)
        {

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!t.IsAbstract && t.IsPublic && typeof(IDefinition).IsAssignableFrom(t))
                    extensionLoader.Register(t, ChannelNames.Definitions);
            }

            extensionLoader.Register<IMapGenerator>(new ComplexPlanetGenerator());

            extensionLoader.Register<IMapPopulator>(new TreePopulator());
            extensionLoader.Register<IMapPopulator>(new WauziPopulator(TypeContainer.Get<IResourceManager>()));

            extensionLoader.Register(typeof(WauziEntity), ChannelNames.Serialization);
            extensionLoader.Register(typeof(Chest), ChannelNames.Serialization);
            extensionLoader.Register(typeof(Furnace), ChannelNames.Serialization);

            extensionLoader.Extend<WauziEntity>(wauziEntity => wauziEntity.RegisterDefault());

            extensionLoader.Extend<Player>((player) =>
            {
                var posComponent = new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) };

                player.Components.AddComponent(posComponent);
                player.Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 3.5f, Radius = 0.75f });
                player.Components.AddComponent(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
                player.Components.AddComponent(new GravityComponent());
                player.Components.AddComponent(new MoveableComponent());
                player.Components.AddComponent(new BoxCollisionComponent(Array.Empty<BoundingBox>()));
                player.Components.AddComponent(new EntityCollisionComponent());
                player.Components.AddComponent(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 4, 2));
                player.Components.AddComponent(new TransferComponent());
                player.Components.AddComponent(new UiMappingComponent() { });

            });

            extensionLoader.Extend<Chest>((chest) =>
            {
                var c = chest;

                if (!c.ContainsComponent<PositionComponent>())
                {
                    var pos = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0));
                    c.Components.AddComponent(new PositionComponent()
                    {
                        Position = pos
                    });

                }

                if (!c.Components.TryGetComponent<AnimationComponent>(out var animationComponent))
                {
                    c.AnimationComponent = new AnimationComponent();
                    c.Components.AddComponent(c.AnimationComponent);
                }
                else
                    c.AnimationComponent = animationComponent;

                if (!c.Components.TryGetComponent<InventoryComponent>(out var inventoryComponent))
                {
                    inventoryComponent = new InventoryComponent();
                    c.Components.AddComponent(inventoryComponent);
                }


                c.Components.AddComponentNoneOfTypePresent(new UiKeyComponent() { PrimaryKey = "Transfer" });

                c.Components.AddComponentNoneOfTypePresent(new BodyComponent() { Height = 0.4f, Radius = 0.2f });
                c.Components.AddComponentNoneOfTypePresent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0), new Vector3(1, 1, 1)) }));
                c.Components.AddComponentNoneOfTypePresent(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchestmodel", BaseZRotation = -90 });

            });

            extensionLoader.Extend<Furnace>((furnace) =>
            {
                var f = furnace;

                if (!f.ContainsComponent<PositionComponent>())
                {
                    var pos = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0));
                    f.Components.AddComponent(new PositionComponent()
                    {
                        Position = pos
                    });

                }

                if (!f.Components.TryGetComponent<AnimationComponent>(out var animationComponent))
                {
                    f.AnimationComponent = new AnimationComponent();
                    f.Components.AddComponent(f.AnimationComponent);
                }
                else
                    f.AnimationComponent = animationComponent;

                if (!f.Components.TryGetComponent<ProductionInventoriesComponent>(out var inventoryComponent))
                {
                    inventoryComponent = new ProductionInventoriesComponent(true, 1);
                    f.productionInventoriesComponent = inventoryComponent;
                    f.Components.AddComponent(inventoryComponent);
                }
                else
                    f.productionInventoriesComponent = inventoryComponent;


                while (inventoryComponent.InputInventory.Inventory.Count < 2)
                {
                    inventoryComponent.InputInventory.Add(new InventorySlot(inventoryComponent.InputInventory));
                }

                f.Components.AddComponent(new BurningComponent());

                f.Components.AddComponentNoneOfTypePresent(new UiKeyComponent() { PrimaryKey = "Furnace" });
                f.Components.AddComponentNoneOfTypePresent(new BodyComponent() { Height = 2f, Radius = 1f });
                f.Components.AddComponentNoneOfTypePresent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }));
                f.Components.AddComponentNoneOfTypePresent(new RenderComponent() { Name = "Furnace", ModelName = "furnace", TextureName = "furnacetext" });

            });

            extensionLoader.Extend<Simulation>((s) =>
            {
                s.Components.AddComponent(new WattMoverComponent());
                s.Components.AddComponent(new NewtonGravitatorComponent());
                s.Components.AddComponent(new ForceAggregatorComponent());
                s.Components.AddComponent(new PowerAggregatorComponent());
                s.Components.AddComponent(new AccelerationComponent());
                s.Components.AddComponent(new MoveComponent());
                //TODO: Fix this
                s.Components.AddComponent(new BlockInteractionComponent(s, TypeContainer.Get<BlockCollectionService>()));

                //TODO: ugly
                //TODO: TypeContainer?
            });
            extensionLoader.Extend<IScreenComponent>((s) =>
            {
                s.Components.AddComponent(new TransferUIComponent());
                s.Add(TypeContainer.GetUnregistered<TransferScreen>());

                s.Components.AddComponent(new FurnaceUIComponent());
                s.Add(TypeContainer.GetUnregistered<FurnaceScreen>());
            });

        }
    }
}
