using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    public interface IBlockRenderer
    {
        // TODO: Irgendwie muss die Texturkoordinate noch in den Renderer - aber wie?!
        VertexPositionNormalTexture[] GenerateMesh(IPlanetResourceManager manager, int x, int y, int z,
            bool blockedTop, bool blockedBottom, bool blockedNorth, bool blockedSouth, bool blockedWest, bool blockedEast);
    }
}
