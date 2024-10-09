using engenious;

using Newtonsoft.Json;
using OctoAwesome.Basics.Definitions.Blocks;
using OctoAwesome.Basics.Definitions.Items;
using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Basics.Definitions.Trees;
using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Basics.SimulationComponents;
using OctoAwesome.Basics.UI.Components;
using OctoAwesome.Basics.UI.Screens;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.Graphs;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Rx;
using OctoAwesome.Services;
using OctoAwesome.UI.Components;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;

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
            RegisterTypeDefinitions(extensionLoader);

            var defActionService = typeContainer.Get<DefinitionActionService>();
            RegisterPlantTree(defActionService);
            RegisterTextureIndex(defActionService);
            RegisterTextureRotations(defActionService);
            RegisterNodeTypes(defActionService);
            RegisterCanMines(defActionService);
            RegisterCreateItem(defActionService);
        }

        private static void RegisterTypeDefinitions(ExtensionService extensionLoader)
        {
            extensionLoader.Register(new TypeDefinitionRegistration("core.block", typeof(BlockDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.networkblock", typeof(NetworkBlockDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.burnable", typeof(BurnableDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material", typeof(MaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_fluid", typeof(FluidMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_gas", typeof(GasMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_food", typeof(FoodMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.material_solid", typeof(SolidMaterialDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.item", typeof(ItemDefinition)));
            extensionLoader.Register(new TypeDefinitionRegistration("core.tree", typeof(TreeDefinition)));
        }

        private void RegisterPlantTree(DefinitionActionService defActionService)
        {
            var plantTree = typeContainer.GetUnregistered<PlantTree>();
            defActionService.Register("PlantTree", "base_tree_birch@core.tree", plantTree.Birch);
            defActionService.Register("PlantTree", "base_tree_spruce@core.tree", plantTree.Spruce);
            defActionService.Register("PlantTree", "base_tree_oak@core.tree", plantTree.Oak);
            defActionService.Register("PlantTree", "base_tree_cactus@core.tree", plantTree.Cactus);
        }

        private void RegisterTextureIndex(DefinitionActionService defActionService)
        {
            var blockTextureIndex = typeContainer.GetUnregistered<BlockTextureIndex>();
            defActionService.Register("GetTextureIndex", "base_block_battery@core.block", blockTextureIndex.BatteryBlock);
            defActionService.Register("GetTextureIndex", "base_block_birch_wood@core.block", blockTextureIndex.Wood);
            defActionService.Register("GetTextureIndex", "base_block_cactus@core.block", blockTextureIndex.Cactus);
            defActionService.Register("GetTextureIndex", "base_block_grass@core.block", blockTextureIndex.Grass);
            defActionService.Register("GetTextureIndex", "base_block_light@core.block", blockTextureIndex.Light);
            defActionService.Register("GetTextureIndex", "base_block_red@core.block", blockTextureIndex.Red);
            defActionService.Register("GetTextureIndex", "base_block_snow@core.block", blockTextureIndex.Snow);
            defActionService.Register("GetTextureIndex", "base_block_wood@core.block", blockTextureIndex.Wood);
        }

        private void RegisterTextureRotations(DefinitionActionService defActionService)
        {
            var blockTextureRotation = typeContainer.GetUnregistered<BlockTextureRotation>();
            defActionService.Register("GetTextureRotation", "base_block_birch_wood@core.block", blockTextureRotation.Wood);
            defActionService.Register("GetTextureRotation", "base_block_cactus@core.block", blockTextureRotation.Cactus);
            defActionService.Register("GetTextureRotation", "base_block_wood@core.block", blockTextureRotation.Wood);
        }
        private void RegisterNodeTypes(DefinitionActionService defActionService)
        {
            var blockTextureRotation = typeContainer.GetUnregistered<BlockTextureRotation>();
            defActionService.Register("CreateNode", "base_block_signaler@core.networkblock", (NodeBase? _, IDefinition _) => new SignalerBlockNode());
            defActionService.Register("CreateNode", "base_block_battery@core.networkblock", (NodeBase? _, IDefinition _) => new BatteryNode());
            defActionService.Register("CreateNode", "base_block_cactus@core.networkblock", (NodeBase? _, IDefinition _) => new CactusBlockNode());
            defActionService.Register("CreateNode", "base_cable_item@core.networkblock", (NodeBase? _, IDefinition _) => new ItemCableNode());
            defActionService.Register("CreateNode", "base_block_source_item@core.networkblock", (NodeBase? _, IDefinition _) => new ItemSourceBlockNode());
            defActionService.Register("CreateNode", "base_block_target_item@core.networkblock", (NodeBase? _, IDefinition _) => new ItemTargetBlockNode());
            defActionService.Register("CreateNode", "base_block_light@core.networkblock", (NodeBase? _, IDefinition _) => new LightNode());
            defActionService.Register("CreateNode", "base_block_cable_power@core.networkblock", (NodeBase? _, IDefinition _) => new PowerCableNode());
            defActionService.Register("CreateNode", "base_block_plate_pressure@core.networkblock", (NodeBase? _, IDefinition _) => new PressurePlateBlockNode());
            defActionService.Register("CreateNode", "base_cable_signal@core.networkblock", (NodeBase? _, IDefinition _) => new SignalCableNode());

        }

        private void RegisterCanMines(DefinitionActionService defActionService)
        {
            var canMines = typeContainer.GetUnregistered<CanMineMaterial>();
            defActionService.Register("CanMineMaterial", "base_hand@core.item", canMines.CanMineEverything);

            defActionService.RegisterMultiple("CanMineMaterial", canMines.CanMineMaterialSolid, "base_axe@core.item", "base_pickaxe@core.item", "base_shover@core.item");
            defActionService.Register("CanMineMaterial", "base_bucket", canMines.CanMineMaterialFluid);
            defActionService.RegisterMultiple("CanMineMaterial", canMines.CanMineNothing, "base_item_chest@core.item", "base_item_furnace@core.item", "base_hammer@core.item", "base_hoe@core.item", "base_item_storage_interface@core.item", "base_sword@core.item", "base_item_wauzi@core.item", "base_item_meat_cooked@core.item", "base_item_meat_raw@core.item");
        }

        private void RegisterCreateItem(DefinitionActionService defActionService)
        {

            //TODO Filter for correct material definitions, so that we don't end up with meat chest and wood meat
            //TODO 2 Do sth. about the method name strings, const somewhere probably
            //TODO 3 easy generator to have definition keys + unique keys in a const file somewhere somehow
            defActionService.Register("CreateItem", "base_axe@core.item", (object _, IDefinition 
                def, IMaterialDefinition mat) => new Axe(def, mat));
            defActionService.Register("CreateItem", "base_bucket@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new Bucket(def, mat));
            defActionService.Register("CreateItem", "base_item_chest@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new ChestItem(def, mat));
            defActionService.Register("CreateItem", "base_item_furnace@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new FurnaceItem(def, mat));
            defActionService.Register("CreateItem", "base_hammer@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new Hammer(def, mat));
            defActionService.Register("CreateItem", "base_hoe@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new Hoe(def, mat));
            defActionService.Register("CreateItem", "base_pickaxe@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new Pickaxe(def, mat));
            defActionService.Register("CreateItem", "base_shovel@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new Shovel(def, mat));
            defActionService.Register("CreateItem", "base_item_storageinterface@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new StorageInterfaceItem(def, mat));
            defActionService.Register("CreateItem", "base_item_wauzi@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new WauziItem(def, mat));
            defActionService.Register("CreateItem", "base_meat_cooked@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new MeatCooked(def, mat));
            defActionService.Register("CreateItem", "base_meat_raw@core.item", (object _, IDefinition
                def, IMaterialDefinition mat) => new MeatRaw(def, mat));

            defActionService.Register("CreateItem", "base_hand@core.item", (object _, IDefinition
                _, IMaterialDefinition _) => Hand.Instance);
        }

        /// <inheritdoc />
        public void Register(ExtensionService extensionLoader)
        {
            extensionLoader.Register<IMapGenerator>(new ComplexPlanetGenerator());

            extensionLoader.Register<IMapPopulator>(new TreePopulator(typeContainer.Get<DefinitionActionService>()));
            extensionLoader.Register<IMapPopulator>(new WauziPopulator(TypeContainer.Get<IResourceManager>()));

            extensionLoader.RegisterTypesWithSerializationId(typeof(Extension).Assembly);

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

                f.Components.AddIfTypeNotExists(new BurningComponent());

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
                s.Components.AddIfTypeNotExists(new BlockInteractionComponent(s, TypeContainer.Get<BlockInteractionService>(), TypeContainer.Get<InteractService>(), TypeContainer.Get<DefinitionActionService>()));

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
