using engenious;

using OctoAwesome.Serialization;

using System.IO;

using NonSucking.Framework.Serialization;
using OctoAwesome.Components;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Entity that is simulated on a remote server.
    /// </summary>
    public partial class RemoteEntity : Entity
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEntity"/> class.
        /// </summary>
        public RemoteEntity() : base()
        {

        }
        public RemoteEntity(Guid id, ComponentList<IComponent> components) : base(id, components)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEntity"/> class.
        /// </summary>
        /// <param name="originEntity">The origin entity that is controlled by the remote server.</param>
        public RemoteEntity(Entity originEntity) : this()
        {
            Simulation = originEntity.Simulation;
            using var ms = Serializer.Manager.GetStream();
            using var bw = new BinaryWriter(ms);
            originEntity.Components.SerializeSendables(bw);
            ms.Position = 0;
            using var br = new BinaryReader(ms);
            Components = ComponentList<IComponent>.DeserializeStatic(br);
    
            Id = originEntity.Id;
        }
    }
}
