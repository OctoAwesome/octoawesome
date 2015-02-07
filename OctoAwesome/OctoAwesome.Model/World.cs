using Microsoft.Xna.Framework;
using OctoAwesome.Components;
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
            int cellZ = (int)Player.Position.Z;

            // Modifikation der Geschwindigkeit
            Vector3 velocity = Player.Velocity;
            // velocity *= cell.VelocityFactor;

            Vector3 newPosition = Player.Position + (velocity * (float)frameTime.ElapsedGameTime.TotalSeconds);

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
            if (velocity.Y < 0)
            {
                if (newPosition.Y < 50)
                {
                    newPosition.Y = 50;
                    Player.OnGround = true;
                }
                else
                {
                    
                }
            }

            Player.Position = newPosition;
        }
    }
}
