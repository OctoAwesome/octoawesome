using Microsoft.Xna.Framework;
using System;
using System.Drawing;

namespace OctoAwesome
{
    public abstract class BlockDefinition : IBlockDefinition
    {
        public abstract string Name { get; }

        public abstract Bitmap Icon { get; }

        public virtual int StackLimit { get { return 100; } }

        public abstract Bitmap[] Textures { get; }

        public virtual bool HasMetaData { get { return false; } }

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
