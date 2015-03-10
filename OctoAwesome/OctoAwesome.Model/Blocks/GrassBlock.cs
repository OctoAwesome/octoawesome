using Microsoft.Xna.Framework;
using OctoAwesome.Model.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Model.Blocks
{
    [BlockDefinition(Name = "Grass")]
    public class GrassBlock : Block
    {
        public static Bitmap Texture { get { return Resources.grass_center; } }
    }
}
