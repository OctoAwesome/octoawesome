﻿using engenious;

using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Items;
using OctoAwesome.EntityComponents;
using System.Diagnostics;
using OctoAwesome.Services;
using OctoAwesome.Graphs;
using System.Collections.Generic;
using OctoAwesome.Caching;
using System.Linq;
using OpenTK.Graphics.ES11;
using System;
using NLog.Layouts;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with block interactions.
    /// </summary>
    [SerializationId()]
    public class BlockInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent, PositionComponent>,
        ControllableComponent,
        InventoryComponent,
        PositionComponent>
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
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent, PositionComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;
            var inventory = value.Component2;
            var positioncomponent = value.Component3;

            var toolbar = entity.Components.Get<ToolBarComponent>();
            var cache = entity.Components.Get<LocalChunkCacheComponent>()?.LocalChunkCache;
            Debug.Assert(cache != null, nameof(cache) + " != null");

            var selectionType = controller.Selection?.SelectionType;

            controller
                .Selection?
                .Visit(
                    blockInfo =>
                    {
                        Debug.Assert(toolbar != null, nameof(toolbar) + " != null");
                        switch (selectionType)
                        {
                            case SumTypes.SelectionType.Hit:
                                HitWith(blockInfo, inventory, toolbar, cache, positioncomponent);
                                break;
                            case SumTypes.SelectionType.Interact:
                                InteractWith(blockInfo, inventory, toolbar, cache, positioncomponent);
                                break;
                            case null:
                                break;
                        }
                    },
                    componentContainer =>
                    {
                        if (selectionType == SumTypes.SelectionType.Interact && componentContainer.TryGetComponent<InteractKeyComponent>(out var keyComp))
                        {
                            interactService.Interact(keyComp.Key, gameTime, entity, componentContainer);
                        }
                    }
                );

            if (toolbar != null && controller.InteractBlock.HasValue)
            {
                if (toolbar.ActiveTool != null)
                {
                    Index3 add = controller.InteractSide switch
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
                        Index3 idx = controller.InteractBlock.Value + add;
                        var boxes = definition.GetCollisionBoxes(cache, idx.X, idx.Y, idx.Z);

                        bool intersects = false;
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
                                cache.SetBlockMeta(idx, (int)controller.InteractSide);
                                if (toolbar.ActiveTool.Amount <= 0)
                                    toolbar.RemoveSlot(toolbar.ActiveTool);

                                DoNetworkBlockStuff(positioncomponent, cache, definition, idx);
                            }
                        }
                    }
                }
                controller.InteractBlock = null;
            }
        }

        private void DoNetworkBlockStuff(PositionComponent? positioncomponent, ILocalChunkCache? cache, IBlockDefinition definition, Index3 idx)
        {
            if (definition is INetworkBlock nb)
            {
                var pencil = simulation.ResourceManager.Pencils[positioncomponent.Position.Planet];
                var ourInfo = cache.GetBlockInfo(idx);


                var node = nb.CreateNode();
                node.BlockInfo = ourInfo;


                foreach (var transferType in nb.TransferTypes)
                {
                    Graph? graph = null;

                    MaybeAddToGraph(idx.X + 1, idx.Y, idx.Z, cache, pencil, ourInfo, transferType, node, ref graph);
                    MaybeAddToGraph(idx.X - 1, idx.Y, idx.Z, cache, pencil, ourInfo, transferType, node, ref graph);
                    MaybeAddToGraph(idx.X, idx.Y + 1, idx.Z, cache, pencil, ourInfo, transferType, node, ref graph);
                    MaybeAddToGraph(idx.X, idx.Y - 1, idx.Z, cache, pencil, ourInfo, transferType, node, ref graph);
                    MaybeAddToGraph(idx.X, idx.Y, idx.Z + 1, cache, pencil, ourInfo, transferType, node, ref graph);
                    MaybeAddToGraph(idx.X, idx.Y, idx.Z - 1, cache, pencil, ourInfo, transferType, node, ref graph);

                    if (graph is null)
                    {
                        if (!Pencil.GraphTypes.TryGetValue(transferType, out var type))
                        {
                            return;
                        }

                        var newGraph = GenericCaster<object, Graph>.Cast(Activator.CreateInstance(type, pencil.PlanetId))!;
                        pencil.AddGraph(newGraph);
                        newGraph.AddBlock(node);
                    }
                }

            }
        }

        private void MaybeAddToGraph(int x, int y, int z, ILocalChunkCache? cache, Pencil? pencil, BlockInfo ourInfo, string transferType, NodeBase node, ref Graph? graph)
        {
            var index3 = new Index3(x, y, z);
            index3.NormalizeXY(pencil.Planet.Size.XY * new Index2(Chunk.CHUNKSIZE_X, Chunk.CHUNKSIZE_Y));
            var id = cache.GetBlock(index3);
            if (id == 0)
                return;
            var definition = simulation.ResourceManager.DefinitionManager.GetBlockDefinitionByIndex(id);
            if (definition is not INetworkBlock networkBlock)
                return;

            var info = cache.GetBlockInfo(index3);
            foreach (var item in pencil.Graphs)
            {
                if (item.TransferType != transferType)
                    continue;

                if (item.ContainsPosition(info.Position))
                {
                    if (graph is null)
                    {
                        item.AddBlock(node);
                        graph = item;
                        break;
                    }
                    else
                    {
                        if (item == graph)
                            continue;
                        graph.MergeWith(item, ourInfo);
                        pencil.RemoveGraph(item);
                        break;
                    }
                }
            }
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }


        private void InteractWith(BlockInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache, PositionComponent posComponent)
        {
            if (!lastBlock.IsEmpty && lastBlock.Block != 0)
            {
                IItem activeItem;
                if (toolbar.ActiveTool?.Item is IItem item)
                    activeItem = item;
                else
                    activeItem = Hand.Instance;

                Debug.Assert(activeItem != null, nameof(activeItem) + " != null");

                if (simulation.ResourceManager.Pencils.TryGetValue(posComponent.Position.Planet, out var pencil))
                {
                    pencil.InteractNode(lastBlock.Position);
                 
                }
            }
        }

        private void HitWith(BlockInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache, PositionComponent posComponent)
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
                {
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
                        //TODO RemoveNode of Graph

                        if (definition is INetworkBlock nb)
                        {
                            var pencil = simulation.ResourceManager.Pencils[posComponent.Position.Planet];
                            foreach (var graph in pencil.Graphs.OfType<Graph<int>>())
                            {
                                if (graph.Nodes.ContainsKey(lastBlock.Position))
                                {
                                    graph.RemoveNode(lastBlock);
                                    break;
                                }
                            }
                        }
                    }
                }

                {
                    if (simulation.ResourceManager.Pencils.TryGetValue(posComponent.Position.Planet, out var pencil))
                    {
                        foreach (var graph in pencil.Graphs)
                        {
                            if (graph.TryGetNode(lastBlock.Position, out var node))
                            {
                                node.Hit();
                            }
                        }
                    }
                }
            }
        }
    }
}
