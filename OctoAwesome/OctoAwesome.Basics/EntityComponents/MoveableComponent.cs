﻿using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents
{
    /// <summary>
    /// Component for entities that can be moved.
    /// </summary>
    [Nooson]
    [SerializationId(2, 12)]
    public sealed partial class MoveableComponent : Component, IEntityComponent
    {
        /// <summary>
        /// Gets or sets the velocity of the entity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        /// <summary>
        /// Gets or sets the position movement vector of the entity.
        /// </summary>
        public Vector3 PositionMove { get; set; }

        /// <summary>
        /// Gets or sets the external forces applied to the entity.
        /// </summary>
        public Vector3 ExternalForces { get; set; }

        /// <summary>
        /// Gets or sets the external powers applied to the entity.
        /// </summary>
        public Vector3 ExternalPowers { get; set; }

    }
}
