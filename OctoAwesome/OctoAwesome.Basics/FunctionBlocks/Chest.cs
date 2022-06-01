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
    /// <summary>
    /// Chest functional block implementation.
    /// </summary>
    [SerializationId(1, 3)]
    public class Chest : FunctionalBlock
    {
        internal AnimationComponent animationComponent;
        private IDisposable changedSub;

        //internal TransferUIComponent transferUiComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chest"/> class.
        /// </summary>
        /// <remarks>Only used for deserialization.</remarks>
        public Chest()
        {

        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            //Doesnt get called
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Chest"/> class.
        /// </summary>
        /// <param name="position">The position the chest is at.</param>
        public Chest(Coordinate position)
        {
            Components.AddComponent(new PositionComponent()
            {
                Position = position
            });
        }


        private void UiComponentChanged((ComponentContainer, string, bool show) e)
        {
            if (e.show)
                return;
            animationComponent.AnimationSpeed = -60f;
            changedSub?.Dispose();

        }

        /// <inheritdoc />
        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (TryGetComponent<UiKeyComponent>(out  var ownUiKeyComponent) 
                && entity.TryGetComponent<TransferComponent>(out var transferComponent) 
                && entity.TryGetComponent<UiMappingComponent>(out var lastUiMappingComponent)
                && this.TryGetComponent<InventoryComponent>(out var inventoryComponent))
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
