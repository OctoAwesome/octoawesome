using OctoAwesome.Components;
using OctoAwesome.Serialization;

using System;
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

        [NoosonIgnore]
        /// <inheritdoc/>
        public IComponentContainer Parent
        {
            get => parent; set
            {

                if (value is null)
                    return;
                //throw new ArgumentNullException(nameof(value));

                //if (parent.Id == value.Id)
                //    return;

                var type = value.GetType();
                if (parent is not null)
                {
                    //throw new NotSupportedException("Can not change the " + type.Name);
                    return;
                }

                var serId = type.SerializationId();
                if (serId > 0)
                    ParentTypeId = serId;
                ParentId = value.Id;
                parent = value;

            }
        }

        /// <summary>
        /// Gets the unique identifier for the <see cref="Parent"/>.
        /// </summary>
        public Guid ParentId { get; set; }

        /// <summary>
        /// Gets the instance type id.
        /// </summary>
        /// <seealso cref="SerializationIdTypeProvider"/>
        public ulong ParentTypeId { get; protected set; }

        private IComponentContainer parent;

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
