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
        /// input for Jump
        /// </summary>
        bool JumpInput { get; set; }
        /// <summary>
        /// Tilt (Neigung des Kopfes)
        /// </summary>
        float Tilt { get; }
        /// <summary>
        /// Yaw (Horizontale Drehung)
        /// </summary>
        float Yaw { get; }
        /// <summary>
        /// Direction of Motion
        /// </summary>
        Vector3 Direction { get; }
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
    }
}
