using engenious;
using OctoAwesome.Information;
using OctoAwesome.Services;
using System;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base class fo block definitions.
    /// </summary>
    public abstract class BlockDefinition : IBlockDefinition
    {
        /// <inheritdoc />
        public virtual uint SolidWall => 0x3f;

        /// <inheritdoc />
        public abstract string DisplayName { get; }

        /// <inheritdoc />
        public abstract string Icon { get; }

        /// <inheritdoc />
        public virtual int StackLimit => 100;

        /// <inheritdoc />
        public virtual int VolumePerUnit => 125;

        /// <inheritdoc />
        public virtual int VolumePerHit => 25;

        /// <inheritdoc />
        public abstract string[] Textures { get; }

        /// <inheritdoc />
        public virtual bool HasMetaData => false;

        /// <inheritdoc />
        public virtual TimeSpan TimeToVolumeReset { get; } = TimeSpan.FromSeconds(10);

        /// <inheritdoc />
        public abstract IMaterialDefinition Material { get; }
        /// <inheritdoc />
        public int Density => Material.Density;

        private readonly BoundingBox[] defaultCollisionBoxes = new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };

        /// <inheritdoc />
        public virtual BlockHitInformation Hit(BlockVolumeState blockVolume, IItem item)
        {
            //item.Definition.Hit(item, volumeState.BlockDefinition, blockHitInformation);
            var valueMined = item.Hit(Material, blockVolume.BlockInfo, blockVolume.VolumeRemaining, VolumePerHit);
            return new BlockHitInformation(valueMined != 0, valueMined, new[] { (VolumePerUnit, (IDefinition)this) });
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

    }
}
