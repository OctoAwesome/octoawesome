using Microsoft.Xna.Framework;
using System.Drawing;

namespace OctoAwesome
{
    public interface IBlockDefinition : IItemDefinition
    {
        void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

        Bitmap[] Textures { get; }

        bool HasMetaData { get; }

        BoundingBox[] GetCollisionBoxes(ILocalChunkCache manager, int x, int y, int z);

        // PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);

        int GetTopTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        int GetBottomTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        int GetNorthTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        int GetSouthTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        int GetWestTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        int GetEastTextureIndex(ILocalChunkCache manager, int x, int y, int z);

        int GetTopTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        int GetBottomTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        int GetEastTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        int GetWestTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        int GetNorthTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        int GetSouthTextureRotation(ILocalChunkCache manager, int x, int y, int z);

        bool IsTopSolidWall(ILocalChunkCache manager, int x, int y, int z);

        bool IsBottomSolidWall(ILocalChunkCache manager, int x, int y, int z);

        bool IsNorthSolidWall(ILocalChunkCache manager, int x, int y, int z);

        bool IsSouthSolidWall(ILocalChunkCache manager, int x, int y, int z);

        bool IsWestSolidWall(ILocalChunkCache manager, int x, int y, int z);

        bool IsEastSolidWall(ILocalChunkCache manager, int x, int y, int z);
    }
}
