using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class WaterBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Water"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { 
                    Resources.water_top, 
                    Resources.water_side
                };
            }
        }

        //public Bitmap TopTexture
        //{
        //    get { return Resources.water_top; }
        //}

        //public Bitmap BottomTexture
        //{
        //    get { return Resources.water_bottom; }
        //}

        //public Bitmap SideTexture
        //{
        //    get { return Resources.water_side; }
        //}

        public IBlock GetInstance()
        {
            return new WaterBlock();
        }

        public Type GetBlockType()
        {
            return typeof(WaterBlock);
        }
    }
}
