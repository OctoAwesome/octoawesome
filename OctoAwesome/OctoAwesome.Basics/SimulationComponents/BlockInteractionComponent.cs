﻿using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Services;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Definitions;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(ControllableComponent), typeof(InventoryComponent))]
    public class BlockInteractionComponent : SimulationComponent<ControllableComponent, InventoryComponent>
    {
        private readonly Simulation simulation;
        private readonly BlockCollectionService service;


        public BlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            this.simulation = simulation;
            service = interactionService;
        }

        protected override bool AddEntity(Entity entity)
        {
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {

        }

        protected override void UpdateEntity(GameTime gameTime, Entity entity, ControllableComponent controller, InventoryComponent inventory)
        {
            var toolbar = entity.Components.GetComponent<ToolBarComponent>();
            var cache = entity.Components.GetComponent<LocalChunkCacheComponent>().LocalChunkCache;

            if (controller.InteractBlock.HasValue)
            {
                var lastBlock = cache.GetBlockInfo(controller.InteractBlock.Value);

                if (!lastBlock.IsEmpty)
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
                controller.InteractBlock = null;
            }

            if (toolbar != null && controller.ApplyBlock.HasValue)
            {
                if (toolbar.ActiveTool != null)
                {
                    Index3 add = new Index3();
                    switch (controller.ApplySide)
                    {
                        case OrientationFlags.SideWest: add = new Index3(-1, 0, 0); break;
                        case OrientationFlags.SideEast: add = new Index3(1, 0, 0); break;
                        case OrientationFlags.SideSouth: add = new Index3(0, -1, 0); break;
                        case OrientationFlags.SideNorth: add = new Index3(0, 1, 0); break;
                        case OrientationFlags.SideBottom: add = new Index3(0, 0, -1); break;
                        case OrientationFlags.SideTop: add = new Index3(0, 0, 1); break;
                    }

                    if (toolbar.ActiveTool.Item is IBlockDefinition definition)
                    {
                        Index3 idx = controller.ApplyBlock.Value + add;
                        var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                        bool intersects = false;
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
                                    intersects = true;
                            }
                        }

                        if (!intersects)
                        {
                            if (inventory.RemoveUnit(toolbar.ActiveTool))
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
    }
}
