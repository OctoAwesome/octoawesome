using engenious;
using Newtonsoft.Json;
using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Basics.SimulationComponents;
using OctoAwesome.Basics.UI.Components;
using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Graphs;
using OctoAwesome.Location;
using OctoAwesome.Rx;
using OctoAwesome.Services;
using OctoAwesome.UI.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;

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
            string ConvertName(string name)
            {
                name
                    = name
                    .Replace("Definition", "");
                StringBuilder sb = new();
                int currIndex = 0;
                foreach (var letter in name)
                {
                    if (char.IsUpper(letter))
                    {
                        sb.Insert(0, '_');
                        sb.Insert(1, letter);
                        currIndex = 1;
                    }
                    else
                        sb.Insert(currIndex, letter);
                    currIndex++;
                }
                return "base" + sb.ToString().ToLower();
            }

            var folder = "F:\\Projekte\\Visual 2019\\OctoAwesome\\OctoAwesome\\temp";
            var list = new Dictionary<string, IDefinition>();

            foreach (var t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (!t.IsAbstract && t.IsPublic && typeof(IDefinition).IsAssignableFrom(t))
                {
                    try
                    {

                        var constructor = t.GetConstructors().First();

                        if (constructor is null)
                        {

                            list.Add(ConvertName(t.Name), (IDefinition)Activator.CreateInstance(t));
                        }
                        else
                        {
                            var parameters
                                = constructor
                                    .GetParameters()
                                    .Select<ParameterInfo, object>(p => Activator.CreateInstance(p.ParameterType))
                                    .ToArray();

                            list.Add(ConvertName(t.Name), (IDefinition)constructor.Invoke(parameters));

                        }
                    }
                    catch (Exception ex)
                    {
                        ;
                    }


                }
                //extensionLoader.Register(t, ChannelNames.Definitions);
            }

            try
            {
                File.WriteAllText(Path.Combine(folder, "definitions.json"), JsonConvert.SerializeObject(list, Formatting.Indented));

            }
            catch (Exception ex)
            {
                ;
            }


            ;

        }

        /// <inheritdoc />
        public void Register(ExtensionService extensionLoader)
        {
            extensionLoader.Register<IMapGenerator>(new ComplexPlanetGenerator());

            extensionLoader.Register<IMapPopulator>(new TreePopulator());
            extensionLoader.Register<IMapPopulator>(new WauziPopulator(TypeContainer.Get<IResourceManager>()));

            extensionLoader.RegisterTypesWithSerializationId(typeof(Extension).Assembly);

            extensionLoader.Register(new TypeDefinitionRegistration("core.block", typeof(BlockDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material", typeof(MaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_fluid", typeof(FluidMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_gas", typeof(GasMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_food", typeof(FoodMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_solid", typeof(SolidMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.item", typeof(ItemDefinition)));

            Extend(extensionLoader);
            RegisterInteracts();
        }

        private void RegisterInteracts()
        {
            var interactService = typeContainer.Get<InteractService>();
            var blockInteractionService = typeContainer.Get<BlockInteractionService>();
            interactService.Register(nameof(Chest), (gt, interactor, target) =>
            {
                if (interactor.TryGetComponent<TransferComponent>(out var transferComponent)
                   && interactor.TryGetComponent<UiMappingComponent>(out var lastUiMappingComponent)
                   && target.TryGetComponent<InventoryComponent>(out var inventoryComponent)
                   && target.TryGetComponent<UiKeyComponent>(out var uiKeyComp)
                   && target.TryGetComponent<AnimationComponent>(out var animationComponent))
                {
                    transferComponent.Targets.Clear();
                    transferComponent.Targets.Add(inventoryComponent);
                    lastUiMappingComponent.Changed.OnNext((interactor, uiKeyComp.PrimaryKey, true));
                    lastUiMappingComponent.Changed.SubscribeOnce(UiComponentChanged);

                    void UiComponentChanged((ComponentContainer, string, bool show) e)
                    {
                        if (e.show)
                            return;
                        animationComponent.AnimationSpeed = -60f;
                    }

                    animationComponent.CurrentTime = 0f;
                    animationComponent.AnimationSpeed = 60f;
                }
            });

            interactService.Register(nameof(Furnace), (gt, interactor, target) =>
            {
                if (target.TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent)
                    && interactor.TryGetComponent<TransferComponent>(out var transferComponent)
                    && interactor.TryGetComponent<UiMappingComponent>(out var uiMappingComponent)
                    && target.TryGetComponent<ProductionInventoriesComponent>(out var productionInventoryCompopnents))
                {
                    transferComponent.Targets.Clear();
                    transferComponent.Targets.Add(productionInventoryCompopnents.InputInventory);
                    transferComponent.Targets.Add(productionInventoryCompopnents.OutputInventory);
                    transferComponent.Targets.Add(productionInventoryCompopnents.ProductionInventory);
                    uiMappingComponent.Changed.OnNext((interactor, ownUiKeyComponent.PrimaryKey, true));
                }
            });

            interactService.Register("", (gt, interactor, target) =>
            {
                if (target.IsEmpty || target.Block == 0)
                    return;
                var sim = interactor.Simulation;
                if (sim is null)
                    return;
                var posComp = interactor.GetComponent<PositionComponent>();

                if (sim.ResourceManager.Pencils.TryGetValue(posComp.Position.Planet, out var pencil))
                {
                    pencil.InteractNode(target.Position);

                }
            });



            interactService.Register("Storage Interface", (gt, interactor, target) =>
            {
                if (target.IsEmpty || target.Block == 0)
                    return;
                var sim = interactor.Simulation;
                if (sim is null)
                    return;
                var posComp = interactor.GetComponent<PositionComponent>();

                if (sim.ResourceManager.Pencils.TryGetValue(posComp.Position.Planet, out var pencil))
                {
                    if (//target.TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent) &&

                         interactor.TryGetComponent<TransferComponent>(out var transferComponent)
                         && interactor.TryGetComponent<UiMappingComponent>(out var uiMappingComponent)
                        // && target.TryGetComponent<ProductionInventoriesComponent>(out var productionInventoryCompopnents)
                        )
                    {
                        transferComponent.Targets.Clear();
                        foreach (var item in pencil.Graphs.OfType<ItemGraph>())
                        {
                            if (!item.TryGetNode(target.Position, out _))
                                continue;

                            foreach (var source in item.Sources.OrderBy(x => x.Priority))
                            {
                                var cap = source.GetCapacity(sim);
                                if (cap.Data.IsEmpty)
                                    continue;
                                cap.Data.Visit(single => transferComponent.Targets.Add(single), multi => multi.ForEach(x => transferComponent.Targets.Add(x)));

                            }

                            break;

                        }
                        uiMappingComponent.Changed.OnNext((interactor, "StorageInterface", true));
                    }
                }
            });
        }

        private void Extend(ExtensionService extensionLoader)
        {
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
                player.Components.AddIfNotExists(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 });

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

                var uiKeyComp = new UiKeyComponent("Transfer");
                c.Components.AddIfNotExists(uiKeyComp);

                c.Components.AddIfNotExists(new BodyComponent() { Height = 0.4f, Radius = 0.2f });
                c.Components.AddIfNotExists(new BoxCollisionComponent([new BoundingBox(new Vector3(0, 0), new Vector3(1, 1, 1))]));
                c.Components.AddIfNotExists(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchestmodel", BaseZRotation = -90 });
                c.Components.AddIfTypeNotExists(new UniquePositionComponent());
                c.Components.AddIfTypeNotExists(new InteractKeyComponent { Key = nameof(Chest) });
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
                f.Components.AddIfNotExists(new BoxCollisionComponent([new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1))]));
                f.Components.AddIfNotExists(new RenderComponent() { Name = "Furnace", ModelName = "furnace", TextureName = "furnacetext" });
                f.Components.AddIfTypeNotExists(new UniquePositionComponent());
                f.Components.AddIfTypeNotExists(new InteractKeyComponent { Key = nameof(Furnace) });



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
                s.Components.AddIfTypeNotExists(new BlockInteractionComponent(s, TypeContainer.Get<BlockInteractionService>(), TypeContainer.Get<InteractService>()));

                //TODO: ugly
                //TODO: TypeContainer?
            });
            extensionLoader.Extend<IScreenComponent>((s) =>
            {
                s.Components.AddIfTypeNotExists(new TransferUIComponent());
                s.Add(TypeContainer.GetUnregistered<TransferScreen>());

                s.Components.AddIfTypeNotExists(new FurnaceUIComponent());
                s.Add(TypeContainer.GetUnregistered<FurnaceScreen>());

                s.Components.AddIfTypeNotExists(new StorageInterfaceUIComponent());
                s.Add(TypeContainer.GetUnregistered<StorageInterfaceScreen>());
            });
        }
    }
}
