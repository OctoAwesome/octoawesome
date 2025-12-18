using OctoAwesome.Serialization;
using OctoAwesome.Components;

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;

namespace OctoAwesome.Components
{
    /// <summary>
    /// Base Class for all components.
    /// </summary>
    [Nooson]
    public abstract partial class Component : IComponent, ISerializable<Component>, IEquatable<Component?>
    {
        /// <inheritdoc />
        public int Id { get; set; } = -1; //Maybe only setable by ComponentContainer somehow somewhere

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

                if (value == parent || value is null || parent is not null)
                    return;

                var type = value.GetType();

                var serId = type.SerializationId();
                if (serId > 0)
                    ParentTypeId = serId;
                OnParentSetting(value);
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

        /// <summary>
        /// Current version of the component, starts at 1, because 0 means invalid
        /// </summary>
        public uint Version => version;

        private uint version = 1;
        private IComponentContainer parent;

        /// <summary>
        /// Initializes a new instance of the <see cref="Component"/> class.
        /// </summary>
        protected Component()
        {
            Enabled = true;
            Sendable = false;
        }

        public void IncrementVersion(bool preventSend = false)
        {
            Interlocked.Increment(ref version);
            if (!preventSend && Sendable)
                Parent?.Simulation?.FooBbqSimulationComponent.Add(this);
        }

        protected void ChangeProperty<T>(ref T field, T value)
        {
            field = value;
            Interlocked.Increment(ref version);
        }

        /// <summary>
        /// Gets called onec, when the parent is about to be set
        /// </summary>
        protected internal virtual void OnParentSetting(IComponentContainer newParent)
        {

        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as Component);
        }

        public bool Equals(Component? other)
        {
            return other is not null &&
                   Id == other.Id;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id);
        }

        public static bool operator ==(Component? left, Component? right)
        {
            return EqualityComparer<Component>.Default.Equals(left, right);
        }

        public static bool operator !=(Component? left, Component? right)
        {
            return !(left == right);
        }
    }
}
