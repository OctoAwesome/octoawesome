using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public sealed class GrassBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Grass"; }
        }

        public Bitmap TopTexture
        {
            get { return Resources.grass_top; }
        }

        public Bitmap BottomTexture
        {
            get { return Resources.grass_bottom; }
        }

        public Bitmap SideTexture
        {
            get { return Resources.grass_side; }
        }

        public IBlock GetInstance()
        {
            return new GrassBlock();
        }

        public Type GetBlockType()
        {
            return typeof(GrassBlock);
        }
    }
}
