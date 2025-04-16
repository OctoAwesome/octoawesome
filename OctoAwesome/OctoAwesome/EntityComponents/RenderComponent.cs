using OctoAwesome.Components;

using System;
using System.Collections.Generic;
using System.IO;
using OctoAwesome.Extension;
using OctoAwesome.Pooling;
using engenious;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Component for rendering entities.
    /// </summary>
    [Nooson]
    [SerializationId()]
    public partial class RenderComponent : Component, IEntityComponent, IEquatable<RenderComponent?>
    {
        private string? name, modelName, textureName;

        /// <summary>
        /// Gets or sets the name of the entity to render.
        /// </summary>
        public string Name
        {
            get => NullabilityHelper.NotNullAssert(name, $"{nameof(Name)} was not initialized!");
            set => name = NullabilityHelper.NotNullAssert(value, $"{nameof(Name)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the name of the model to render for this entity.
        /// </summary>
        public string ModelName
        {
            get => NullabilityHelper.NotNullAssert(modelName, $"{nameof(ModelName)} was not initialized!");
            set => modelName = NullabilityHelper.NotNullAssert(value, $"{nameof(ModelName)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the name of the texture to render on the model.
        /// </summary>
        public string TextureName
        {
            get => NullabilityHelper.NotNullAssert(textureName, $"{nameof(TextureName)} was not initialized!");
            set => textureName = NullabilityHelper.NotNullAssert(value, $"{nameof(TextureName)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the rotationof the model.
        /// </summary>
        public Vector3 BaseRotation { get; set; }

        /// <summary>
        /// Gets or sets the offset of the model used for rendering.
        /// </summary>
        public Vector3 BaseOffset { get; set; }

        /// <summary>
        /// Gets or sets if the texture should be loaded from the asset component or from the game components. True would load it from the asset component.
        /// </summary>
        public bool LoadFromAssetComponent { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RenderComponent"/> class.
        /// </summary>
        public RenderComponent()
        {
            Sendable = true;
        }

        /// <inheritdoc/>
        public override bool Equals(object? obj)
        {
            return Equals(obj as RenderComponent);
        }

        /// <inheritdoc/>
        public bool Equals(RenderComponent? other)
        {
            return other is not null &&
                   Name == other.Name &&
                   ModelName == other.ModelName &&
                   TextureName == other.TextureName &&
                   BaseRotation == other.BaseRotation &&
                   BaseOffset == other.BaseOffset;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return HashCode.Combine(Name, ModelName, TextureName, BaseRotation, BaseOffset);
        }

        /// <summary>Compare two render components for equality.</summary>
        /// <param name="left">The first <see cref="RenderComponent" /> to compare.</param>
        /// <param name="right">The second <see cref="RenderComponent" /> to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are considered equal; otherwise, <see langword="false" />.</returns>
        /// <seealso cref="Equals(RenderComponent)" />
        public static bool operator ==(RenderComponent? left, RenderComponent? right)
        {
            return EqualityComparer<RenderComponent>.Default.Equals(left, right);
        }

        /// <summary>Compare two render components for inequality.</summary>
        /// <param name="left">The first <see cref="RenderComponent" /> to compare.</param>
        /// <param name="right">The second <see cref="RenderComponent" /> to compare.</param>
        /// <returns><see langword="true" /> if <paramref name="left"/> and <paramref name="right"/> are not considered equal; otherwise, <see langword="false" />.</returns>
        /// <seealso cref="Equals(RenderComponent)" />
        public static bool operator !=(RenderComponent? left, RenderComponent? right)
        {
            return !(left == right);
        }
    }
}
