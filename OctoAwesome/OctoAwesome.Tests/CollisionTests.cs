using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Tests
{
    [TestClass]
    public class CollisionTests
    {
        private float gap = 0.00001f;
        private Coordinate player;
        private float playerRadius = 0.5f;
        private float playerHeight = 2f;
        private Vector3 move;
        private BlockDefinition blockDefinition = new TestBlockDefinition();
        private List<Index3> blocks = new List<Index3>();

        [TestInitialize]
        public void Init()
        {
            blocks.Clear();
        }

        [TestCleanup]
        public void Cleanup()
        {
        }

        [TestMethod]
        public void CollisionFromEastToWestTest()
        {
            player = new Coordinate();
            player.GlobalPosition = new Vector3(10.6f, 10.5f, 10f);

            // Wand
            blocks.Add(new Index3(7, 9, 10));
            blocks.Add(new Index3(7, 10, 10));
            blocks.Add(new Index3(7, 11, 10));

            // boden y-1
            blocks.Add(new Index3(7, 9, 9));
            blocks.Add(new Index3(8, 9, 9));
            blocks.Add(new Index3(9, 9, 9));
            blocks.Add(new Index3(10, 9, 9));
            blocks.Add(new Index3(11, 9, 9));

            // Boden mitte
            blocks.Add(new Index3(7, 10, 9));
            blocks.Add(new Index3(8, 10, 9));
            blocks.Add(new Index3(9, 10, 9));
            blocks.Add(new Index3(10, 10, 9));
            blocks.Add(new Index3(11, 10, 9));

            // boden y+1
            blocks.Add(new Index3(7, 11, 9));
            blocks.Add(new Index3(8, 11, 9));
            blocks.Add(new Index3(9, 11, 9));
            blocks.Add(new Index3(10, 11, 9));
            blocks.Add(new Index3(11, 11, 9));


            move = new Vector3(-1, 0, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(9.6f, 10.5f, 10f + gap), player.GlobalPosition);

            move = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(8.6f, 10.6f, 10f + gap), player.GlobalPosition);

            move = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(8.5f + gap, 10.7f, 10f + gap), player.GlobalPosition);

            move = new Vector3(-0.4f, -0.1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(8.5f + gap, 10.6f, 10f + gap), player.GlobalPosition);
        }

        [TestMethod]
        public void CollisionFromEastToWestIntMaxTest()
        {
            int max = int.MaxValue - 1000;

            player = new Coordinate();
            player.GlobalPosition = new Vector3(max + 10.5f, max + 10.5f, max + 10f);

            // Wand
            blocks.Add(new Index3(max + 7, max + 9, max + 10));
            blocks.Add(new Index3(max + 7, max + 10, max + 10));
            blocks.Add(new Index3(max + 7, max + 11, max + 10));

            // boden y-1
            blocks.Add(new Index3(max + 7, max + 9, max + 9));
            blocks.Add(new Index3(max + 8, max + 9, max + 9));
            blocks.Add(new Index3(max + 9, max + 9, max + 9));
            blocks.Add(new Index3(max + 10, max + 9, max + 9));
            blocks.Add(new Index3(max + 11, max + 9, max + 9));

            // Boden mitte
            blocks.Add(new Index3(max + 7, max + 10, max + 9));
            blocks.Add(new Index3(max + 8, max + 10, max + 9));
            blocks.Add(new Index3(max + 9, max + 10, max + 9));
            blocks.Add(new Index3(max + 10, max + 10, max + 9));
            blocks.Add(new Index3(max + 11, max + 10, max + 9));

            // boden y+1
            blocks.Add(new Index3(max + 7, max + 11, max + 9));
            blocks.Add(new Index3(max + 8, max + 11, max + 9));
            blocks.Add(new Index3(max + 9, max + 11, max + 9));
            blocks.Add(new Index3(max + 10, max + 11, max + 9));
            blocks.Add(new Index3(max + 11, max + 11, max + 9));


            move = new Vector3(-1, 0, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(max + 9.6f, max + 10.5f, max + 10f + gap), player.GlobalPosition);

            move = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(max + 8.6f, max + 10.6f, max + 10f + gap), player.GlobalPosition);

            move = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(max + 8.5f + gap, max + 10.7f, max + 10f + gap), player.GlobalPosition);

            move = new Vector3(-0.4f, -0.1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(max + 8.5f + gap, max + 10.6f, max + 10f + gap), player.GlobalPosition);
        }

        [TestMethod]
        public void CollisionFrameSouthToNorthTest()
        {
            player = new Coordinate();
            player.GlobalPosition = new Vector3(10.5f, 10.5f, 10f);

            // Wand
            blocks.Add(new Index3(9, 7, 10));
            blocks.Add(new Index3(10, 7, 10));
            blocks.Add(new Index3(11, 7, 10));

            // boden y-1
            blocks.Add(new Index3(9, 7, 9));
            blocks.Add(new Index3(9, 8, 9));
            blocks.Add(new Index3(9, 9, 9));
            blocks.Add(new Index3(9, 10, 9));
            blocks.Add(new Index3(9, 11, 9));

            // Boden mitte
            blocks.Add(new Index3(10, 7, 9));
            blocks.Add(new Index3(10, 8, 9));
            blocks.Add(new Index3(10, 9, 9));
            blocks.Add(new Index3(10, 10, 9));
            blocks.Add(new Index3(10, 11, 9));

            // boden y+1
            blocks.Add(new Index3(11, 7, 9));
            blocks.Add(new Index3(11, 8, 9));
            blocks.Add(new Index3(11, 9, 9));
            blocks.Add(new Index3(11, 10, 9));
            blocks.Add(new Index3(11, 11, 9));

            move = new Vector3(0f, -1, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(10.5f, 9.5f, 10f + gap), player.GlobalPosition);

            move = new Vector3(0f, -1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(10.5f, 8.5f, 10f + gap), player.GlobalPosition);

            move = new Vector3(0f, -1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(10.5f, 8.5f + gap, 10f + gap), player.GlobalPosition);

            move = new Vector3(0f, -1f, -0.1f);
            Move();
            AssertEx.AreEqual(Vector3.Zero, move);
            AssertEx.AreEqual(new Vector3(10.5f, 8.5f + gap, 10f + gap), player.GlobalPosition);
        }

        private void Move()
        {
            bool collision = false;
            int loop = 0;

            do
            {
                BoundingBox playerBox = new BoundingBox(
                    new Vector3(
                        player.GlobalPosition.X - playerRadius,
                        player.GlobalPosition.Y - playerRadius,
                        player.GlobalPosition.Z),
                    new Vector3(
                        player.GlobalPosition.X + playerRadius,
                        player.GlobalPosition.Y + playerRadius,
                        player.GlobalPosition.Z + playerHeight));

                collision = false;
                float min = 1f;
                Axis minAxis = Axis.None;

                foreach (var pos in blocks)
                {
                    Axis? localAxis;
                    float? moveFactor = Block.Intersect(blockDefinition.GetCollisionBoxes(null, pos.X, pos.Y, pos.Z),
                        pos, playerBox, move, out localAxis);

                    if (moveFactor.HasValue && moveFactor.Value < min)
                    {
                        collision = true;
                        min = moveFactor.Value;
                        minAxis = localAxis.Value;
                    }
                }

                player += (move*min);
                move *= (1f - min);
                switch (minAxis)
                {
                    case Axis.X:
                        player += new Vector3(move.X > 0 ? -gap : gap, 0, 0);
                        move.X = 0f;
                        break;
                    case Axis.Y:
                        player += new Vector3(0, move.Y > 0 ? -gap : gap, 0);
                        move.Y = 0f;
                        break;
                    case Axis.Z:
                        player += new Vector3(0, 0, move.Z > 0 ? -gap : gap);
                        move.Z = 0f;
                        break;
                }

                // Koordinate normalisieren (Rundwelt)
                // player.NormalizeChunkIndexXY(planet.Size);

                loop++;
            } while (collision && loop < 3);
        }
    }
}