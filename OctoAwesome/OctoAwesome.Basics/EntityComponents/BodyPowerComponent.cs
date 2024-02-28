namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Component to apply power to the body of an entity.
    /// </summary>
    [Nooson]
    [SerializationId()]
    public sealed partial class BodyPowerComponent : PowerComponent
    {
        /// <summary>
        /// Gets or sets a value indicating the time a jump should take.
        /// </summary>
        public int JumpTime { get; set; }

    }
}
 