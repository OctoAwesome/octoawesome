using Microsoft.Xna.Framework;
using OctoAwesome.Components;
using OctoAwesome.Model.Blocks;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Model
{
    public sealed class World
    {
        private readonly float Gap = 0.001f;

        private readonly Vector3[] CollisionOrder = new[] 
        {
            new Vector3(0, -5, 0),
            new Vector3(0, -4, 0),
            new Vector3(0, -3, 0),
            new Vector3(0, -2, 0),

            // Block direkt unter dem Player
            new Vector3(0, -1, 0),

            // Blocks am Boden um den Player
            new Vector3(-1, -1, -1),new Vector3(1, -1, -1),new Vector3(-1, -1, 1),new Vector3(1, -1, 1),
            new Vector3(-1, -1, 0),new Vector3(1, -1, 0),new Vector3(0, -1, -1),new Vector3(0, -1, 1),

            // Kollision mit der Decke
            new Vector3(0, 6, 0),
            new Vector3(-1, 6, -1),new Vector3(1, 6, -1),new Vector3(-1, 6, 1),new Vector3(1, 6, 1),
            new Vector3(-1, 6, 0),new Vector3(1, 6, 0),new Vector3(0, 6, -1),new Vector3(0, 6, 1),
            new Vector3(0, 5, 0),                
            new Vector3(-1, 5, -1),new Vector3(1, 5, -1),new Vector3(-1, 5, 1),new Vector3(1, 5, 1),
            new Vector3(-1, 5, 0),new Vector3(1, 5, 0),new Vector3(0, 5, -1),new Vector3(0, 5, 1),
            new Vector3(0, 4, 0),                
            new Vector3(-1, -1, -1),new Vector3(1, -1, -1),new Vector3(-1, -1, 1),new Vector3(1, -1, 1),
            new Vector3(-1, 4, 0),new Vector3(1, 4, 0),new Vector3(0, 4, -1),new Vector3(0, 4, 1),
            new Vector3(0, 3, 0),                
            new Vector3(-1, 3, -1),new Vector3(1, 3, -1),new Vector3(-1, 3, 1),new Vector3(1, 3, 1),
            new Vector3(-1, 3, 0),new Vector3(1, 3, 0),new Vector3(0, 3, -1),new Vector3(0, 3, 1),
            new Vector3(0, 2, 0),                
            new Vector3(-1, 2, -1),new Vector3(1, 2, -1),new Vector3(-1, 2, 1),new Vector3(1, 2, 1),
            new Vector3(-1, 2, 0),new Vector3(1, 2, 0),new Vector3(0, 2, -1),new Vector3(0, 2, 1),
            new Vector3(0, 1, 0),                
            new Vector3(-1, 1, -1),new Vector3(1, 1, -1),new Vector3(-1, 1, 1),new Vector3(1, 1, 1),
            new Vector3(-1, 1, 0),new Vector3(1, 1, 0),new Vector3(0, 1, -1),new Vector3(0, 1, 1),
            new Vector3(0, 0, 0),                
            new Vector3(-1, 0, -1),new Vector3(1, 0, -1),new Vector3(-1, 0, 1),new Vector3(1, 0, 1),
            new Vector3(-1, 0, 0),new Vector3(1, 0, 0),new Vector3(0, 0, -1),new Vector3(0, 0, 1),
        };

        public Chunk Chunk { get; private set; }

        public Player Player { get; private set; }

        public World(IInputSet input)
        {
            Chunk = new Model.Chunk();
            Player = new Player(input);
        }

        public void Update(GameTime frameTime)
        {
            Player.ExternalForce = new Vector3(0, -20f, 0) * Player.Mass;

            Player.Update(frameTime);

            Vector3 move = Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds;

            Player.OnGround = false;

            int minx = (int)Math.Min(
                Player.Position.X - Player.Radius,
                Player.Position.X - Player.Radius + move.X);
            int maxx = (int)Math.Max(
                Player.Position.X + Player.Radius,
                Player.Position.X + Player.Radius + move.X);
            int miny = (int)Math.Min(
                Player.Position.Y,
                Player.Position.Y + move.Y);
            int maxy = (int)Math.Max(
                Player.Position.Y + Player.Height,
                Player.Position.Y + Player.Height + move.Y);
            int minz = (int)Math.Min(
                Player.Position.Z - Player.Radius,
                Player.Position.Z - Player.Radius + move.Z);
            int maxz = (int)Math.Max(
                Player.Position.Z + Player.Radius,
                Player.Position.Z + Player.Radius + move.Z);

            bool collision = false;
            int loops = 0;

            do
            {
                BoundingBox playerBox = new BoundingBox(
                    new Vector3(
                        Player.Position.X + move.X - Player.Radius,
                        Player.Position.Y + move.Y,
                        Player.Position.Z + move.Z - Player.Radius),
                    new Vector3(
                        Player.Position.X + move.X + Player.Radius,
                        Player.Position.Y + move.Y + Player.Height,
                        Player.Position.Z + move.Z + Player.Radius));

                collision = false;
                float min = 1f;
                float minGap = 0f;
                Axis minAxis = Axis.None;

                for (int z = minz; z <= maxz; z++)
                {
                    for (int y = miny; y <= maxy; y++)
                    {
                        for (int x = minx; x <= maxx; x++)
                        {
                            int ix = (int)(x + Player.Position.X + move.X);
                            int iy = (int)(y + Player.Position.Y + move.Y);
                            int iz = (int)(z + Player.Position.Z + move.Z);

                            if (x < 0 || x >= Chunk.CHUNKSIZE_X ||
                                y < 0 || y >= Chunk.CHUNKSIZE_Y ||
                                z < 0 || z >= Chunk.CHUNKSIZE_Z)
                                continue;

                            IBlock block = Chunk.Blocks[x, y, z];
                            if (block == null)
                                continue;

                            BoundingBox[] boxes = block.GetCollisionBoxes();

                            foreach (var box in boxes)
                            {
                                BoundingBox transformedBox = new BoundingBox(
                                    box.Min + new Vector3(x, y, z),
                                    box.Max + new Vector3(x, y, z));

                                // (1) Kollisionscheck
                                bool collisionX = (transformedBox.Min.X <= playerBox.Max.X && transformedBox.Max.X >= playerBox.Min.X);
                                bool collisionY = (transformedBox.Min.Y <= playerBox.Max.Y && transformedBox.Max.Y >= playerBox.Min.Y);
                                bool collisionZ = (transformedBox.Min.Z <= playerBox.Max.Z && transformedBox.Max.Z >= playerBox.Min.Z);

                                if (collisionX && collisionY && collisionZ)
                                {
                                    collision = true;

                                    // (2) Kollisionszeitpunkt ermitteln
                                    float max = 0f;
                                    Axis maxAxis = Axis.None;
                                    float maxGap = 0f;

                                    float nx = 1f;
                                    if (move.X > 0)
                                    {
                                        float diff = playerBox.Max.X - transformedBox.Min.X;
                                        if (diff < move.X)
                                        {
                                            nx = 1f - (diff / move.X);
                                            if (nx > max)
                                            {
                                                max = nx;
                                                maxAxis = Axis.X;
                                                maxGap = -Gap;
                                            }
                                        }

                                    }
                                    else if (move.X < 0)
                                    {
                                        float diff = transformedBox.Max.X - playerBox.Min.X;
                                        if (diff < -move.X)
                                        {
                                            nx = 1f - (diff / -move.X);
                                            if (nx > max)
                                            {
                                                max = nx;
                                                maxAxis = Axis.X;
                                                maxGap = Gap;
                                            }
                                        }
                                    }

                                    float ny = 1f;
                                    if (move.Y > 0)
                                    {
                                        float diff = playerBox.Max.Y - transformedBox.Min.Y;
                                        if (diff < move.Y)
                                        {
                                            ny = 1f - (diff / move.Y);
                                            if (ny > max)
                                            {
                                                max = ny;
                                                maxAxis = Axis.Y;
                                                maxGap = -Gap;
                                            }
                                        }

                                    }
                                    else if (move.Y < 0)
                                    {
                                        float diff = transformedBox.Max.Y - playerBox.Min.Y;
                                        if (diff < -move.Y) // TODO: Toleranzfall (kleine Stufe) if (diff < -move.Y + 1.1f)
                                        {
                                            ny = 1f - (diff / -move.Y);
                                            if (ny > max)
                                            {
                                                max = ny;
                                                maxAxis = Axis.Y;
                                                maxGap = Gap;
                                            }
                                        }
                                    }

                                    float nz = 1f;
                                    if (move.Z > 0)
                                    {
                                        float diff = playerBox.Max.Z - transformedBox.Min.Z;
                                        if (diff < move.Z)
                                        {
                                            nz = 1f - (diff / move.Z);
                                            if (nz > max)
                                            {
                                                max = nz;
                                                maxAxis = Axis.Z;
                                                maxGap = -Gap;
                                            }
                                        }

                                    }
                                    else if (move.Z < 0)
                                    {
                                        float diff = transformedBox.Max.Z - playerBox.Min.Z;
                                        if (diff < -move.Z)
                                        {
                                            nz = 1f - (diff / -move.Z);
                                            if (nz > max)
                                            {
                                                max = nz;
                                                maxAxis = Axis.Z;
                                                maxGap = Gap;
                                            }
                                        }
                                    }

                                    if (max < min)
                                    {
                                        min = max;
                                        minAxis = maxAxis;
                                        minGap = maxGap;
                                    }
                                }
                            }
                        }
                    }
                }

                if (collision)
                {
                    Vector3 movePart = move * min;
                    Player.Position += movePart;
                    move *= 1 - min;
                    switch (minAxis)
                    {
                        case Axis.X:
                            move.X = 0;
                            Player.Position = Player.Position + new Vector3(minGap, 0, 0);
                            Player.Velocity *= new Vector3(0, 1, 1);
                            break;
                        case Axis.Y:
                            move.Y = 0;
                            Player.Position = Player.Position + new Vector3(0, minGap, 0);
                            Player.Velocity *= new Vector3(1, 0, 1);
                            //if (Gap > 0) Player.OnGround = true;
                            break;
                        case Axis.Z:
                            move.Z = 0;
                            Player.Position = Player.Position + new Vector3(0, 0, minGap);
                            Player.Velocity *= new Vector3(1, 1, 0);
                            break;
                    }
                }
                else
                {
                    Player.Position += move;
                }

                Player.OnGround = true;

                loops++;

            } while (collision && loops < 3);
        }

        public void DeleteBlock(int x, int y, int z)
        {
            Chunk.Blocks[x, y, z] = null;
        }

        public void PutBlock(int x, int y, int z)
        {
            Chunk.Blocks[x, y, z] = new GrassBlock();
        }
    }
}
