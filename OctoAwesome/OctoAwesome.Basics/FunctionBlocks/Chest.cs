using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using OctoAwesome.UI.Components;
using System;
using OctoAwesome.Extension;



namespace OctoAwesome.Basics.FunctionBlocks
{
    /// <summary>
    /// Chest entity implementation.
    /// </summary>
    [SerializationId(2, 2)]
    [Nooson]
    public partial class Chest : Entity, ISerializable<Chest>
    {
        internal AnimationComponent AnimationComponent
        {
            get => NullabilityHelper.NotNullAssert(animationComponent, $"{nameof(AnimationComponent)} was not initialized!");
            set => animationComponent = NullabilityHelper.NotNullAssert(value, $"{nameof(AnimationComponent)} cannot be initialized with null!");
        }

        private IDisposable? changedSub;
        private AnimationComponent? animationComponent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Chest"/> class.
        /// </summary>
        /// <remarks>Only used for deserialization.</remarks>
        public Chest()
        {

        }


        /// <summary>
        /// Initializes a new instance of the <see cref="Chest"/> class.
        /// </summary>
        /// <param name="position">The position the chest is at.</param>
        public Chest(Coordinate position)
        {
            Components.AddIfTypeNotExists(new PositionComponent()
            {
                Position = position
            });
        }


        private void UiComponentChanged((ComponentContainer, string, bool show) e)
        {
            if (e.show)
                return;
            AnimationComponent.AnimationSpeed = -60f;
            changedSub?.Dispose();

        }

        /// <inheritdoc />
        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (TryGetComponent<UiKeyComponent>(out var ownUiKeyComponent)
                && entity.TryGetComponent<TransferComponent>(out var transferComponent)
                && entity.TryGetComponent<UiMappingComponent>(out var lastUiMappingComponent)
                && this.TryGetComponent<InventoryComponent>(out var inventoryComponent))
            {
                transferComponent.Targets.Clear();
                transferComponent.Targets.Add(inventoryComponent);
                lastUiMappingComponent.Changed.OnNext((entity, ownUiKeyComponent.PrimaryKey, true));
                changedSub?.Dispose();
                changedSub = lastUiMappingComponent.Changed.Subscribe(UiComponentChanged);

                AnimationComponent.CurrentTime = 0f;
                AnimationComponent.AnimationSpeed = 60f;
            }
        }
    }
}
