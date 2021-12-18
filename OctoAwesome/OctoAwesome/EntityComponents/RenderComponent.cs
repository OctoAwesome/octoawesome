using OctoAwesome.Components;
using System.IO;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for rendering entities.
    /// </summary>
    public class RenderComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// Gets or sets the name of the entity to render.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the name of the model to render for this entity.
        /// </summary>
        public string ModelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the texture to render on the model.
        /// </summary>
        public string TextureName { get; set; }

        /// <summary>
        /// Gets or sets the rotation of the z-axis of the model.
        /// </summary>
        public float BaseZRotation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        public RenderComponent()
        {
            Sendable = true;
        }

        /// <inheritdoc />
        public override void Serialize(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write(ModelName);
            writer.Write(TextureName);
            writer.Write(BaseZRotation);
            base.Serialize(writer);
        }

        /// <inheritdoc />
        public override void Deserialize(BinaryReader reader)
        {
            Name = reader.ReadString();
            ModelName = reader.ReadString();
            TextureName = reader.ReadString();
            BaseZRotation = reader.ReadSingle();
            base.Deserialize(reader);
        }
    }
}
