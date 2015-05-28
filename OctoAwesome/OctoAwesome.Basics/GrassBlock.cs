using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public class GrassBlock : Block
    {
        public GrassBlock()
        {
            TopTexture = 0;
            BottomTexture = 1;

            NorthTexture = 2;
            SouthTexture = 2;
            WestTexture = 2;
            EastTexture = 2;
        }
    }
}
