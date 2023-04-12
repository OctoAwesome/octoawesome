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

        private ITypeContainer TypeContainer
        {
            get => NullabilityHelper.NotNullAssert(typeContainer, $"{nameof(TypeContainer)} was not initialized!");
            set => typeContainer = NullabilityHelper.NotNullAssert(value, $"{nameof(TypeContainer)} cannot be initialized with null!");
        }

        private ITypeContainer? typeContainer;

        /// <inheritdoc />
        public void Register(ITypeContainer typeContainer)
        {
            typeContainer.Register<IMapGenerator, ComplexPlanetGenerator>();
            typeContainer.Register<IPlanet, ComplexPlanet>();
            this.typeContainer = typeContainer;
        }

        /// <inheritdoc />
        public void RegisterTypes(ExtensionService extensionLoader)
        {
            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!t.IsAbstract && t.IsPublic && typeof(IDefinition).IsAssignableFrom(t))
                    extensionLoader.Register(t, ChannelNames.Definitions);
            }
        }

        /// <inheritdoc />
        public void Register(ExtensionService extensionLoader)
        {

            extensionLoader.Register<IMapGenerator>(new ComplexPlanetGenerator());

            extensionLoader.Register<IMapPopulator>(new TreePopulator());
            extensionLoader.Register<IMapPopulator>(new WauziPopulator(TypeContainer.Get<IResourceManager>()));

            extensionLoader.RegisterTypesWithSerializationId(typeof(Extension).Assembly);
            //extensionLoader.Register(typeof(WauziEntity), ChannelNames.Serialization);
            //extensionLoader.Register(typeof(Chest), ChannelNames.Serialization);
            //extensionLoader.Register(typeof(Furnace), ChannelNames.Serialization);

            extensionLoader.Extend<WauziEntity>(wauziEntity => wauziEntity.RegisterDefault());

            extensionLoader.Extend<Player>((player) =>
            {
                var posComponent = new PositionComponent { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) };

                player.Components.AddIfTypeNotExists(posComponent);
                player.Components.AddIfTypeNotExists(new BodyComponent() { Mass = 50f, Height = 3.5f, Radius = 0.75f });
                player.Components.AddIfTypeNotExists(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
                player.Components.AddIfTypeNotExists(new GravityComponent());
                player.Components.AddIfTypeNotExists(new MoveableComponent());
                player.Components.AddIfTypeNotExists(new BoxCollisionComponent(Array.Empty<BoundingBox>()));
                player.Components.AddIfTypeNotExists(new EntityCollisionComponent());
                player.Components.AddIfTypeNotExists(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 4, 2));
                player.Components.AddIfTypeNotExists(new TransferComponent());
                player.Components.AddIfTypeNotExists(new UiMappingComponent() { });

            });

            extensionLoader.Extend<Chest>((chest) =>
            {
                var c = chest;

                if (!c.ContainsComponent<PositionComponent>())
                {
                    var pos = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0));
                    c.Components.AddIfTypeNotExists(new PositionComponent()
                    {
                        Position = pos
                    });

                }

                if (!c.Components.TryGet<AnimationComponent>(out var animationComponent))
                {
                    c.AnimationComponent = new AnimationComponent();
                    c.Components.AddIfTypeNotExists(c.AnimationComponent);
                }
                else
                    c.AnimationComponent = animationComponent;

                if (!c.Components.TryGet<InventoryComponent>(out var inventoryComponent))
                {
                    inventoryComponent = new InventoryComponent();
                    c.Components.AddIfTypeNotExists(inventoryComponent);
                }


                c.Components.AddIfNotExists(new UiKeyComponent("Transfer"));

                c.Components.AddIfNotExists(new BodyComponent() { Height = 0.4f, Radius = 0.2f });
                c.Components.AddIfNotExists(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0), new Vector3(1, 1, 1)) }));
                c.Components.AddIfNotExists(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchestmodel", BaseZRotation = -90 });
                c.Components.AddIfTypeNotExists(new UniquePositionComponent());

            });

            extensionLoader.Extend<Furnace>((furnace) =>
            {
                var f = furnace;

                if (!f.ContainsComponent<PositionComponent>())
                {
                    var pos = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0));
                    f.Components.AddIfTypeNotExists(new PositionComponent()
                    {
                        Position = pos
                    });

                }

                if (!f.Components.TryGet<AnimationComponent>(out var animationComponent))
                {
                    f.AnimationComponent = new AnimationComponent();
                    f.Components.Add(f.AnimationComponent);
                }
                else
                    f.AnimationComponent = animationComponent;

                if (!f.Components.TryGet<ProductionInventoriesComponent>(out var inventoryComponent))
                {
                    inventoryComponent = new ProductionInventoriesComponent(true, 1);
                    f.ProductionInventoriesComponent = inventoryComponent;
                    f.Components.AddIfTypeNotExists(inventoryComponent);
                }
                else
                    f.ProductionInventoriesComponent = inventoryComponent;


                while (inventoryComponent.InputInventory.Inventory.Count < 2)
                {
                    inventoryComponent.InputInventory.Add(new InventorySlot(inventoryComponent.InputInventory));
                }

                f.Components.Add(new BurningComponent());

                f.Components.AddIfNotExists(new UiKeyComponent("Furnace"));
                f.Components.AddIfNotExists(new BodyComponent() { Height = 2f, Radius = 1f });
                f.Components.AddIfNotExists(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }));
                f.Components.AddIfNotExists(new RenderComponent() { Name = "Furnace", ModelName = "furnace", TextureName = "furnacetext" });
                f.Components.AddIfTypeNotExists(new UniquePositionComponent() );

            });

            extensionLoader.Extend<Simulation>((s) =>
            {
                s.Components.AddIfTypeNotExists(new WattMoverComponent());
                s.Components.AddIfTypeNotExists(new NewtonGravitatorComponent());
                s.Components.AddIfTypeNotExists(new ForceAggregatorComponent());
                s.Components.AddIfTypeNotExists(new PowerAggregatorComponent());
                s.Components.AddIfTypeNotExists(new AccelerationComponent());
                s.Components.AddIfTypeNotExists(new MoveComponent());
                //TODO: Fix this
                s.Components.AddIfTypeNotExists(new BlockInteractionComponent(s, TypeContainer.Get<BlockCollectionService>()));

                //TODO: ugly
                //TODO: TypeContainer?
            });
            extensionLoader.Extend<IScreenComponent>((s) =>
            {
                s.Components.AddIfTypeNotExists(new TransferUIComponent());
                s.Add(TypeContainer.GetUnregistered<TransferScreen>());

                s.Components.AddIfTypeNotExists(new FurnaceUIComponent());
                s.Add(TypeContainer.GetUnregistered<FurnaceScreen>());
            });

        }
    }
}
