using Microsoft.Xna.Framework;
using System;
using System.Drawing;

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
        /// 
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public abstract PhysicalProperties GetProperties(ILocalChunkCache manager, int x, int y, int z);

        public abstract void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

        public virtual BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z)
        {
            return new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) };
        }

        public virtual int GetTopTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetBottomTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetNorthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetSouthTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetWestTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetEastTextureIndex(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z)
        {
            return 0;
        }

        public virtual bool IsTopSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsBottomSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsNorthSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsSouthSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsWestSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }

        public virtual bool IsEastSolidWall(ILocalChunkCache manager, int x, int y, int z)
        {
            return true;
        }
    }

    public interface IUpdateable
    {
        void Tick(ILocalChunkCache manager, int x, int y, int z);
    }
}
