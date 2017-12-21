using System.Drawing;
using engenious;
using System.Collections.Generic;

namespace OctoAwesome
{
    //TODO: #CleanUp -> Default values or not complete?
    //more generic please!
    /// <summary>
    /// Eine definition eines Block-Typen
    /// </summary>
    public abstract class BlockDefinition : IBlockDefinition
    {
        public virtual uint SolidWall => 0x3f;

        /// <summary>
        /// Der Name des Block-Typen
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Icon für die Toolbar
        /// </summary>
        public abstract string Icon { get; }

        /// <summary>
        /// Die maximale Stackgrösse
        /// </summary>
        public virtual int StackLimit => 100;

        /// <summary>
        /// Gibt das Volumen für eine Einheit an.
        /// </summary>
        public virtual decimal VolumePerUnit => 125;

        /// <summary>
        /// Array, das alle Texturen für alle Seiten des Blocks enthält
        /// </summary>
        public abstract string[] Textures { get; }

        /// <summary>
        /// Zeigt, ob der Block-Typ Metadaten besitzt
        /// </summary>
        public virtual bool HasMetaData => false;

        /// <summary>
        /// Liefert die Physikalischen Paramerter, wie härte, dichte und bruchzähigkeit
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Die physikalischen Parameter</returns>
        public abstract PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z);

        /// <summary>
        /// Geplante Methode, mit der der Block auf Interaktion von aussen reagieren kann.
        /// </summary>
        /// <param name="block">Der Block-Typ des interagierenden Elements</param>
        /// <param name="itemProperties">Die physikalischen Parameter des interagierenden Elements</param>
        public abstract void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

        /// <summary>
        /// Liefert die Kollisionsbox für den Block. Da ein Array zurück gegeben wird, lässt sich die 
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Ein Array von Kollisionsboxen</returns>
        public virtual BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z)
            => new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };

        public virtual int GetTextureIndex(Wall wall, ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Oberseite (Positiv Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Unterseite (Negativ Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Ostseite (Positiv X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Westseite (Negativ X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Nordseite (Positiv Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z) => 0;

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Südseite (Negativ Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z) => 0;

        public bool IsSolidWall(Wall wall) => (SolidWall & (1 << (int)wall)) != 0;
    }
}
