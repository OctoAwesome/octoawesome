using Microsoft.Xna.Framework;
using System.Drawing;

namespace OctoAwesome
{
    public interface IBlockDefinition : IItemDefinition
    {
        void Hit(IBlockDefinition block, PhysicalProperties itemProperties);

        Bitmap[] Textures { get; }

        BoundingBox[] GetCollisionBoxes(IPlanetResourceManager manager, int x, int y, int z);

        // PhysicalProperties GetProperties(IPlanetResourceManager manager, int x, int y, int z);

        int GetTopTextureIndex(IPlanetResourceManager manager, int x, int y, int z);

        int GetBottomTextureIndex(IPlanetResourceManager manager, int x, int y, int z);

        int GetNorthTextureIndex(IPlanetResourceManager manager, int x, int y, int z);

        int GetSouthTextureIndex(IPlanetResourceManager manager, int x, int y, int z);

        int GetWestTextureIndex(IPlanetResourceManager manager, int x, int y, int z);

        int GetEastTextureIndex(IPlanetResourceManager manager, int x, int y, int z);

        int GetTopTextureRotation(IPlanetResourceManager manager, int x, int y, int z);

        int GetBottomTextureRotation(IPlanetResourceManager manager, int x, int y, int z);

        int GetEastTextureRotation(IPlanetResourceManager manager, int x, int y, int z);

        int GetWestTextureRotation(IPlanetResourceManager manager, int x, int y, int z);

        int GetNorthTextureRotation(IPlanetResourceManager manager, int x, int y, int z);

        int GetSouthTextureRotation(IPlanetResourceManager manager, int x, int y, int z);

        bool IsTopSolidWall(IPlanetResourceManager manager, int x, int y, int z);

        bool IsBottomSolidWall(IPlanetResourceManager manager, int x, int y, int z);

        bool IsNorthSolidWall(IPlanetResourceManager manager, int x, int y, int z);

        bool IsSouthSolidWall(IPlanetResourceManager manager, int x, int y, int z);

        bool IsWestSolidWall(IPlanetResourceManager manager, int x, int y, int z);

        bool IsEastSolidWall(IPlanetResourceManager manager, int x, int y, int z);
    }
}
