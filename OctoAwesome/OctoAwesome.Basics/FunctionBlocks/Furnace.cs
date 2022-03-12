using engenious;

using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Location;
using OctoAwesome.Serialization;
using OctoAwesome.UI.Components;

using System.IO;

namespace OctoAwesome.Basics.FunctionBlocks
{
    [SerializationId(1, 4)]
    public class Furnace : FunctionalBlock
    {
        internal InventoryComponent inventoryComponent;
        internal AnimationComponent animationComponent;

        public Furnace()
        {

        }

        public override void Deserialize(BinaryReader reader) => base.Deserialize(reader);//Doesnt get called

        public Furnace(Coordinate position, float rot)
        {
            Components.AddComponent(new PositionComponent()
            {
                Position = position,
                Direction = rot
            });
        }

        protected override void OnHit(GameTime gameTime, Entity entity)
        {
            if (TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent)
               && entity.TryGetComponent<TransferComponent>(out var transferComponent)
               && entity.TryGetComponent<UiMappingComponent>(out var uiMappingComponent))
            {
                transferComponent.Targets.Clear();
                transferComponent.Targets.Add(inventoryComponent);
                uiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));

                animationComponent.CurrentTime = 0f;
                animationComponent.AnimationSpeed = 60f;
            }
        }
    }
}
