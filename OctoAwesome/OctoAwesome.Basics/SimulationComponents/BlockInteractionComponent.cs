using engenious;

using OctoAwesome.Chunking;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.Basics.Definitions.Items;
using OctoAwesome.EntityComponents;
using System.Diagnostics;
using OctoAwesome.Location;
using OctoAwesome.Services;
using OctoAwesome.Graphs;
using System.Collections.Generic;
using OctoAwesome.Caching;
using System.Linq;
using OpenTK.Graphics.ES11;
using System;
using NLog.Layouts;
using OctoAwesome.Information;
using OctoAwesome.Extension;

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
        private readonly BlockInteractionService service;
        private readonly InteractService interactService;
        private readonly IDefinitionManager definitionManager;
        private readonly DefinitionActionService definitionActionService;


        /// <summary>
        /// Initializes a new instance of the <see cref="BlockInteractionComponent"/> class.
        /// </summary>
        /// <param name="simulation">The simulation the block interactions should happen in.</param>
        /// <param name="blockInteractionService">
        /// The interaction service to actually interact with blocks in the simulation.
        /// </param>
        public BlockInteractionComponent(Simulation simulation, BlockInteractionService blockInteractionService, InteractService interactService, DefinitionActionService definitionActionService)
        {
            this.simulation = simulation;
            service = blockInteractionService;
            this.interactService = interactService;
            definitionManager = simulation.ResourceManager.DefinitionManager;
            this.definitionActionService = definitionActionService;
        }


        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent, PositionComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;
            var inventory = value.Component2;
            var positioncomponent = value.Component3;

            var toolbar = entity.GetComponent<ToolBarComponent>();
            var cache = entity.GetComponent<LocalChunkCacheComponent>()?.LocalChunkCache;
            Debug.Assert(cache != null, nameof(cache) + " != null");

            var selectionType = controller.Selection?.SelectionType;

            controller
                .Selection?
                .Visit(
                    hitInfo =>
                    {
                        Debug.Assert(toolbar != null, nameof(toolbar) + " != null");
                        switch (selectionType)
                        {
                            case SumTypes.SelectionType.Hit:
                                HitWith(hitInfo, inventory, toolbar, cache, positioncomponent);
                                break;
                            case SumTypes.SelectionType.Interact:
                                {

                                    IItem activeItem;
                                    if (toolbar.ActiveTool?.Item is IItem item)
                                        activeItem = item;
                                    else
                                        activeItem = Hand.Instance;

                                    interactService.Interact(activeItem.Definition.DisplayName, gameTime, entity, hitInfo);
                                    interactService.Interact("", gameTime, entity, hitInfo);
                                    InteractWith(hitInfo, inventory, toolbar, cache, positioncomponent);
                                    break;
                                }
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
        }

        private void DoNetworkBlockStuff(PositionComponent? positioncomponent, ILocalChunkCache? cache, IBlockDefinition definition, Index3 idx)
        {

            if (definitionManager.TryGetVariation<INetworkBlock>(definition, out var nb))
            {
                var pencil = simulation.ResourceManager.Pencils[positioncomponent.Position.Planet];
                var ourInfo = cache.GetBlockInfo(idx);


                var node = definitionActionService.Function<NodeBase>("CreateNode", nb, null);
                node.BlockInfo = ourInfo;
                node.PlanetId = positioncomponent.Position.Planet;


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
            var defaultDefinition = simulation.ResourceManager.DefinitionManager.GetDefinitionByIndex(id);

            if (defaultDefinition is null ||
                !simulation.ResourceManager.DefinitionManager.TryGetVariation<INetworkBlock>(defaultDefinition, out _))
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



        private void HitWith(HitInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache, PositionComponent posComponent)
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


                        if (definition is INetworkBlock nb)
                        {
                            var pencil = simulation.ResourceManager.Pencils[posComponent.Position.Planet];
                            foreach (var graph in pencil.Graphs.OfType<Graph<int>>())
                            {
                                if (graph.Nodes.ContainsKey(lastBlock.Position))
                                {
                                    graph.RemoveNode(new(lastBlock.Position, lastBlock.Block, lastBlock.Meta));
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

        private void InteractWith(HitInfo lastBlock, InventoryComponent inventory, ToolBarComponent toolbar, ILocalChunkCache cache, PositionComponent posComponent)
        {
            if (!lastBlock.IsEmpty && lastBlock.Block != 0)
            {
                IItem activeItem;
                if (toolbar.ActiveTool?.Item is IItem item)
                    activeItem = item;
                else
                    activeItem = Hand.Instance;

                Debug.Assert(activeItem != null, nameof(activeItem) + " != null");

                _ = service.Interact(lastBlock, activeItem, cache);

                if (simulation.ResourceManager.Pencils.TryGetValue(posComponent.Position.Planet, out var pencil))
                {
                    foreach (var graph in pencil.Graphs)
                    {
                        if (graph.TryGetNode(lastBlock.Position, out var node))
                        {
                            node.Interact();
                        }
                    }
                }
            }
        }
    }
}
