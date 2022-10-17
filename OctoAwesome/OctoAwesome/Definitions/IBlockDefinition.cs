using engenious;

using OctoAwesome.Chunking;
using OctoAwesome.Information;
using OctoAwesome.Services;

using System;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface for block definitions.
    /// </summary>
    public interface IBlockDefinition : IInventoryable, IDefinition
    {
        /// <summary>
        /// Gets block hit information for hitting a specific block with a specific item.
        /// </summary>
        /// <param name="blockVolume">The block volume information for the block to hit.</param>
        /// <param name="itemDefinition">The item to hit the block with.</param>
        BlockHitInformation Hit(BlockVolumeState blockVolume, IItem itemDefinition);


        /// <summary>
        /// Geplante Methode, mit der der Block auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="block">Der Block-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        BlockHitInformation Apply(BlockVolumeState blockVolume, IItem itemDefinition);

        /// <summary>
        /// Gets an array of texture names for all block sides.
        /// </summary>
        string[] Textures { get; }

        /// <summary>
        /// Gets a value indicating whether the block type has meta data.
        /// </summary>
        bool HasMetaData { get; }

        /// <summary>
        /// Gets the collision boxes for a block.
        /// </summary>
        /// <param name="manager">The local chunk cache to get the block properties from.</param>
        /// <param name="x">The x component of the local block position.</param>
        /// <param name="y">The y component of the local block position.</param>
        /// <param name="z">The z component of the local block position.</param>
        /// <returns>An array of collision boxes. For full blocks usually only one item.</returns>
        BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z);

        /*/// <summary>
        /// Gets the physical properties of a block.
        /// </summary>
        /// <param name="manager">The local chunk cache to get the block properties from.</param>
        /// <param name="x">The x component of the local block position.</param>
        /// <param name="y">The y component of the local block position.</param>
        /// <param name="z">The z component of the local block position.</param>
        /// <returns>The physical block properties.</returns>
        PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);*/

        /// <summary>
        /// Gets the texture index of a blocks texture.
        /// </summary>
        /// <param name="wall">The side of the block to get the texture index for.</param>
        /// <param name="manager">The local chunk cache to get the block properties from.</param>
        /// <param name="x">The x component of the local block position.</param>
        /// <param name="y">The y component of the local block position.</param>
        /// <param name="z">The z component of the local block position.</param>
        /// <returns>The texture index of the block side.</returns>
        int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Get texture rotation in 90° increments for the top surface(positive Z) of a block.
        /// </summary>
        /// <param name="wall">The side of the block to get the texture rotation for.</param>
        /// <param name="manager">The local chunk cache to get the block texture rotation from.</param>
        /// <param name="x">The x component of the local block position.</param>
        /// <param name="y">The y component of the local block position.</param>
        /// <param name="z">The z component of the local block position.</param>
        /// <returns>Rotation of the texture in 90° increments.</returns>
        int GetTextureRotation(Wall wall, ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Gets a bitset depicting which sides of the block are solid.
        /// </summary>
        /// <remarks>Use <see cref="IsSolidWall"/> to check.</remarks>
        uint SolidWall { get; }

        /// <summary>
        /// Gets the time till the block volume should reset.
        /// </summary>
        TimeSpan TimeToVolumeReset { get; }

        /// <summary>
        /// Checks whether the provided <see cref="Wall"/> is solid on the block.
        /// </summary>
        /// <param name="wall">The <see cref="Wall"/> to check.</param>
        /// <returns>A value indicating whether the provided <see cref="Wall"/> is solid on the block.</returns>
        bool IsSolidWall(Wall wall);

        /// <summary>
        /// Gets a value indicating the amount of volume that can be extracted from the block with one hit.
        /// </summary>
        int VolumePerHit { get; }
    }
}
