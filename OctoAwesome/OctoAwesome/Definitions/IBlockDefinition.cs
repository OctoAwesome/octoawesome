using engenious;
using OctoAwesome.Information;
using OctoAwesome.Services;
using System;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Basisinterface für eine Blockdefinition
    /// </summary>
    public interface IBlockDefinition : IInventoryable, IDefinition
    {
        /// <summary>
        /// Geplante Methode, mit der der Block auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="block">Der Block-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        BlockHitInformation Hit(BlockVolumeState blockVolume, IItem itemDefinition);

        /// <summary>
        /// Array, das alle Texturen für alle Seiten des Blocks enthält
        /// </summary>
        string[] Textures { get; }

        /// <summary>
        /// Zeigt, ob der Block-Typ Metadaten besitzt
        /// </summary>
        bool HasMetaData { get; }

        /// <summary>
        /// Liefert die Kollisionsbox für den Block. Da ein Array zurück gegeben wird, lässt sich die
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Ein Array von Kollisionsboxen</returns>
        BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z);

        /*/// <summary>
        /// Liefert die Physikalischen Paramerter, wie härte, dichte und bruchzähigkeit
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Die physikalischen Parameter</returns>
        PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);*/

        int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Oberseite (Positiv Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        int GetTextureRotation(Wall wall, ILocalChunkCache manager, int x, int y, int z);

        uint SolidWall { get; }
        TimeSpan TimeToVolumeReset { get; }
        IMaterialDefinition Material { get; }
        bool IsSolidWall(Wall wall);

        int VolumePerHit { get; }
    }
}
