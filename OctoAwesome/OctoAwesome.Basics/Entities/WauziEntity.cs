using engenious;

using OctoAwesome.Basics.Definitions.Items.Food;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Serialization;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Serialization;

namespace OctoAwesome.Basics.Entities
{
    /// <summary>
    /// An entity used for dogs in the game.
    /// </summary>
    [SerializationId(1, 2)]
    public class WauziEntity : UpdateableEntity
    {
        /// <summary>
        /// Gets or sets a value indicating the time left to the next jump.
        /// </summary>
        public int JumpTime { get; set; }

        private Vector2 moveDir = new Vector2(0.5f, 0.5f);
        private PositionComponent posComponent;
        private Entity followEntity;        

        /// <summary>
        /// Initializes a new instance of the<see cref="WauziEntity" /> class
        /// </summary>
        public WauziEntity() : base()
        {
        }

        ///<inheritdoc/>
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
                inventory.Remove(toolbar.ActiveTool);
                toolbar.RemoveSlot(toolbar.ActiveTool);
            }
        }

        /// <inheritdoc />
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

            if (position is not null)
            {
                var diff = posComponent.Position.GlobalBlockIndex.ShortestDistanceXY(position.Position.GlobalBlockIndex, posComponent.Planet.Size);
                var length = diff.Length();
                if (followEntity is not null || length < 50)
                {
                    if (length > 250 || length != length)
                        posComponent.Position = position.Position;
                    if (length < 5 || length != length)
                        diff = Index3.Zero;

                    moveDir = new Vector2(diff.X, diff.Y);
                    if (diff != Index3.Zero)
                    {
                        moveDir.Normalize();
                    }
                }
            }
            else
                moveDir = new Vector2(0.5f, 0.5f);

            ControllableComponent controller = Components.GetComponent<ControllableComponent>();
            controller.MoveInput = moveDir;
            if (JumpTime <= 0)
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
            posComponent = Components.GetComponent<PositionComponent>() ?? new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 200), new Vector3(0, 0, 0)) };

            Components.AddComponent(posComponent);
            Components.AddComponent(new GravityComponent());
            Components.AddComponent(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });
            Components.AddComponent(new BodyPowerComponent() { Power = 600f, JumpTime = 120 });
            Components.AddComponent(new MoveableComponent());
            Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }), true);
            Components.AddComponent(new ControllableComponent());
            Components.AddComponent(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 }, true);
            Components.AddComponent(new LocalChunkCacheComponent(posComponent.Planet.GlobalChunkCache, 2, 1));
        }
    }
}
