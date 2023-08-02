using engenious;

using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Items;
using OctoAwesome.EntityComponents;
using System.Diagnostics;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with block interactions.
    /// </summary>
    [SerializationId(2, 17)]
    public class BlockInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>,
        ControllableComponent,
        InventoryComponent>
    {
        private readonly Simulation simulation;
        private readonly BlockCollectionService service;
        private readonly InteractService interactService;


        /// <summary>
        /// Initializes a new instance of the <see cref="BlockInteractionComponent"/> class.
        /// </summary>
        /// <param name="simulation">The simulation the block interactions should happen in.</param>
        /// <param name="blockInteractionService">
        /// The interaction service to actually interact with blocks in the simulation.
        /// </param>
        public BlockInteractionComponent(Simulation simulation, BlockCollectionService blockInteractionService, InteractService interactService)
        {
            this.simulation = simulation;
            service = blockInteractionService;
            this.interactService = interactService;
        }

        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;
            var inventory = value.Component2;

            var toolbar = entity.Components.Get<ToolBarComponent>();
            var cache = entity.Components.Get<LocalChunkCacheComponent>()?.LocalChunkCache;
            Debug.Assert(cache != null, nameof(cache) + " != null");

            controller
                .Selection?
                .Visit(
                    blockInfo =>
                    {
                        Debug.Assert(toolbar != null, nameof(toolbar) + " != null");
                        InteractWith(blockInfo, inventory, toolbar, cache);
                    },
                    componentContainer =>
                    {
                        if (componentContainer.TryGetComponent<InteractKeyComponent>(out var keyComp))
                        {
                            interactService.Interact(keyComp.Key, gameTime, entity, componentContainer);
                        }
                    }
                );

            if (toolbar != null && controller.ApplyBlock.HasValue)
            {
                if (toolbar.ActiveTool != null)
                {
                    Index3 add = controller.ApplySide switch
                    {
                        OrientationFlags.SideWest => new Index3(-1, 0, 0),
                        OrientationFlags.SideEast => new Index3(1, 0, 0),
                        OrientationFlags.SideSouth => new Index3(0, -1, 0),
                        OrientationFlags.SideNorth => new Index3(0, 1, 0),
                        OrientationFlags.SideBottom => new Index3(0, 0, -1),
                        OrientationFlags.SideTop => new Index3(0, 0, 1),
                        _ => new Index3(),
                    };

                    if (toolbar.ActiveTool.Item is IBlockDefinition definition)
                    {
                        Index3 idx = controller.ApplyBlock.Value + add;
                        var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                        bool intersects = false;
                        var positioncomponent = entity.Components.Get<PositionComponent>();
                        var bodycomponent = entity.Components.Get<BodyComponent>();

                        if (positioncomponent != null && bodycomponent != null)
                        {
                            float gap = 0.01f;
                            var playerBox = new BoundingBox(
                                new Vector3(
                                    positioncomponent.Position.GlobalBlockIndex.X + positioncomponent.Position.BlockPosition.X - bodycomponent.Radius + gap,
                                    positioncomponent.Position.GlobalBlockIndex.Y + positioncomponent.Position.BlockPosition.Y - bodycomponent.Radius + gap,
                                    positioncomponent.Position.GlobalBlockIndex.Z + positioncomponent.Position.BlockPosition.Z + gap),
                                new Vector3(
                                    positioncomponent.Position.GlobalBlockIndex.X + positioncomponent.Position.BlockPosition.X + bodycomponent.Radius - gap,
                                    positioncomponent.Position.GlobalBlockIndex.Y + positioncomponent.Position.BlockPosition.Y + bodycomponent.Radius - gap,
                                    positioncomponent.Position.GlobalBlockIndex.Z + positioncomponent.Position.BlockPosition.Z + bodycomponent.Height - gap)
                                );

                            // Do not build in oneself
                            for (var i = 0; i < boxes.Length; i++)
                            {
                                var box = boxes[i];
                                var newBox = new BoundingBox(idx + box.Min, idx + box.Max);
                                if (newBox.Min.X < playerBox.Max.X && newBox.Max.X > playerBox.Min.X &&
                                    newBox.Min.Y < playerBox.Max.Y && newBox.Max.X > playerBox.Min.Y &&
                                    newBox.Min.Z < playerBox.Max.Z && newBox.Max.X > playerBox.Min.Z)
                                    intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            if (inventory.RemoveUnit(toolbar.ActiveTool) > 0)
                            {
                                cache.SetBlock(idx, simulation.ResourceManager.DefinitionManager.GetDefinitionIndex(definition));
                                cache.SetBlockMeta(idx, (int)controller.ApplySide);
                                if (toolbar.ActiveTool.Amount <= 0)
                                    toolbar.RemoveSlot(toolbar.ActiveTool);
                            }
                        }
                    }
                }
                controller.ApplyBlock = null;
            }
        }

        private void InteractWith(BlockInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache)
        {
            if (!lastBlock.IsEmpty && lastBlock.Block != 0)
            {
                IItem activeItem;
                if (toolbar.ActiveTool?.Item is IItem item)
                    activeItem = item;
                else
                    activeItem = Hand.Instance;

                Debug.Assert(activeItem != null, nameof(activeItem) + " != null");

                var blockHitInformation = service.Hit(lastBlock, activeItem, cache);

                if (blockHitInformation.Valid && blockHitInformation.List != null)
                    foreach (var (quantity, definition) in blockHitInformation.List)
                    {
                        if (activeItem is IFluidInventory fluidInventory
                            && definition is IBlockDefinition { Material: IFluidMaterialDefinition } fluidBlock)
                        {
                            fluidInventory.AddFluid(quantity, fluidBlock);
                        }
                        else if (definition is IInventoryable invDef)
                        {
                            inventory.Add(invDef, quantity);
                        }

                    }

            }
        }
    }
}
