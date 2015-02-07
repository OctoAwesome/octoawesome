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
        public Chunk Chunk { get; private set; }

        public Player Player { get; private set; }

        public World(IInputSet input)
        {
            Chunk = new Model.Chunk();
            Player = new Player(input);
        }

        public void Update(GameTime frameTime)
        {
            Player.Update(frameTime);

            // Ermittlung der Oberflächenbeschaffenheit
            int cellX = (int)Player.Position.X;
            int cellY = (int)Player.Position.Y;
            int cellZ = (int)Player.Position.Z;

            // Modifikation der Geschwindigkeit
            Player.Velocity += Player.Mass * new Vector3(0, -5f, 0) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            // velocity *= cell.VelocityFactor;

            Vector3 newPosition = Player.Position + (Player.Velocity * (float)frameTime.ElapsedGameTime.TotalSeconds);

            BoundingBox playerBox = new BoundingBox(
                new Vector3(Player.Position.X - Player.Radius, Player.Position.Y + 4f, Player.Position.Z - Player.Radius),
                new Vector3(Player.Position.X + Player.Radius, Player.Position.Y, Player.Position.Z + Player.Radius));

            int range = 1;
            for (int z = cellZ - range; z < cellZ + range; z++)
            {
                for (int y = cellY - range; y < cellY + range; y++)
                {
                    for (int x = cellX - range; x < cellX + range; x++)
                    {
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
                            if (playerBox.Intersects(box))
                            {
                            }
                        }
                    }
                }
            }

            // Block nach links (Kartenrand + nicht begehbare Zellen)
            //if (velocity.X < 0)
            //{
            //    float posLeft = newPosition.X - Player.Radius;
            //    cellX = (int)posLeft;
            //    cellZ = (int)Player.Position.Z;

            //    if (posLeft < 0)
            //    {
            //        newPosition = new Vector3(cellX + Player.Radius, newPosition.Y, newPosition.Z);
            //    }

            //    if (cellX < 0)
            //    {
            //        newPosition = new Vector3((cellX + 1) + Player.Radius, newPosition.Y, newPosition.Z);
            //    }
            //}

            //// Block nach oben (Kartenrand + nicht begehbare Zellen)
            //if (velocity.Z < 0)
            //{
            //    float posTop = newPosition.Z - Player.Radius;
            //    cellX = (int)Player.Position.X;
            //    cellZ = (int)posTop;

            //    if (posTop < 0)
            //    {
            //        newPosition = new Vector3(newPosition.X, newPosition.Y, cellZ + Player.Radius);
            //    }

            //    if (cellZ < 0)
            //    {
            //        newPosition = new Vector3(newPosition.X, newPosition.Y, cellZ + 1 + Player.Radius);
            //    }
            //}

            //if (velocity.X > 0)
            //{
            //    float posRight = newPosition.X + Player.Radius;
            //    cellX = (int)posRight;
            //    cellZ = (int)Player.Position.Z;

            //    if (cellX >= Chunk.CHUNKSIZE_X)
            //    {
            //        newPosition = new Vector3(cellX - Player.Radius, newPosition.Y, newPosition.Z);
            //    }
            //}

            //if (velocity.Z > 0)
            //{
            //    float posBottom = newPosition.Z + Player.Radius;
            //    cellX = (int)Player.Position.X;
            //    cellZ = (int)posBottom;

            //    if (cellZ >= Chunk.CHUNKSIZE_Z)
            //    {
            //        newPosition = new Vector3(newPosition.X, newPosition.Y, cellZ - Player.Radius);
            //    }
            //}

            Player.OnGround = false;
            if (Player.Velocity.Y < 0)
            {
                if (newPosition.Y < 50)
                {
                    newPosition.Y = 50;
                    Player.Velocity = new Vector3(Player.Velocity.X, 0, Player.Velocity.Z);
                    Player.OnGround = true;
                }
            }

            Player.Position = newPosition;
        }
    }
}
