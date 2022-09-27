using engenious;

using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Entity that is simulated on a remote server.
    /// </summary>
    public class RemoteEntity : Entity
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
                    Components.AddComponent(component);
            }
            Id = originEntity.Id;
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            Components.Serialize(writer);
            base.Serialize(writer);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            Components.Deserialize(reader);
            base.Deserialize(reader);
        }

        /// <inheritdoc />
        protected override void OnInteract(GameTime gameTime, Entity entity) => throw new System.NotImplementedException();
    }
}
