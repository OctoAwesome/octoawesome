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

        Bitmap TopTexture { get; }
        
        Bitmap BottomTexture { get; }
        
        Bitmap SideTexture { get; }

        IBlock GetInstance();

        Type GetBlockType();
    }
}
