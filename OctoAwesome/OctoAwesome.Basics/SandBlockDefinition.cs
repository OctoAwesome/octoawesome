using OctoAwesome.Basics.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics
{
    public sealed class SandBlockDefinition : IBlockDefinition
    {
        public string Name
        {
            get { return "Sand"; }
        }

        public IEnumerable<Bitmap> Textures
        {
            get
            {
                return new[] { 
                    Resources.sand_bottom
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
            return 0;
        }

        public int GetSouthTextureIndex(IBlock block)
        {
            return 0;
        }

        public int GetWestTextureIndex(IBlock block)
        {
            return 0;
        }

        public int GetEastTextureIndex(IBlock block)
        {
            return 0;
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
            throw new NotImplementedException();
        }

        public Type GetBlockType()
        {
            return typeof(SandBlock);
        }
    }
}
