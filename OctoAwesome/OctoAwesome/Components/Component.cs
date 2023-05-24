using OctoAwesome.Components;
using OctoAwesome.Serialization;

using System.IO;
using System.Runtime.CompilerServices;

namespace OctoAwesome
{
    /// <summary>
    /// Base Class for all components.
    /// </summary>
    [Nooson]
    public abstract partial class Component : IComponent, ISerializable<Component>
    {
        /// <inheritdoc />
        public bool Enabled { get; set; }

        /// <inheritdoc />
        public bool Sendable { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        protected Component()
        {
            Enabled = true;
            Sendable = false;
        }

    }
}
