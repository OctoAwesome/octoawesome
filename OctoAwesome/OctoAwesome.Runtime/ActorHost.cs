using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ActorHost : IPlayerController
    {
        public static int SELECTIONRANGE = 8;

        private readonly float Gap = 0.001f;

        private IPlanet planet;
        private Cache<Index3, IChunk> localChunkCache;

        private bool lastJump = false;

        // private IInputSet input;

        public Player Player { get; private set; }

        public Vector3? SelectedBox { get; private set; }

        public ActorHost(Player player)
        {
            Player = player;
            SelectedBox = null;
            localChunkCache = new Cache<Index3, IChunk>(10, loadChunk, null);
            planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);
        }

        public void Update(GameTime frameTime)
        {
            Player.ExternalForce = new Vector3(0, 0, -20f) * Player.Mass;

            #region Inputverarbeitung

            Vector3 externalPower = ((Player.ExternalForce * Player.ExternalForce) / (2 * Player.Mass)) * (float)frameTime.ElapsedGameTime.TotalSeconds;

            // Input verarbeiten
            Player.Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * Head.X;
            Player.Tilt += (float)frameTime.ElapsedGameTime.TotalSeconds * Head.Y;
            Player.Tilt = Math.Min(1.5f, Math.Max(-1.5f, Player.Tilt));

            float lookX = (float)Math.Cos(Player.Angle);
            float lookY = -(float)Math.Sin(Player.Angle);
            var VelocityDirection = new Vector3(lookX, lookY, 0) * Move.Y;

            float stafeX = (float)Math.Cos(Player.Angle + MathHelper.PiOver2);
            float stafeY = -(float)Math.Sin(Player.Angle + MathHelper.PiOver2);
            VelocityDirection += new Vector3(stafeX, stafeY, 0) * Move.X;

            Vector3 Friction = new Vector3(1, 1, 0.1f) * Player.FRICTION;
            Vector3 powerdirection = new Vector3();

            powerdirection += Player.ExternalForce;
            powerdirection += (Player.POWER * VelocityDirection);
            // if (OnGround && input.JumpTrigger)
            if (lastJump)
            {
                lastJump = false;
                Vector3 jumpDirection = new Vector3(lookX, lookY, 0f) * Move.Y * 0.1f;
                jumpDirection.Z = 1f;
                jumpDirection.Normalize();
                powerdirection += jumpDirection * Player.JUMPPOWER;
            }

            Vector3 VelocityChange = (2.0f / Player.Mass * (powerdirection - Friction * Player.Velocity)) *
                (float)frameTime.ElapsedGameTime.TotalSeconds;

            Player.Velocity += new Vector3(
                (float)(VelocityChange.X < 0 ? -Math.Sqrt(-VelocityChange.X) : Math.Sqrt(VelocityChange.X)),
                (float)(VelocityChange.Y < 0 ? -Math.Sqrt(-VelocityChange.Y) : Math.Sqrt(VelocityChange.Y)),
                (float)(VelocityChange.Z < 0 ? -Math.Sqrt(-VelocityChange.Z) : Math.Sqrt(VelocityChange.Z)));

            #endregion

            #region Playerbewegung

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

                            IBlock block = GetBlock(pos);
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

            #endregion

            #region Selektion

            Index3 localcell = Player.Position.LocalBlockIndex;
            Index3 currentChunk = Player.Position.ChunkIndex;

            Vector3 direction = new Vector3(
                (float)Math.Cos(Player.Angle),
                -(float)Math.Sin(Player.Angle),
                (float)Math.Sin(Player.Tilt));
            direction.Normalize();

            Ray pickRay = new Ray(new Vector3(
                        Player.Position.LocalPosition.X,
                        Player.Position.LocalPosition.Y,
                        Player.Position.LocalPosition.Z + 3.2f), direction);

            Vector3? selected = null;
            float? bestDistance = null;
            for (int z = localcell.Z - SELECTIONRANGE; z < localcell.Z + SELECTIONRANGE; z++)
            {
                for (int y = localcell.Y - SELECTIONRANGE; y < localcell.Y + SELECTIONRANGE; y++)
                {
                    for (int x = localcell.X - SELECTIONRANGE; x < localcell.X + SELECTIONRANGE; x++)
                    {
                        Index3 pos = new Index3(
                            x + (currentChunk.X * Chunk.CHUNKSIZE_X),
                            y + (currentChunk.Y * Chunk.CHUNKSIZE_Y),
                            z + (currentChunk.Z * Chunk.CHUNKSIZE_Z));

                        IBlock block = GetBlock(pos);
                        if (block == null)
                            continue;

                        BoundingBox[] boxes = block.GetCollisionBoxes();

                        foreach (var box in boxes)
                        {
                            BoundingBox transformedBox = new BoundingBox(
                                box.Min + new Vector3(x, y, z),
                                box.Max + new Vector3(x, y, z));

                            float? distance = pickRay.Intersects(transformedBox);
                            if (distance.HasValue)
                            {
                                if (!bestDistance.HasValue || bestDistance.Value > distance)
                                {
                                    bestDistance = distance.Value;
                                    selected = new Vector3(pos.X, pos.Y, pos.Z);
                                }
                            }
                        }
                    }
                }
            }

            SelectedBox = selected;

            #endregion

            #region Block Interaktion

            //if (input.ApplyTrigger && SelectedBox.HasValue)
            //{
            //    Index3 pos = new Index3(
            //        (int)SelectedBox.Value.X,
            //        (int)SelectedBox.Value.Y,
            //        (int)SelectedBox.Value.Z);

            //    ResourceManager.Instance.SetBlock(planet.Id, pos, null);
            //}

            #endregion
        }
        private IChunk loadChunk(Index3 index)
        {
            IPlanet planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);
            return ResourceManager.Instance.GetChunk(planet.Id, index);
        }

        /// <summary>
        /// Liefert den Block an der angegebenen Block-Koodinate zurück.
        /// </summary>
        /// <param name="index">Block Index</param>
        /// <returns>Block oder null, falls dort kein Block existiert</returns>
        public IBlock GetBlock(Index3 index)
        {
            IPlanet planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            index.NormalizeXY(new Index2(
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);

            // Betroffener Chunk ermitteln
            Index3 chunkIndex = coordinate.ChunkIndex;
            if (chunkIndex.X < 0 || chunkIndex.X >= planet.Size.X ||
                chunkIndex.Y < 0 || chunkIndex.Y >= planet.Size.Y ||
                chunkIndex.Z < 0 || chunkIndex.Z >= planet.Size.Z)
                return null;
            IChunk chunk = localChunkCache.Get(chunkIndex);
            if (chunk == null)
                return null;

            return chunk.GetBlock(coordinate.LocalBlockIndex);
        }

        /// <summary>
        /// Überschreibt den Block an der angegebenen Koordinate.
        /// </summary>
        /// <param name="index">Block-Koordinate</param>
        /// <param name="block">Neuer Block oder null, falls der alte Bock gelöscht werden soll.</param>
        public void SetBlock(Index3 index, IBlock block)
        {
            IPlanet planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            index.NormalizeXYZ(new Index3(
                planet.Size.X * Chunk.CHUNKSIZE_X,
                planet.Size.Y * Chunk.CHUNKSIZE_Y,
                planet.Size.Z * Chunk.CHUNKSIZE_Z));
            Coordinate coordinate = new Coordinate(0, index, Vector3.Zero);
            IChunk chunk = localChunkCache.Get(coordinate.ChunkIndex);
            chunk.SetBlock(coordinate.LocalBlockIndex, block);
        }

        public Coordinate Position
        {
            get { return Player.Position; }
        }

        public float Radius
        {
            get { return Player.Radius; }
        }

        public float Angle
        {
            get { return Player.Angle; }
        }

        public float Height
        {
            get { return Player.Height; }
        }

        public bool OnGround
        {
            get { return Player.OnGround; }
        }

        public float Tilt
        {
            get { return Player.Tilt; }
        }

        public Vector2 Move { get; set; }

        public Vector2 Head { get; set; }

        public void Jump()
        {
            lastJump = true;
        }

        public void Interact()
        {
            throw new NotImplementedException();
        }

        public void Apply()
        {
            throw new NotImplementedException();
        }
    }
}
