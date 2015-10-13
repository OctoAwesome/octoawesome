using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public class ActorHost : IPlayerController
    {
        private readonly IChunkLoader _chunkLoader;
        private readonly float Gap = 0.001f;

        private IPlanet planet;

        private bool lastJump = false;

        private Index3? lastInteract = null;
        private Index3? lastApply = null;
        private OrientationFlags lastOrientation = OrientationFlags.None;
        private Index3 _oldIndex;

        private IPlanetResourceManager _manager;

        public Player Player { get; private set; }

        public InventorySlot ActiveTool { get; set; }

        public WorldState State { get; private set; }

        public ActorHost(Player player, IChunkLoader chunkLoader)
        {
            _chunkLoader = chunkLoader;

            Player = player;
            planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            _oldIndex = Player.Position.ChunkIndex;

            ActiveTool = null;
            State = WorldState.Loading;
        }

        public void Initialize()
        {
            _manager = ResourceManager.Instance.GetManagerForPlanet(planet.Id);

            State = WorldState.Running;

            _chunkLoader.UpdatePosition(0, 0, 0);
        }

        public void Update(GameTime frameTime)
        {
            if (!Player.FlyMode)
            {
                Player.ExternalForce = new Vector3(0, 0, -20f) * Player.Mass;
            }
            else
            {
                Player.ExternalForce = Vector3.Zero;
            }

            #region Inputverarbeitung

            Vector3 externalPower = ((Player.ExternalForce * Player.ExternalForce) / (2 * Player.Mass)) * (float)frameTime.ElapsedGameTime.TotalSeconds;
            externalPower *= new Vector3(Math.Sign(Player.ExternalForce.X), Math.Sign(Player.ExternalForce.Y), Math.Sign(Player.ExternalForce.Z));

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

            if (Player.FlyMode)
            {
                VelocityDirection += new Vector3(0, 0, (float)Math.Sin(Player.Tilt) * Move.Y);
                Friction = Vector3.One * Player.FRICTION;
            }

            powerdirection += externalPower;
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
                            Index3 blockPos = pos + Player.Position.GlobalBlockIndex;
                            ushort block = _manager.GetBlock(blockPos);
                            if (block == 0)
                                continue;

                            Axis? localAxis;
                            IBlockDefinition blockDefinition = DefinitionManager.GetBlockDefinitionByIndex(block);
                            float? moveFactor = Block.Intersect(
                                blockDefinition.GetCollisionBoxes(_manager, blockPos.X, blockPos.Y, blockPos.Z),
                                pos, playerBox, move, out localAxis);

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

            if (Player.Position.ChunkIndex != _oldIndex)
            {
                //TODO: Planeten rundung beachten :)

                _chunkLoader.UpdatePosition(Player.Position.ChunkIndex.X - _oldIndex.X, Player.Position.ChunkIndex.Y - _oldIndex.Y, Player.Position.ChunkIndex.Z - _oldIndex.Z);
                _oldIndex = Player.Position.ChunkIndex;
            }

            #endregion

            #region Block Interaction

            if (lastInteract.HasValue)
            {
                ushort lastBlock = _manager.GetBlock(lastInteract.Value);
                _manager.SetBlock(lastInteract.Value, 0);

                if (lastBlock != 0)
                {
                    var blockDefinition = DefinitionManager.GetBlockDefinitionByIndex(lastBlock);

                    var slot = Player.Inventory.Where(s => s.Definition == blockDefinition && s.Amount < blockDefinition.StackLimit).FirstOrDefault();

                    // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
                    if (slot == null || slot.Amount >= blockDefinition.StackLimit)
                    {
                        slot = new InventorySlot()
                        {
                            Definition = blockDefinition,
                            Amount = 0
                        };
                        Player.Inventory.Add(slot);
                    }
                    slot.Amount++;
                }
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

                    if (ActiveTool.Definition is IBlockDefinition)
                    {
                        IBlockDefinition definition = ActiveTool.Definition as IBlockDefinition;
                        _manager.SetBlock(lastApply.Value + add, DefinitionManager.GetBlockDefinitionIndex(definition));

                        ActiveTool.Amount--;
                        if (ActiveTool.Amount <= 0)
                        {
                            Player.Inventory.Remove(ActiveTool);
                            ActiveTool = null;
                        }
                    }

                    // TODO: Fix Interaction ;)
                    //ushort block = _manager.GetBlock(lastApply.Value);
                    //IBlockDefinition blockDefinition = BlockDefinitionManager.GetForType(block);
                    //IItemDefinition itemDefinition = ActiveTool.Definition;

                    //blockDefinition.Hit(blockDefinition, itemDefinition.GetProperties(null));
                    //itemDefinition.Hit(null, blockDefinition.GetProperties(block));
                }

                lastApply = null;
            }

            #endregion
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
