using engenious;
using OctoAwesome.Basics.EntityComponents.UIComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Serialization;
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


            //Simulation.Entities.FirstOrDefault(x=>x.)
        }

        internal void TransferUiComponentClosed(object? sender, engenious.UI.NavigationEventArgs e)
        {
            animationComponent.AnimationSpeed = -60f;
        }

        /// <inheritdoc />
        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (entity.TryGetComponent(out TransferComponent component))
            {
                component.Target = inventoryComponent;
                component.Transfering = true;

                animationComponent.CurrentTime = 0f;
                animationComponent.AnimationSpeed = 60f;
            }
        }
    }
}
