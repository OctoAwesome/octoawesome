using engenious;

using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.UI.Components;

using System;
using System.IO;

namespace OctoAwesome.Basics.FunctionBlocks
{
    [SerializationId(1, 3)]
    public class Chest : FunctionalBlock
    {
        internal InventoryComponent inventoryComponent;
        internal AnimationComponent animationComponent;
        private IDisposable changedSub;

        //internal TransferUIComponent transferUiComponent;

        public Chest()
        {

        }

        public override void Deserialize(BinaryReader reader) => base.Deserialize(reader);//Doesnt get called

        public Chest(Coordinate position, float direction)
        {
            Components.AddComponent(new PositionComponent()
            {
                Position = position,
                Direction = direction
            });
        }


        private void UiComponentChanged((ComponentContainer, string, bool show) e)
        {
            if (e.show)
                return;
            animationComponent.AnimationSpeed = -60f;
            changedSub?.Dispose();

        }

        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent)
                && entity.TryGetComponent<TransferComponent>(out var transferComponent)
                && entity.TryGetComponent<UiMappingComponent>(out var lastUiMappingComponent))
            {
                transferComponent.Targets.Clear();
                transferComponent.Targets.Add(inventoryComponent);
                lastUiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));
                changedSub?.Dispose();
                changedSub = lastUiMappingComponent.Changed.Subscribe(UiComponentChanged);

                animationComponent.CurrentTime = 0f;
                animationComponent.AnimationSpeed = 60f;
            }
        }
    }
}
