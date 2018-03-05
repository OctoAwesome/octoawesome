using engenious;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Interface for controlling an entity
    /// </summary>
    public interface IEntityController
    {
        // TODO: more ui inputs and data from the user.
        /// <summary>
        /// Pressed numbers on Keyboard (0 to 9)
        /// </summary>
        bool[] SlotInput { get; }
        /// <summary>
        /// Indicates that Slots have to be shifted left
        /// </summary>
        bool SlotLeftInput { get; set; }
        /// <summary>
        /// Indicates that Slots have to be shifted right
        /// </summary>
        bool SlotRightInput { get; set; }
        /// <summary>
        /// Input for interaction
        /// </summary>
        bool InteractInput { get; set; }
        /// <summary>
        /// Input for apply
        /// </summary>
        bool ApplyInput { get; set; }
        /// <summary>
        /// Input for Jump
        /// </summary>
        bool JumpInput { get; set; }
        /// <summary>
        /// Tilt feedback in world coordinatesystem
        /// </summary>
        float Tilt { get; set; }
        /// <summary>
        /// Yaw feedback in wolrd coordinatesystem
        /// </summary>
        float Yaw { get; set; }
        /// <summary>
        /// Roll feedback in wolrd coordinatesystem
        /// </summary>
        float Roll { get; set; }
        /// <summary>
        /// Move direction in Enttiy coordinatesystem
        /// </summary>
        Vector2 MoveInput { get; }
        /// <summary>
        /// Head input in Entity coordinatesystem
        /// </summary>
        Vector2 HeadInput { get; }
        /// <summary>
        /// Index of Block to interact
        /// </summary>
        Index3? SelectedBlock { get; }
        /// <summary>
        /// Selected point of the block
        /// </summary>
        Vector2? SelectedPoint { get; }
        /// <summary>
        /// Selected side of the block
        /// </summary>
        OrientationFlags SelectedSide { get; }
        /// <summary>
        /// Selected edge o the block
        /// </summary>
        OrientationFlags SelectedEdge { get; }
        /// <summary>
        /// Selected corner of the block
        /// </summary>
        OrientationFlags SelectedCorner { get; }
        /// <summary>
        /// Indicats that the controller can be freezed
        /// </summary>
        bool CanFreeze { get; }
        /// <summary>
        /// Indicates that the controller is freezed or freez the controller
        /// </summary>
        bool Freezed { get; set; }
    }
}
