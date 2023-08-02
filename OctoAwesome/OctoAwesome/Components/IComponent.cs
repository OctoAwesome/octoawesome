using OctoAwesome.Serialization;

namespace OctoAwesome.Components
{
    /// <summary>
    /// Interface for components.
    /// </summary>
    public interface IComponent : ISerializable
    {
        /// <summary>
        /// Gets the globally unique identifier of this component. -1 means unset id
        /// </summary>
        int Id { get; set; }
        /// <summary>
        /// Gets or sets a value indicating whether this component can be sent.
        /// </summary>
        bool Sendable { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this component is enabled.
        /// </summary>
        bool Enabled { get; set; }

        /// <summary>
        /// Gets or  sets the parent that owns this component
        /// </summary>
        IComponentContainer Parent { get; set; }
    }
}
