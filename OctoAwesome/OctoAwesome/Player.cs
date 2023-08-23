using engenious;

using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Serialization;
using NonSucking.Framework.Serialization;
using OctoAwesome.EntityComponents;
using OctoAwesome.Components;
using System.IO;
using OctoAwesome.Extension;
using System;

namespace OctoAwesome
{
   

    /// <summary>
    /// Entity, that the user can control using input devices.
    /// </summary>
    [SerializationId(1, 1)]
    [Nooson]
    public partial class Player : Entity, IConstructionSerializable<Player>
    {
        /// <summary>
        /// The range the user can interact with in game elements e.g. <see cref="Block"/> and <see cref="Entity"/>.
        /// </summary>
        public const int SELECTIONRANGE = 8;

        /// <summary>
        /// Gets or Sets the Name of this player
        /// </summary>
        [NoosonIgnore]
        public string Name { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Player"/> class.
        /// </summary>
        public Player()
        {
        }

        public Player(Guid id, ComponentList<IComponent> components) : base(id, components)
        {
        }


    }
}
