using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    public class GrassBlock : IBlock
    {
        public static Bitmap Texture { get { return Resources.grass_center; } }
    }
}
