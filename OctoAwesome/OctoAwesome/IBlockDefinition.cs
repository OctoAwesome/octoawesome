using engenious;
using System;

namespace OctoAwesome
{
    /// <summary>
    /// Basisinterface für eine Blockdefinition
    /// </summary>
    public interface IBlockDefinition : IInventoryableDefinition, IDefinition
    {
        /// <summary>
        /// Geplante Methode, mit der der Block auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="block">Der Block-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

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

        /// <summary>
        /// Returns the Texture index of given Side (Wall)
        /// </summary>
        /// <param name="wall">Side of the Block</param>
        /// <param name="manager"><see cref="ILocalChunkCache"/></param>
        /// <param name="x">X Cooridnate in the Chunk</param>
        /// <param name="y">Y Coordinate in the Chunk</param>
        /// <param name="z">Z Coordinate in the Chunk</param>
        /// <returns></returns>
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

        bool IsSolidWall(Wall wall);
    }
}
