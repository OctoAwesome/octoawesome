using engenious;

using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Serialization;

using System.IO;

namespace OctoAwesome.Basics.FunctionBlocks
{
    [SerializationId(1, 4)]
    public class Furnace : FunctionalBlock
    {
        internal InventoryComponent inventoryComponent;
        internal AnimationComponent animationComponent;
        private TransferComponent lastUsedTransferComponent;

        public Furnace()
        {

        }

        public override void Deserialize(BinaryReader reader) => base.Deserialize(reader);//Doesnt get called

        public Furnace(Coordinate position)
        {
            Components.AddComponent(new PositionComponent()
            {
                Position = position
            });
        }



        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (entity.TryGetComponent(out lastUsedTransferComponent))
            {
                lastUsedTransferComponent.Targets.Clear();

                lastUsedTransferComponent.Targets.Add(inventoryComponent);
                lastUsedTransferComponent.Transfering = true;

                animationComponent.CurrentTime = 0f;
                animationComponent.AnimationSpeed = 60f;
            }
        }
    }
}
