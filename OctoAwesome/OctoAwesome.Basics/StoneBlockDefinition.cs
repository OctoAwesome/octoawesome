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

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { 
                    Resources.stone_bottom, 
                    Resources.stone_side
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

        public IBlock GetInstance(OrientationFlags orientation)
        {
            return new StoneBlock();
        }


        public Type GetBlockType()
        {
            return typeof(StoneBlock);
        }
    }
}
