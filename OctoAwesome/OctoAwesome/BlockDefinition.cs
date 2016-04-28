using System.Drawing;
using engenious;

namespace OctoAwesome
{
    /// <summary>
    /// Eine definition eines Block-Typen
    /// </summary>
    public abstract class BlockDefinition : IBlockDefinition
    {
        /// <summary>
        /// Der Name des Block-Typen
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Icon für die Toolbar
        /// </summary>
        public abstract Bitmap Icon { get; }

        /// <summary>
        /// Die maximale Stackgrösse
        /// </summary>
        public virtual int StackLimit { get { return 100; } }

        /// <summary>
        /// Array, das alle Texturen für alle Seiten des Blocks enthält
        /// </summary>
        public abstract Bitmap[] Textures { get; }

        /// <summary>
        /// Zeigt, ob der Block-Typ Metadaten besitzt
        /// </summary>
        public virtual bool HasMetaData { get { return false; } }

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
        {
            return new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };
        }

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Oberseite (Positiv Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        public virtual int GetTopTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Unterseite (Negativ Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        public virtual int GetBottomTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Nordseite (Positiv Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        public virtual int GetNorthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Südseite (negativ Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        public virtual int GetSouthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Westseite (Negativ X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        public virtual int GetWestTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Texturindex für das Array <see cref="Textures"/> für die Ostseite (Positiv X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Index für das Array <see cref="Textures"/></returns>
        public virtual int GetEastTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Oberseite (Positiv Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Unterseite (Negativ Z) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Ostseite (Positiv X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Westseite (Negativ X) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Nordseite (Positiv Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Rotation der Textur in 90° Schritten für die Südseite (Negativ Y) des Blocks
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>Rotation der Textur in 90° Schritten</returns>
        public virtual int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        /// <summary>
        /// Gibt an, ob die Oberseite (Positiv Z) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        public virtual bool IsTopSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        /// <summary>
        /// Gibt an, ob die Unterseite (Negativ Z) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        public virtual bool IsBottomSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        /// <summary>
        /// Gibt an, ob die Nordseite (Positiv Y) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        public virtual bool IsNorthSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        /// <summary>
        /// Gibt an, ob die Südseite (Negativ Y) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        public virtual bool IsSouthSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        /// <summary>
        /// Gibt an, ob die Westseite (Positiv X) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        public virtual bool IsWestSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        /// <summary>
        /// Gibt an, ob die Ostseite (Negativ X) undurchsichtig ist, also Blöcke dahinter nicht gesehen werden können
        /// </summary>
        /// <param name="manager">[Bitte ergänzen]</param>
        /// <param name="x">X-Anteil der Koordinate des Blocks</param>
        /// <param name="y">Y-Anteil der Koordinate des Blocks</param>
        /// <param name="z">Z-Anteil der Koordinate des Blocks</param>
        /// <returns>True, wenn die Wand undurchsichtig ist</returns>
        public virtual bool IsEastSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }
    }
}
