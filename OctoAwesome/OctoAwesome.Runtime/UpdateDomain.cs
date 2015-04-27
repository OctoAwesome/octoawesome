using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    internal class UpdateDomain
    {
        private readonly float Gap = 0.001f;

        // private IPlanet[] planets;
        private IUniverse universe;

        public Player Player { get; private set; }

        public UpdateDomain(IInputSet input, int planetCount)
        {
            Player = new Player(input);
        }

        public void Update(GameTime frameTime)
        {
            Player.ExternalForce = new Vector3(0, 0, -20f) * Player.Mass;

            Player.Update(frameTime);

            Vector3 move = Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds;
            IPlanet planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            Index2 planetSize = new Index2(
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y);

            Player.OnGround = false;

            int minx = (int)Math.Min(
                Player.Position.GlobalPosition.X - Player.Radius,
                Player.Position.GlobalPosition.X - Player.Radius + move.X);
            int maxx = (int)Math.Max(
                Player.Position.GlobalPosition.X + Player.Radius,
                Player.Position.GlobalPosition.X + Player.Radius + move.X);
            int miny = (int)Math.Min(
                Player.Position.GlobalPosition.Y - Player.Radius,
                Player.Position.GlobalPosition.Y - Player.Radius + move.Y);
            int maxy = (int)Math.Max(
                Player.Position.GlobalPosition.Y + Player.Radius,
                Player.Position.GlobalPosition.Y + Player.Radius + move.Y);
            int minz = (int)Math.Min(
                Player.Position.GlobalPosition.Z,
                Player.Position.GlobalPosition.Z + move.Z);
            int maxz = (int)Math.Max(
                Player.Position.GlobalPosition.Z + Player.Height,
                Player.Position.GlobalPosition.Z + Player.Height + move.Z);

            bool collision = false;
            int loops = 0;

            do
            {
                BoundingBox playerBox = new BoundingBox(
                    new Vector3(
                        Player.Position.GlobalPosition.X + move.X - Player.Radius,
                        Player.Position.GlobalPosition.Y + move.Y - Player.Radius,
                        Player.Position.GlobalPosition.Z + move.Z),
                    new Vector3(
                        Player.Position.GlobalPosition.X + move.X + Player.Radius,
                        Player.Position.GlobalPosition.Y + move.Y + Player.Radius,
                        Player.Position.GlobalPosition.Z + move.Z + Player.Height));

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
                            Index3 pos = new Index3(x, y, z);
                            pos.NormalizeXY(planetSize);

                            IBlock block = ResourceManager.Instance.GetBlock(pos);
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
                                        if (diff < -move.Y)
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
                            break;
                        case Axis.Z:
                            move.Z = 0;
                            Player.Position = Player.Position + new Vector3(0, 0, minGap);
                            Player.Velocity *= new Vector3(1, 1, 0);
                            if (minGap > 0) Player.OnGround = true;
                            break;
                    }
                }
                else
                {
                    Player.Position += move;
                }

                Coordinate playerPosition = Player.Position;
                Index3 blockIndex = playerPosition.GlobalBlockIndex;
                blockIndex.NormalizeXY(planetSize);
                Player.Position = new Coordinate(playerPosition.Planet, blockIndex, playerPosition.BlockPosition);

                loops++;

            } while (collision && loops < 3);
        }

        public void Save()
        {
            ResourceManager.Instance.Save();
        }
    }
}
