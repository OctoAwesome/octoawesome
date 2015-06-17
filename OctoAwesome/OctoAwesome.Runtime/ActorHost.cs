using Microsoft.Xna.Framework;
using OctoAwesome.Basics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ActorHost : IPlayerController
    {
        private readonly float Gap = 0.00001f;

        private IPlanet planet;
        private Cache<Index3, IChunk> localChunkCache;

        private bool lastJump = false;

        private Index3? lastInteract = null;
        private Index3? lastApply = null;
        private OrientationFlags lastOrientation = OrientationFlags.None;

        public Player Player { get; private set; }

        public IBlockDefinition ActiveTool { get; set; }

        public ActorHost(Player player)
        {
            Player = player;
            localChunkCache = new Cache<Index3, IChunk>(10, loadChunk, null);
            planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            ActiveTool = null;
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

            Player.OnGround = false;
            bool collision = false;
            int loop = 0;

            do
            {
                int minx = (int)Math.Floor(Math.Min(
                    Player.Position.BlockPosition.X - Player.Radius,
                    Player.Position.BlockPosition.X - Player.Radius + move.X));
                int maxx = (int)Math.Floor(Math.Max(
                    Player.Position.BlockPosition.X + Player.Radius,
                    Player.Position.BlockPosition.X + Player.Radius + move.X));
                int miny = (int)Math.Floor(Math.Min(
                    Player.Position.BlockPosition.Y - Player.Radius,
                    Player.Position.BlockPosition.Y - Player.Radius + move.Y));
                int maxy = (int)Math.Floor(Math.Max(
                    Player.Position.BlockPosition.Y + Player.Radius,
                    Player.Position.BlockPosition.Y + Player.Radius + move.Y));
                int minz = (int)Math.Floor(Math.Min(
                    Player.Position.BlockPosition.Z,
                    Player.Position.BlockPosition.Z + move.Z));
                int maxz = (int)Math.Floor(Math.Max(
                    Player.Position.BlockPosition.Z + Player.Height,
                    Player.Position.BlockPosition.Z + Player.Height + move.Z));

                // Relative PlayerBox
                BoundingBox playerBox = new BoundingBox(
                    new Vector3(
                        Player.Position.BlockPosition.X - Player.Radius,
                        Player.Position.BlockPosition.Y - Player.Radius,
                        Player.Position.BlockPosition.Z),
                    new Vector3(
                        Player.Position.BlockPosition.X + Player.Radius,
                        Player.Position.BlockPosition.Y + Player.Radius,
                        Player.Position.BlockPosition.Z + Player.Height));

                collision = false;
                float min = 1f;
                Axis minAxis = Axis.None;

                for (int z = minz; z <= maxz; z++)
                {
                    for (int y = miny; y <= maxy; y++)
                    {
                        for (int x = minx; x <= maxx; x++)
                        {
                            Index3 pos = new Index3(x, y, z);
                            IBlock block = GetBlock(pos + 
                                Player.Position.GlobalBlockIndex);
                            if (block == null)
                                continue;

                            Axis? localAxis;
                            float? moveFactor = block.Intersect(pos, playerBox, move, out localAxis);

                            if (moveFactor.HasValue && moveFactor.Value < min)
                            {
                                collision = true;
                                min = moveFactor.Value;
                                minAxis = localAxis.Value;
                            }
                        }
                    }
                }

                Player.Position += (move * min);
                move *= (1f - min);
                switch (minAxis)
                {
                    case Axis.X:
                        Player.Velocity *= new Vector3(0, 1, 1);
                        Player.Position += new Vector3(move.X > 0 ? -Gap : Gap, 0, 0);
                        move.X = 0f;
                        break;
                    case Axis.Y:
                        Player.Velocity *= new Vector3(1, 0, 1);
                        Player.Position += new Vector3(0, move.Y > 0 ? -Gap : Gap, 0);
                        move.Y = 0f;
                        break;
                    case Axis.Z:
                        Player.OnGround = true;
                        Player.Velocity *= new Vector3(1, 1, 0);
                        Player.Position += new Vector3(0, 0, move.Z > 0 ? -Gap : Gap);
                        move.Z = 0f;
                        break;
                }

                // Koordinate normalisieren (Rundwelt)
                Coordinate position = Player.Position;
                position.NormalizeChunkIndexXY(planet.Size);
                Player.Position = position;

                loop++;
            }
            while (collision && loop < 3);

            #endregion

            #region Block Interaktion

            if (lastInteract.HasValue)
            {
                ResourceManager.Instance.SetBlock(planet.Id, lastInteract.Value, null);
                lastInteract = null;
            }

            if (lastApply.HasValue)
            {
                if (ActiveTool != null)
                {
                    Index3 add = new Index3();
                    switch (lastOrientation)
                    {
                        case OrientationFlags.SideWest: add = new Index3(-1, 0, 0); break;
                        case OrientationFlags.SideEast: add = new Index3(1, 0, 0); break;
                        case OrientationFlags.SideSouth: add = new Index3(0, -1, 0); break;
                        case OrientationFlags.SideNorth: add = new Index3(0, 1, 0); break;
                        case OrientationFlags.SideBottom: add = new Index3(0, 0, -1); break;
                        case OrientationFlags.SideTop: add = new Index3(0, 0, 1); break;
                    }

                    ResourceManager.Instance.SetBlock(planet.Id, 
                        lastApply.Value + add, ActiveTool.GetInstance(lastOrientation));
                    lastApply = null;
                }
            }

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

        public void Interact(Index3 blockIndex)
        {
            lastInteract = blockIndex;
        }

        public void Apply(Index3 blockIndex, OrientationFlags orientation)
        {
            lastApply = blockIndex;
            lastOrientation = orientation;
        }
    }
}
