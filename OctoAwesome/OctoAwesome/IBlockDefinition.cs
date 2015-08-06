using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IBlockDefinition
    {
        string Name { get; }

        Bitmap Icon { get; }

        PhysicalProperties GetProperties(IBlock block);

        void Hit(IBlock block, PhysicalProperties itemProperties);

        IEnumerable<Bitmap> Textures { get; }

        int GetTopTextureIndex(IBlock block);

        int GetBottomTextureIndex(IBlock block);

        int GetNorthTextureIndex(IBlock block);

        int GetSouthTextureIndex(IBlock block);

        int GetWestTextureIndex(IBlock block);

        int GetEastTextureIndex(IBlock block);

        int GetTopTextureRotation(IBlock block);
        
        int GetBottomTextureRotation(IBlock block);
        
        int GetEastTextureRotation(IBlock block);
        
        int GetWestTextureRotation(IBlock block);
        
        int GetNorthTextureRotation(IBlock block);
        
        int GetSouthTextureRotation(IBlock block);

        bool IsTopSolidWall(IBlock block);

        bool IsBottomSolidWall(IBlock block);

        bool IsNorthSolidWall(IBlock block);
        
        bool IsSouthSolidWall(IBlock block);
        
        bool IsWestSolidWall(IBlock block);
        
        bool IsEastSolidWall(IBlock block);

        IBlock GetInstance(OrientationFlags orientation);

        Type GetBlockType();
    }
}
