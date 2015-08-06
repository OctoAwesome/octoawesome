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

        public Bitmap Icon
        {
            get { return Resources.wood_bottom; }
        }

        public PhysicalProperties GetProperties(IBlock block)
        {
            return new PhysicalProperties()
            {
                Density = 0.3f,
                FractureToughness = 0.3f,
                Granularity = 0.9f,
                Hardness = 0.1f
            };
        }

        public void Hit(IBlock block, PhysicalProperties itemProperties)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { 
                    Resources.water_bottom, 
                    Resources.water_side
                };
            }
        }

        public int GetTopTextureIndex(IBlock block)
        {
            return 0;
        }

        public int GetBottomTextureIndex(IBlock block)
        {
            return 0;
        }

        public int GetNorthTextureIndex(IBlock block)
        {
            return 1;
        }

        public int GetSouthTextureIndex(IBlock block)
        {
            return 1;
        }

        public int GetWestTextureIndex(IBlock block)
        {
            return 1;
        }

        public int GetEastTextureIndex(IBlock block)
        {
            return 1;
        }

        public int GetTopTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetBottomTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetEastTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetWestTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetNorthTextureRotation(IBlock block)
        {
            return 0;
        }

        public int GetSouthTextureRotation(IBlock block)
        {
            return 0;
        }

        public bool IsTopSolidWall(IBlock block)
        {
            return false;
        }

        public bool IsBottomSolidWall(IBlock block)
        {
            return false;
        }

        public bool IsNorthSolidWall(IBlock block)
        {
            return false;
        }

        public bool IsSouthSolidWall(IBlock block)
        {
            return false;
        }

        public bool IsWestSolidWall(IBlock block)
        {
            return false;
        }

        public bool IsEastSolidWall(IBlock block)
        {
            return false;
        }

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new WaterBlock();
        }

        public Type GetBlockType()
        {
            return typeof(WaterBlock);
        }
    }
}
