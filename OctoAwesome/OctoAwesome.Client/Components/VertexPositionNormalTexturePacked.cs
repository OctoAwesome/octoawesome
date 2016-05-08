using engenious;
using engenious.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    struct VertexPositionNormalTexturePacked : IVertexType
    {
        //uv:(0,0),(0,1),(1,0),(1,1)
        //normal:(1,0,0),(-1,0,0)
        //      (0,1,0),(0,-1,0)
        //      (0,0,1),(0,0,-1)

        public static readonly VertexDeclaration VertexDeclaration;
        static VertexPositionNormalTexturePacked()
        {
            VertexDeclaration = new engenious.Graphics.VertexDeclaration(sizeof(uint)*2, new VertexElement(0, VertexElementFormat.Rgba32, VertexElementUsage.Position, 0), new VertexElement(sizeof(uint), VertexElementFormat.Rgba32, VertexElementUsage.Normal, 0));
        }
        public VertexPositionNormalTexturePacked(Vector3 position,Vector3 normal,Vector2 uv)
        {
            uint posX = (uint)position.X;
            uint posY = (uint)position.Y;
            uint posZ = (uint)position.Z;

            int normalX = (int)normal.X;
            int normalY = (int)normal.Y;
            int normalZ = (int)normal.Z;

            int normalExpanded = (normalX + 1) * 100 + (normalY + 1) * 10 + (normalZ + 1);

            uint normalPacked;
            switch (normalExpanded)
            {
                case 211: normalPacked = 0; break;
                case 11: normalPacked = 1; break;
                case 121: normalPacked = 2; break;
                case 101: normalPacked = 3; break;
                case 112: normalPacked = 4; break;
                case 110: normalPacked = 5; break;
                default:
                    throw new Exception("Expected error happened.");
            }

            uint uvExpanded = ((uint)uv.X << 1) | ((uint)uv.Y);

            PackedValue = posX | posY << 8 | posZ << 16 | normalPacked << 24 | uvExpanded << 28;
            PackedValue2 = ((uint)(uv.X * 65536) << 16) | (uint)(uv.Y * 65536);
        }
        public UInt32 PackedValue
        {
            get;private set;
        }
        public UInt32 PackedValue2
        {
            get;private set;
        }

        VertexDeclaration IVertexType.VertexDeclaration
        {
            get
            {
                return VertexDeclaration;
            }
        }
    }
}
