using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class StoneBlock : Block
    {
        public StoneBlock()
        {
            TopTexture = 0;
            BottomTexture = 0;

            NorthTexture = 1;
            SouthTexture = 1;
            WestTexture = 1;
            EastTexture = 1;
        }
    }
}
