using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class StoneBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Stone"; }
        }

        public Bitmap TopTexture
        {
            get { return Resources.stone_top; }
        }

        public Bitmap BottomTexture
        {
            get { return Resources.stone_bottom; }
        }

        public Bitmap SideTexture
        {
            get { return Resources.stone_side; }
        }

        public IBlock GetInstance(Index3 globalPosition)
        {
            return new StoneBlock(globalPosition);
        }


        public Type GetBlockType()
        {
            return typeof(StoneBlock);
        }
    }
}
