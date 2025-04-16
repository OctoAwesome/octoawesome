using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Caching;
using OctoAwesome.Chunking;
using OctoAwesome.Components;
using OctoAwesome.Definitions;
using OctoAwesome.EntityComponents;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OctoAwesome.Basics;


internal class FallBlockDetector : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, MoveableComponent, FallingBlockComponent, PositionComponent>,
        MoveableComponent,
        FallingBlockComponent,
        PositionComponent>, IDisposable
{
    private readonly IDefinitionManager definitionManager;
    private readonly IResourceManager resManager;
    private readonly Pooling.Pool<PoolableEntity> pool;
    private readonly Relay<Notification> simulationRelay;
    private readonly IDisposable simulationSubscription;
    private readonly IDisposable listenSubscription;
    private readonly Dictionary<Guid, List<Entity>> fallingEntities = new();

    private IGlobalChunkCache? globalChunkCache;
    private ushort? zLimitOfChunkColumn;

    public FallBlockDetector(IDefinitionManager definitionManager, IResourceManager resManager)
    {
        this.definitionManager = definitionManager;
        this.resManager = resManager;
        pool = new Pool<PoolableEntity>();
        simulationRelay = new Relay<Notification>();

        simulationSubscription
            = resManager
            .UpdateHub
            .AddSource(simulationRelay, DefaultChannels.Simulation);
        listenSubscription = resManager.UpdateHub.ListenOn(DefaultChannels.Chunk).Subscribe(OnNext);
    }

    public void Dispose()
    {
        listenSubscription?.Dispose();
        simulationSubscription?.Dispose();
    }

    protected override bool Match(Entity value)
    {
        return base.Match(value);
    }

    protected override SimulationComponentRecord<Entity, MoveableComponent, FallingBlockComponent, PositionComponent> OnAdd(Entity value)
    {
        var groupId = value.GetComponent<FallingBlockComponent>().GroupId;
        if (groupId != Guid.Empty)
        {
            ref var list = ref CollectionsMarshal.GetValueRefOrAddDefault(fallingEntities, groupId, out var exists);
            if (!exists)
                list = new();
            list!.Add(value);
        }

        return base.OnAdd(value);
    }

    public void OnNext(object value)
    {
        switch (value)
        {
            case BlockChangedNotification blockChangedNotification:

                CheckUpperBlock(blockChangedNotification);
                break;
            case BlocksChangedNotification blocksChangedNotification:
                //Update(blocksChangedNotification);
                break;
            default:
                break;
        }
    }

    internal void CheckUpperBlock(BlockChangedNotification notification)
    {
        if (globalChunkCache is null)
        {
            var planet = resManager.GetPlanet(0);
            globalChunkCache = planet.GlobalChunkCache;
            zLimitOfChunkColumn = (ushort)(planet.Size.Z * Chunk.CHUNKSIZE_Z);
        }

        var blockInfo = notification.BlockInfo;
        var chunkPos = notification.ChunkPos;

        if (blockInfo.Block == 0)
        {
            SpawnReplacement(blockInfo, chunkPos);
        }
        else if (definitionManager.GetDefinitionByIndex(blockInfo.Block) is IDefinition definition
            && definitionManager.TryGetVariation<FallBlockDefinition>(definition, out var fallDef))
        {
            var column = globalChunkCache.Peek(chunkPos.XY);
            if (column == null)
            {
                Debug.WriteLine("Column should never be null!");
                return;
            }
            var pos = blockInfo.Position;
            var belowPos = new Index3(pos.X, pos.Y, pos.Z - 1);
            var below = column.GetBlock(belowPos);
            if (below == 0)
            {
                SpawnReplacement(new BlockInfo(belowPos, 0), chunkPos);
            }
        }
    }

    private async Task SpawnReplacement(BlockInfo info, Index3 chunkPos)
    {
        await Task.Yield();

        var maximumBlocks = zLimitOfChunkColumn.Value - info.Position.Z;
        var fallBlockDefinitionPosition = ArrayPool<(Index3, FallBlockDefinition)>.Shared.Rent(maximumBlocks);
        int upTo = 0;

        var column = globalChunkCache.Peek(chunkPos.XY);
        for (int i = info.Position.Z; i < zLimitOfChunkColumn; i++)
        {
            var pos = new Index3(info.Position.X, info.Position.Y, i + 1);

            var block = column.GetBlock(pos);
            var general = definitionManager.GetDefinitionByIndex(block);
            if (general is null)
                break;
            var canFall = definitionManager.TryGetVariation<FallBlockDefinition>(general, out var fallDef);
            if (!canFall)
                break;
            fallBlockDefinitionPosition[i - info.Position.Z] = (pos, fallDef!);
            upTo++;
        }
        column.SetBlocks(true, fallBlockDefinitionPosition.Take(upTo).Select(x => new BlockInfo(x.Item1, 0)));
        var groupId = Guid.NewGuid();
        var relevants = GenericCaster<IComponentContainer, Simulation>
            .Cast(Parent)
            .GlobalComponentList
            .OfType<PositionComponent>()
            .Where(x => x.Position.GlobalBlockIndex.XY == info.Position.XY);

        for (int i = 0; i < upTo; i++)
        {
            var data = fallBlockDefinitionPosition[i];
            ReplaceAndSpawnEntity(data.Item1, column, data.Item2, groupId, relevants);
        }
        ArrayPool<(Index3, FallBlockDefinition)>.Shared.Return(fallBlockDefinitionPosition);
    }

    private void ReplaceAndSpawnEntity(Index3 pos, IChunkColumn? column, FallBlockDefinition fallDef, Guid groupId, IEnumerable<PositionComponent> relevants)
    {
        var blockEntity = pool.Rent();

        if (blockEntity.Components.TryGet<PositionComponent>(out var positionComponent))
        {
            positionComponent.Position = new Coordinate(0, pos, new Vector3(0.5f, 0.5f, 0));
        }
        else
        {
            positionComponent = new PositionComponent { Position = new Coordinate(0, pos, new Vector3(0.5f, 0.5f, 0)) };
            blockEntity.Components.Add(positionComponent);
        }

        if (!blockEntity.ContainsComponent<BodyComponent>())
            blockEntity.Components.Add(new BodyComponent() { Mass = 1f, Height = 0.5f, Radius = 0.25f });

        if (!blockEntity.ContainsComponent<GravityComponent>())
            blockEntity.Components.Add(new GravityComponent());

        if (!blockEntity.ContainsComponent<BoxCollisionComponent>())
            blockEntity.Components.Add(new BoxCollisionComponent([new BoundingBox(new Vector3(-0.5f, -0.5f), new Vector3(0.5f, 0.5f, 1))]));

        if (!blockEntity.ContainsComponent<EntityCollisionComponent>())
            blockEntity.Components.Add(new EntityCollisionComponent());
        if (!blockEntity.ContainsComponent<UniquePositionComponent>())
            blockEntity.Components.Add(new UniquePositionComponent());
        if (!blockEntity.ContainsComponent<InteractKeyComponent>())
            blockEntity.Components.Add(new InteractKeyComponent { Key = nameof(FallingBlockComponent) });


        if (blockEntity.Components.TryGet<MoveableComponent>(out var moveable))
        {
            moveable.OnGround = false;
            moveable.Velocity = Vector3.Zero;
            moveable.PositionMove = Vector3.Zero;
            moveable.ExternalForces = Vector3.Zero;
            moveable.ExternalPowers = Vector3.Zero;
        }
        else
        {
            blockEntity.Components.Add(new MoveableComponent());
        }

        if (blockEntity.Components.TryGet<RenderComponent>(out var render))
        {
            render.TextureName = fallDef!.Textures.First();
        }
        else
        {
            blockEntity.Components.Add(new RenderComponent() { Name = "Block", ModelName = "block", TextureName = fallDef!.Textures.First(), BaseRotation = new(90, 0, 0), BaseOffset = new(0, 0, 0.5f), LoadFromAssetComponent = true });
        }

        if (blockEntity.Components.TryGet<FallingBlockComponent>(out var fallComp))
        {
            fallComp.DefinitionIndex = definitionManager.GetDefinitionIndex(fallDef);
            fallComp.GroupId = groupId;
        }
        else
        {
            blockEntity.Components.Add(new FallingBlockComponent() { DefinitionIndex = definitionManager.GetDefinitionIndex(fallDef), GroupId = groupId });
        }

        blockEntity.Components.ReplaceAllWith(new LocalChunkCacheComponent(positionComponent.Planet.GlobalChunkCache, 0, 0));

        if (blockEntity.Components.TryGet<RelevantPositionsComponent>(out var relevantComp))
        {
            relevantComp.Positions.Clear();
            relevantComp.Positions.AddRange(relevants);
        }
        else
        {
            blockEntity.Components.Add(new RelevantPositionsComponent { Positions = [.. relevants] });
        }
        foreach (var others in relevants)
        {
            if (others.Parent.GetComponent<RelevantPositionsComponent>() is { } comp)
                comp.Positions.Add(positionComponent);
        }

        simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Add, blockEntity));
    }

    protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, MoveableComponent, FallingBlockComponent, PositionComponent> value)
    {
        if (!value.Component1.OnGround)
            return;
        var groupId = value.Component2.GroupId;
        if (groupId != Guid.Empty && fallingEntities.TryGetValue(groupId, out var entities))
        {
            fallingEntities.Remove(groupId);
            foreach (var item in entities)
            {
                ReplaceEntity(values.First(x => x.Value == item));
            }
        }
        else
        {
            ReplaceEntity(value);
        }


        void ReplaceEntity(SimulationComponentRecord<Entity, MoveableComponent, FallingBlockComponent, PositionComponent> value)
        {
            var chunkColumn = globalChunkCache.Peek(value.Component3.Position.ChunkIndex.XY);
            var globalPos = value.Component3.Position.GlobalPosition;
            var blockIdx = new Index3((int)globalPos.X, (int)globalPos.Y, (int)(globalPos.Z + 0.5));

            chunkColumn.SetBlock(blockIdx, value.Component2.DefinitionIndex);

            simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Remove, value.Value));
            if (value.Value is PoolableEntity poolable)
                pool.Return(poolable);
        }
    }
}
