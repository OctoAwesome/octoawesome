using engenious;

using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class BlockInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>,
        ControllableComponent,
        InventoryComponent>
    {
        private readonly Simulation simulation;
        private readonly BlockInteractionService service;


        public BlockInteractionComponent(Simulation simulation, BlockInteractionService interactionService)
        {
            this.simulation = simulation;
            service = interactionService;
        }

        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;
            var inventory = value.Component2;

            var toolbar = entity.Components.GetComponent<ToolBarComponent>();
            var cache = entity.Components.GetComponent<LocalChunkCacheComponent>().LocalChunkCache;

            controller
                .Selection?
                .Visit(
                    hitInfo => InteractWith(hitInfo, inventory, toolbar, cache),
                    applyInfo => ApplyWith(applyInfo, inventory, toolbar, cache),
                    componentContainer => componentContainer?.Hit(gameTime, entity)
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

                    IBlockDefinition definition = toolbar.ActiveTool.Item as IBlockDefinition;

                    if (definition is null && toolbar.ActiveTool.Item is IFluidInventory fluidInventory)
                    {
                        definition = fluidInventory.FluidBlock;
                    }

                    if (definition is not null)
                    {
                        bool intersects = !GetPosition(entity, controller, cache, add, definition, out var idx);

                        if (!intersects)
                        {
                            if (toolbar.ActiveTool.Item is not IBlockDefinition || inventory.RemoveUnit(toolbar.ActiveTool))
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


        private void ApplyWith(ApplyInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache)
        {
            if (!lastBlock.IsEmpty && lastBlock.Block != 0)
            {
                IItem activeItem;
                if (toolbar.ActiveTool.Item is IItem item)
                {
                    activeItem = item;
                }
                else
                {
                    activeItem = toolbar.HandSlot.Item as IItem;
                }

                _ = service.Apply(lastBlock, activeItem, cache);
            }
        }

        private void InteractWith(HitInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache)
        {
            if (!lastBlock.IsEmpty && lastBlock.Block != 0)
            {
                IItem activeItem;
                if (toolbar.ActiveTool.Item is IItem item)
                {
                    activeItem = item;
                }
                else
                {
                    activeItem = toolbar.HandSlot.Item as IItem;
                }

                var blockHitInformation = service.Hit(lastBlock, activeItem, cache);

                if (blockHitInformation.Valid)
                    foreach (var (Quantity, Definition) in blockHitInformation.List)
                    {
                        if (activeItem is IFluidInventory fluidInventory
                            && Definition is IBlockDefinition fluidBlock
                            && fluidBlock.Material is IFluidMaterialDefinition)
                        {
                            fluidInventory.AddFluid(Quantity, fluidBlock);
                        }
                        else if (Definition is IInventoryable invDef)
                        {
                            inventory.AddUnit(Quantity, invDef);
                        }

                    }

            }
        }
        private static bool GetPosition(Entity entity, ControllableComponent controller, ILocalChunkCache cache, Index3 add, IBlockDefinition definition, out Index3 idx)
        {
            idx = controller.ApplyBlock.Value + add;
            var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);


            var positioncomponent = entity.Components.GetComponent<PositionComponent>();
            var bodycomponent = entity.Components.GetComponent<BodyComponent>();

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

                // Nicht in sich selbst reinbauen
                for (var i = 0; i < boxes.Length; i++)
                {
                    var box = boxes[i];
                    var newBox = new BoundingBox(idx + box.Min, idx + box.Max);
                    if (newBox.Min.X < playerBox.Max.X && newBox.Max.X > playerBox.Min.X &&
                        newBox.Min.Y < playerBox.Max.Y && newBox.Max.X > playerBox.Min.Y &&
                        newBox.Min.Z < playerBox.Max.Z && newBox.Max.X > playerBox.Min.Z)
                        return false;
                }
            }
            return true;
        }
    }
}
