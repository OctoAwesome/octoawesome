using System;
using System.Diagnostics;
using engenious;

using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Chunking;
using OctoAwesome.EntityComponents;
using OctoAwesome.Location;
using OctoAwesome.Extension;
using OctoAwesome.Serialization;

namespace OctoAwesome.Basics.Entities
{
    /// <summary>
    /// An entity used for dogs in the game.
    /// </summary>
    [SerializationId(1, 2)]
    public class WauziEntity : Entity
    {
        class MoveLogic
        {
            private System.Random random = new();
            private Index2 lastGoal = default;
            private Coordinate lastPosition = default;
            private int lastPositionCount = 0;

            internal bool ShouldForceJump()
            {
                return lastPositionCount > 5;

            }

            internal Vector2 GetMoveDir(PositionComponent pointOfInterest, PositionComponent wauziPosition, Entity followEntity, Index2 size)
            {
                if (lastPosition == wauziPosition.Position)
                    lastPositionCount++;
                else
                    lastPositionCount = 0;

                lastPosition = wauziPosition.Position;

                if (lastGoal == default)
                    lastGoal = new Index2(wauziPosition.Position.GlobalBlockIndex);
                Vector2 move = Vector2.Zero;

                if (pointOfInterest is not null)
                {
                    var diff = wauziPosition.Position.GlobalBlockIndex.ShortestDistanceXY(pointOfInterest.Position.GlobalBlockIndex, size);
                    var length = diff.Length();
                    if (followEntity is not null)
                    {
                        if (length > 250 || length != length)
                            wauziPosition.Position = pointOfInterest.Position;

                        if (length is < 50 and > 5)
                        {
                            move = WanderAroundPosition(wauziPosition, lastGoal, size);
                        }
                    }
                    else
                    {

                        if (length < 7 && length == length)
                        {
                            var poiPos = new Index2(pointOfInterest.Position.GlobalBlockIndex);
                            if (lastGoal.ShortestDistanceXY(poiPos, size).Length() >= 7)
                                lastGoal = new Index2(wauziPosition.Position.GlobalBlockIndex);

                            move = WanderAroundPosition(wauziPosition, new Index2(pointOfInterest.Position.GlobalBlockIndex), size, -4, 5, -4, 5);

                        }
                        else if (length < 70)
                            move = new Vector2(diff.X, diff.Y);
                    }
                }

                if (move == Vector2.Zero)
                    move = WanderAroundPosition(wauziPosition, lastGoal, size);

                if (move != Vector2.Zero)
                {
                    move.Normalize();
                }
                return move;
            }

            private Vector2 WanderAroundPosition(PositionComponent wauziPosition, Index2 wanderAround, Index2 size, int minX = -10, int maxX = 11, int minY = -10, int maxY = 11)
            {
                Vector2 move;
                wanderAround.NormalizeXY(size);
                if (lastGoal.X == wauziPosition.Position.GlobalBlockIndex.X && lastGoal.Y == wauziPosition.Position.GlobalBlockIndex.Y)
                {
                    lastGoal = new Index2(wanderAround.X + random.Next(minX, maxX), wanderAround.Y + random.Next(minY, maxY));
                    lastGoal.NormalizeXY(size);
                }
                var shortestDist = new Index2(wauziPosition.Position.GlobalBlockIndex).ShortestDistanceXY(lastGoal, size);
                move = new Vector2(shortestDist.X, shortestDist.Y);
                return move;
            }
        }

        public int JumpTime { get; set; }

        private Vector2 moveDir = new Vector2(0.5f, 0.5f);
        private PositionComponent positionComponent;
        private Entity followEntity;
        private MoveLogic moveLogic = new();

        public WauziEntity() : base()
        {
        }

        /// <inheritdoc />
        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (!entity.Components.TryGet<ToolBarComponent>(out var toolbar)
                || !entity.Components.TryGet<InventoryComponent>(out var inventory)
                || !entity.Components.TryGet<PositionComponent>(out var position)
                || toolbar.ActiveTool?.Item is not MeatRaw)
                return;

            var controller = Components.Get<ControllableComponent>();
            controller.JumpInput = true;
            if (!Components.Contains<RelatedEntityComponent>())
            {
                var relEntity = new RelatedEntityComponent();
                relEntity.RelatedEntityId = entity.Id;
                Components.Add(relEntity);
                followEntity = entity;
            }

            inventory.RemoveUnit(toolbar.ActiveTool);
            if (toolbar.ActiveTool.Amount < 1)
            {
                inventory.Remove(toolbar.ActiveTool);
                toolbar.RemoveSlot(toolbar.ActiveTool);
            }
        }


        /// <inheritdoc />
        public override void Update(GameTime gameTime)
        {
            PositionComponent? position = null;

            if (followEntity is null && Components.TryGet<RelatedEntityComponent>(out var relEntity))
            {
                Simulation.TryGetById<Entity>(relEntity.RelatedEntityId, out followEntity);
            }

            if (followEntity is null)
            {
                foreach (var player in Simulation.GetEntitiesOfType<Player>())
                {
                    if (player.Components.TryGet<ToolBarComponent>(out var toolbar)
                        && player.Components.TryGet<PositionComponent>(out var newPos)
                        && toolbar.ActiveTool?.Item is MeatRaw)
                    {
                        position = newPos;
                    }
                }
            }
            else
            {
                position = followEntity.Components.Get<PositionComponent>();
            }

            moveDir = moveLogic.GetMoveDir(position, positionComponent, followEntity, new Index2(positionComponent.Planet.Size.X * Chunk.CHUNKSIZE_X, positionComponent.Planet.Size.Y * Chunk.CHUNKSIZE_Y));

            ControllableComponent controller = Components.Get<ControllableComponent>();
            controller.MoveInput = moveDir;
            if (JumpTime <= 0 || moveLogic.ShouldForceJump())
            {
                controller.JumpInput = true;
                JumpTime = 10000;
            }
            else
            {
                JumpTime -= gameTime.ElapsedGameTime.Milliseconds;
            }

            if (controller.JumpActive)
            {
                controller.JumpInput = false;
            }
        }

        /// <inheritdoc />
        public override void RegisterDefault()
        {
            positionComponent = Components.Get<PositionComponent>() ?? new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) };


            Components.AddIfTypeNotExists(positionComponent);
            Components.AddIfTypeNotExists(new GravityComponent());
            Components.AddIfTypeNotExists(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            Components.AddIfTypeNotExists(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
            Components.AddIfTypeNotExists(new MoveableComponent());
            Components.AddIfNotExists(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }));
            Components.AddIfTypeNotExists(new ControllableComponent());
            Components.AddIfNotExists(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 });
            Components.AddIfTypeNotExists(new LocalChunkCacheComponent(positionComponent.Planet.GlobalChunkCache, 2, 1));
        }
    }
}
