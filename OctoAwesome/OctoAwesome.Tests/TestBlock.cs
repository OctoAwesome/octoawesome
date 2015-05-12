using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Tests
{
    public class TestBlock : Block
    {
        public TestBlock(Index3 position) : base(position) { }

        public override BoundingBox[] GetCollisionBoxes()
        {
            BoundingBox[] result = new BoundingBox[2];
            result[0] = new BoundingBox(new Vector3(0, 0, 0), new Vector3(0.5f, 1, 1));
            result[1] = new BoundingBox(new Vector3(0.5f, 0, 0), new Vector3(1, 1, 1));
            return result;
        }
    }
}
