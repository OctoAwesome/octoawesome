using engenious;

using OctoAwesome.Serialization;

using System.IO;

using NonSucking.Framework.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Entity that is simulated on a remote server.
    /// </summary>
    [Nooson]
    public partial class RemoteEntity : Entity, ISerializable<RemoteEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEntity"/> class.
        /// </summary>
        public RemoteEntity()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoteEntity"/> class.
        /// </summary>
        /// <param name="originEntity">The origin entity that is controlled by the remote server.</param>
        public RemoteEntity(Entity originEntity)
        {
            foreach (var component in Components)
            {
                if (component.Sendable)
                    Components.AddIfTypeNotExists(component);
            }
            Id = originEntity.Id;
        }

        /// <inheritdoc />
        protected override void OnInteract(GameTime gameTime, Entity entity) => throw new System.NotImplementedException();
    }
}
