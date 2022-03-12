using engenious;

using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Chunking;
using OctoAwesome.EntityComponents;
using OctoAwesome.Location;
using OctoAwesome.Serialization;

namespace OctoAwesome.Basics.Entities
{
    [SerializationId(1, 2)]
    public class WauziEntity : UpdateableEntity
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

                //if (pointOfInterest is not null)
                //    wauziPosition.Position = pointOfInterest.Position;

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
        private PositionComponent posComponent;
        private Entity followEntity;
        private MoveLogic moveLogic = new();

        public WauziEntity() : base()
        {
        }

        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (!entity.Components.TryGetComponent<ToolBarComponent>(out var toolbar)
                || !entity.Components.TryGetComponent<InventoryComponent>(out var inventory)
                || !entity.Components.TryGetComponent<PositionComponent>(out var position)
                || toolbar.ActiveTool?.Item is not MeatRaw)
                return;

            ControllableComponent controller = Components.GetComponent<ControllableComponent>();
            controller.JumpInput = true;
            if (!Components.ContainsComponent<RelatedEntityComponent>())
            {
                var relEntity = new RelatedEntityComponent();
                relEntity.RelatedEntityId = entity.Id;
                Components.AddComponent(relEntity);
                followEntity = entity;
            }

            inventory.RemoveUnit(toolbar.ActiveTool);
            if (toolbar.ActiveTool.Amount < 1)
            {
                inventory.RemoveSlot(toolbar.ActiveTool);
                toolbar.RemoveSlot(toolbar.ActiveTool);
            }
        }


        public override void Update(GameTime gameTime)
        {
            PositionComponent position = null;

            if (followEntity is null && Components.TryGetComponent<RelatedEntityComponent>(out var relEntity))
            {
                Simulation.TryGetById<Entity>(relEntity.RelatedEntityId, out followEntity);
            }

            if (followEntity is null)
            {
                foreach (var player in Simulation.GetEntitiesOfType<Player>())
                {
                    if (!player.Components.TryGetComponent<ToolBarComponent>(out var toolbar)
                        || !player.Components.TryGetComponent<PositionComponent>(out var newPos))
                        continue;
                    if (toolbar.ActiveTool?.Item is not MeatRaw)
                        continue;
                    position = newPos;
                }
            }
            else
            {
                position = followEntity.GetComponent<PositionComponent>();
            }

            moveDir = moveLogic.GetMoveDir(position, posComponent, followEntity, new Index2(posComponent.Planet.Size.X * Chunk.CHUNKSIZE_X, posComponent.Planet.Size.Y * Chunk.CHUNKSIZE_Y));



            ControllableComponent controller = Components.GetComponent<ControllableComponent>();
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

        public override void RegisterDefault()
        {
            posComponent = Components.GetComponent<PositionComponent>() ?? new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) };

            Components.AddComponent(posComponent);
            Components.AddComponent(new GravityComponent());
            Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f }, true);
            Components.AddComponent(new BodyPowerComponent() { Power = 600f, JumpTime = 150 }, true);
            Components.AddComponent(new MoveableComponent());
            Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }), true);
            Components.AddComponent(new ControllableComponent());
            Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Components.AddComponent(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 2, 1));
        }
    }
}
