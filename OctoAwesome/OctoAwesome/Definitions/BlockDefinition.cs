using engenious;

using OctoAwesome.Chunking;
using OctoAwesome.Information;
using OctoAwesome.Services;

using System;
using System.Linq;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base class fo block definitions.
    /// </summary>
    public class BlockDefinition : IBlockDefinition
    {
        /// <inheritdoc />
        public virtual uint SolidWall => 0x3f;

        /// <inheritdoc />
        public virtual string DisplayName { get; init; }

        /// <inheritdoc />
        public virtual string Icon { get; init; }

        /// <inheritdoc />
        public virtual int StackLimit => 100;

        /// <inheritdoc />
        public virtual int VolumePerUnit => 125;

        /// <inheritdoc />
        public virtual int VolumePerHit => 25;

        /// <inheritdoc />
        public virtual string[] Textures { get; init; }

        /// <inheritdoc />
        public virtual bool HasMetaData { get; init; } = false;

        /// <inheritdoc />
        public virtual TimeSpan TimeToVolumeReset { get; init; } = TimeSpan.FromSeconds(10);

        /// <inheritdoc />        
        [JsonConverter(typeof(TypesConverter<MaterialDefinition>)), JsonInclude, JsonPropertyName("Material")]
        public virtual IMaterialDefinition Material { get; init; }

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        /// <inheritdoc />
        public int Density => Material.Density;

        private readonly BoundingBox[] defaultCollisionBoxes = [new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1))];

        /// <inheritdoc />
        public virtual BlockHitInformation Hit(BlockVolumeState blockVolume, IItem item)
        {
            //item.Definition.Hit(item, volumeState.BlockDefinition, blockHitInformation);
            var valueMined = item.Hit(Material, blockVolume.BlockInfo, blockVolume.VolumeRemaining, VolumePerHit);
            return new BlockHitInformation(valueMined != 0, valueMined, new[] { (VolumePerUnit, (IDefinition)this) });
        }
        /// <summary>
        /// Methode, mit der der Block auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="blockVolume">Der Block-Typ des interagierenden Elements</param>
        /// <param name="item">Die physikalischen Parameter des interagierenden Elements</param>
        public virtual BlockHitInformation Apply(BlockVolumeState blockVolume, IItem item)
        {
            //item.Definition.Hit(item, volumeState.BlockDefinition, blockHitInformation);
            var applied = item.Interact(Material, blockVolume.BlockInfo, blockVolume.VolumeRemaining);
            return new BlockHitInformation(applied != 0, applied, new[] { (VolumePerUnit, (IDefinition)this) });
        }

        /// <inheritdoc />
        public virtual BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z)
            => defaultCollisionBoxes;

        /// <inheritdoc />
        public virtual int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <inheritdoc />
        public virtual int GetTextureRotation(Wall wall, ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <inheritdoc />
        public bool IsSolidWall(Wall wall) => (SolidWall & (1 << (int)wall)) != 0;

        /// <summary>
        /// Get the current definiton for this definition
        /// </summary>
        /// <returns>Current <see cref="BlockDefinition"/></returns>
        public IDefinition GetDefinition() => this;

    }   
}
