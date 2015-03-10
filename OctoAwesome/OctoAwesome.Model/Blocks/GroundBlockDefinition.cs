using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public sealed class GroundBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Ground"; }
        }

        public Bitmap TopTexture
        {
            get { return Resources.ground_top; }
        }

        public Bitmap BottomTexture
        {
            get { return Resources.ground_bottom; }
        }

        public Bitmap SideTexture
        {
            get { return Resources.ground_side; }
        }

        public IBlock GetInstance()
        {
            return new GroundBlock();
        }

        public Type GetBlockType()
        {
            return typeof(GroundBlock);
        }
    }
}
