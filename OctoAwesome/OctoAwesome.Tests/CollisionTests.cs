using engenious;
using Xunit;
using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Tests
{
    //TODO: Fixen
    /*

    using System;
    using Xunit;

    public class CollisionTests
    {
        private float gap = 0.0000f;
        private Player Player;
        private List<Index3> blocks = new List<Index3>();

        public CollisionTests()
        {
            Player = new Player(null);
            blocks.Clear();
        }

        [Fact]
        public void CollisionFromEastToWestTest()
        {

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



            Player.Velocity = new Vector3(-1, 0, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(9.6f, 10.5f, 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(8.6f, 10.6f, 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(8.5f + gap, 10.7f, 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(-0.4f, -0.1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(8.5f + gap, 10.6f, 10f + gap), Player.Position.GlobalPosition);
        }

        [Fact]
        public void CollisionFromEastToWestIntMaxTest()
        {
            int max = int.MaxValue - 1000;

            Player.Position = new Coordinate(){GlobalPosition=new Vector3(max + 10.5f, max + 10.5f, max + 10f)};

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



            Player.Velocity = new Vector3(-1, 0, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(max + 9.6f, max + 10.5f, max + 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(max + 8.6f, max + 10.6f, max + 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(-1, 0.1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(max + 8.5f + gap, max + 10.7f, max + 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(-0.4f, -0.1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(max + 8.5f + gap, max + 10.6f, max + 10f + gap), Player.Position.GlobalPosition);
        }

        [Fact]
        public void CollisionFrameSouthToNorthTest()
        {
            Player.Position = new Coordinate(){GlobalPosition=new Vector3(10.5f, 10.5f, 10f)};
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

            Player.Velocity = new Vector3(0f, -1, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(10.5f, 9.5f, 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(0f, -1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(10.5f, 8.5f, 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(0f, -1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(10.5f, 8.5f + gap, 10f + gap), Player.Position.GlobalPosition);

            Player.Velocity = new Vector3(0f, -1f, -0.1f);
            Move();
            AssertEx.Equal(Vector3.Zero, Player.Velocity);
            AssertEx.Equal(new Vector3(10.5f, 8.5f + gap, 10f + gap), Player.Position.GlobalPosition);
        }

        private void Move()
        {

            //Blocks finden die eine Kollision verursachen könnten
            int minx = (int)Math.Floor(Math.Min(
                Player.Position.BlockPosition.X - Player.Radius,
                Player.Position.BlockPosition.X - Player.Radius + Player.Velocity.X));
            int maxx = (int)Math.Ceiling(Math.Max(
                Player.Position.BlockPosition.X + Player.Radius,
                Player.Position.BlockPosition.X + Player.Radius + Player.Velocity.X));
            int miny = (int)Math.Floor(Math.Min(
                Player.Position.BlockPosition.Y - Player.Radius,
                Player.Position.BlockPosition.Y - Player.Radius + Player.Velocity.Y));
            int maxy = (int)Math.Ceiling(Math.Max(
                Player.Position.BlockPosition.Y + Player.Radius,
                Player.Position.BlockPosition.Y + Player.Radius + Player.Velocity.Y));
            int minz = (int)Math.Floor(Math.Min(
                Player.Position.BlockPosition.Z,
                Player.Position.BlockPosition.Z + Player.Velocity.Z));
            int maxz = (int)Math.Ceiling(Math.Max(
                Player.Position.BlockPosition.Z + Player.Height,
                Player.Position.BlockPosition.Z + Player.Height + Player.Velocity.Z));

            var playerplanes = CollisionPlane.GetPlayerCollisionPlanes(Player).ToList();

            bool abort = false;
            var move = Player.Velocity;
            for (int z = minz; z <= maxz && !abort; z++)
            {
                for (int y = miny; y <= maxy && !abort; y++)
                {
                    for (int x = minx; x <= maxx && !abort; x++)
                    {
                        move = Player.Velocity;

                        Index3 pos = new Index3(x, y, z);
                        Index3 blockPos = pos + Player.Position.GlobalBlockIndex;
                        if (!blocks.Contains(blockPos))
                            continue;



                        var blockplane = CollisionPlane.GetBlockCollisionPlanes(pos, Player.Velocity).ToList();

                        var planes = from pp in playerplanes
                            from bp in blockplane
                                where CollisionPlane.Intersect(bp, pp)
                                                         let distance = CollisionPlane.GetDistance(bp, pp)
                                                             where CollisionPlane.CheckDistance(distance, move)
                                                         select new { BlockPlane = bp, PlayerPlane = pp, Distance = distance };

                        foreach (var plane in planes)
                        {

                            var subvelocity = (plane.Distance );
                            var diff = Player.Velocity - subvelocity;

                            float vx;
                            float vy;
                            float vz;

                            if (plane.BlockPlane.normal.X != 0 && (Player.Velocity.X > 0 && diff.X >= 0 && subvelocity.X >= 0 || Player.Velocity.X < 0 && diff.X <= 0 && subvelocity.X <= 0))
                                vx = subvelocity.X;
                            else
                                vx = Player.Velocity.X;

                            if (plane.BlockPlane.normal.Y != 0 && (Player.Velocity.Y > 0 && diff.Y >= 0 && subvelocity.Y >= 0 || Player.Velocity.Y < 0 && diff.Y <= 0 && subvelocity.Y <= 0))
                                vy = subvelocity.Y;
                            else
                                vy = Player.Velocity.Y;

                            if (plane.BlockPlane.normal.Z != 0 && (Player.Velocity.Z > 0 && diff.Z >= 0 && subvelocity.Z >= 0 || Player.Velocity.Z < 0 && diff.Z <= 0 && subvelocity.Z <= 0))
                                vz = subvelocity.Z;
                            else
                                vz = Player.Velocity.Z;

                            Player.Velocity = new Vector3(vx, vy, vz);

                            if (vx == 0 && vy == 0 && vz == 0)
                            {
                                abort = true;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }

    */
}
