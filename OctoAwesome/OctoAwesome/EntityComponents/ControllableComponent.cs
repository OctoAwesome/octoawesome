using engenious;
using OctoAwesome.Components;
using OctoAwesome.Serialization;
using OctoAwesome.SumTypes;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for a controllable entity.
    /// </summary>
    [SerializationId(1, 9)]
    public class ControllableComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets a value indicating whether the jump input was triggered on the last frame.
        /// </summary>
        public bool JumpInput { get; set; }

        /// <summary>
        /// Gets or sets the move input direction of the last frame.
        /// </summary>
        public Vector2 MoveInput { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether a jump is currently happening.
        /// </summary>
        public bool JumpActive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating the remaining jump time.
        /// </summary>
        public int JumpTime { get; set; }


        /// <summary>
        /// Gets or sets the selection(block/functional block/entity) of the last frame; or <c>null</c> for no selection.
        /// </summary>
        public Selection? Selection { get; set; }

        /// <summary>
        /// Gets or sets the block index to interact with; or <c>null</c> for no interaction.
        /// </summary>
        public Index3? InteractBlock { get; set; }

        /// <summary>
        /// Gets or sets the block index to apply on; or <c>null</c> for no interaction.
        /// </summary>
        public Index3? ApplyBlock { get; set; }

        /// <summary>
        /// Gets or sets application orientation.
        /// </summary>
        public OrientationFlags ApplySide { get; set; }
    }
}
