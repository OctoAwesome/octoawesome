using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Kapselung eines Spielers.
    /// </summary>
    public class ActorHost : IPlayerController
    {
        private readonly float Gap = 0.001f;

        private IPlanet planet;

        private bool lastJump = false;

        private Index3? lastInteract = null;
        private Index3? lastApply = null;
        private InventorySlot lastTool = null;
        private OrientationFlags lastOrientation = OrientationFlags.None;
        private Index3 _oldIndex;

        private ILocalChunkCache localChunkCache;

        /// <summary>
        /// Der Spieler dieses ActorHosts.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Gibt an, ob der Spieler bereit ist.
        /// </summary>
        public bool ReadyState { get; private set; }

        /// <summary>
        /// Erzeugt einen neuen ActorHost.
        /// </summary>
        /// <param name="player">Der Player</param>
        public ActorHost(Player player)
        {
            Player = player;
            planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            localChunkCache = new LocalChunkCache(ResourceManager.Instance.GlobalChunkCache, 2, 1, true);
            _oldIndex = Player.Position.ChunkIndex;

            ReadyState = false;
        }

        /// <summary>
        /// Initialisiert den ActorHost und lädt die Chunks rund um den Spieler.
        /// </summary>
        public void Initialize()
        {
            localChunkCache.SetCenter(planet, Player.Position.ChunkIndex, (success) =>
            {
                IChunk chunk = localChunkCache.GetChunk(Player.Position.ChunkIndex);
                if (chunk != null)
                {
                    chunk.Entities.Add(Player);
                    Server.Instance.InsertEntity(Player, chunk);
                }
                ReadyState = success;
            });
        }

        /// <summary>
        /// Deinitialisiert den ActorHost und gibt die verwendeten Chunks frei.
        /// </summary>
        public void Uninitialize()
        {
            IChunk chunk = localChunkCache.GetChunk(Player.Position.ChunkIndex);
            if (chunk != null)
            {
                chunk.Entities.Remove(Player);
                Server.Instance.RemoveEntity(Player, chunk);
            }
        }

        /// <summary>
        /// Aktualisiert den Spieler (Bewegung, Interaktion)
        /// </summary>
        /// <param name="frameTime">Die aktuelle Zeit.</param>
        public void Update(GameTime frameTime)
        {
            #region Inputverarbeitung

            // Input verarbeiten
            Player.Angle += (float)frameTime.ElapsedGameTime.TotalSeconds * Head.X;
            Player.Tilt += (float)frameTime.ElapsedGameTime.TotalSeconds * Head.Y;
            Player.Tilt = Math.Min(1.5f, Math.Max(-1.5f, Player.Tilt));

            #endregion

            #region Physik

            float lookX = (float)Math.Cos(Player.Angle);
            float lookY = -(float)Math.Sin(Player.Angle);
            var velocitydirection = new Vector3(lookX, lookY, 0) * Move.Y;

            float stafeX = (float)Math.Cos(Player.Angle + MathHelper.PiOver2);
            float stafeY = -(float)Math.Sin(Player.Angle + MathHelper.PiOver2);
            velocitydirection += new Vector3(stafeX, stafeY, 0) * Move.X;

            Player.Velocity += PhysicalUpdate(velocitydirection, frameTime.ElapsedGameTime, !Player.FlyMode,
                Player.FlyMode, Player.Sprint);

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
                            ushort block = localChunkCache.GetBlock(blockPos);
                            if (block == 0)
                                continue;

                            Axis? localAxis;
                            IBlockDefinition blockDefinition = DefinitionManager.GetBlockDefinitionByIndex(block);
                            float? moveFactor = Block.Intersect(
                                blockDefinition.GetCollisionBoxes(localChunkCache, blockPos.X, blockPos.Y, blockPos.Z),
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

                //Beam me up
                KeyboardState ks = Keyboard.GetState();
                if (ks.IsKeyDown(Keys.P))
                {
                    position = position + new Vector3(0, 0, 10);
                }

                Player.Position = position;

                loop++;
            } while (collision && loop < 3);


            if (Player.Position.ChunkIndex != _oldIndex)
            {
                // Aus altem Chunk entfernen
                IChunk oldChunk = localChunkCache.GetChunk(_oldIndex);
                if (oldChunk != null)
                    oldChunk.Entities.Remove(Player);

                _oldIndex = Player.Position.ChunkIndex;
                ReadyState = false;
                localChunkCache.SetCenter(planet, Player.Position.ChunkIndex, (success) =>
                {
                    // Zu neuem Chunk hinzufügen
                    IChunk newChunk = localChunkCache.GetChunk(Player.Position.ChunkIndex);
                    if (newChunk != null)
                        newChunk.Entities.Add(Player);

                    // Move-Event
                    Server.Instance.MoveEntity(Player, oldChunk, newChunk);

                    ReadyState = success;
                });
            }

            #endregion

            #region Block Interaction

            if (lastInteract.HasValue)
            {
                ushort lastBlock = localChunkCache.GetBlock(lastInteract.Value);
                localChunkCache.SetBlock(lastInteract.Value, 0);
                Server.Instance.RemoveBlock(Player.Position.Planet, lastInteract.Value);

                if (lastBlock != 0)
                {
                    var blockDefinition = DefinitionManager.GetBlockDefinitionByIndex(lastBlock);

                    var slot =
                        Player.Inventory.Where(
                            s => s.Definition == blockDefinition && s.Amount < blockDefinition.StackLimit)
                            .FirstOrDefault();

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
                if (lastTool != null)
                {
                    Index3 add = new Index3();
                    switch (lastOrientation)
                    {
                        case OrientationFlags.SideWest:
                            add = new Index3(-1, 0, 0);
                            break;
                        case OrientationFlags.SideEast:
                            add = new Index3(1, 0, 0);
                            break;
                        case OrientationFlags.SideSouth:
                            add = new Index3(0, -1, 0);
                            break;
                        case OrientationFlags.SideNorth:
                            add = new Index3(0, 1, 0);
                            break;
                        case OrientationFlags.SideBottom:
                            add = new Index3(0, 0, -1);
                            break;
                        case OrientationFlags.SideTop:
                            add = new Index3(0, 0, 1);
                            break;
                    }

                    if (lastTool.Definition is IBlockDefinition)
                    {
                        IBlockDefinition definition = lastTool.Definition as IBlockDefinition;
                        localChunkCache.SetBlock(lastApply.Value + add,
                            DefinitionManager.GetBlockDefinitionIndex(definition));
                        Server.Instance.AddBlock(Player.Position.Planet, lastApply.Value + add, definition, 0);

                        lastTool.Amount--;
                        if (lastTool.Amount <= 0)
                        {
                            Player.Inventory.Remove(lastTool);
                            lastTool = null;
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

            #region Property Events

            // Position
            if (Position.Planet != oldPosition.Planet ||
                Position.GlobalBlockIndex != oldPosition.GlobalBlockIndex ||
                Position.BlockPosition != oldPosition.BlockPosition)
            {
                oldPosition = Position;
                if (OnPositionChanged != null)
                    OnPositionChanged(Position);
            }

            // Radius
            if (Radius != oldRadius)
            {
                oldRadius = Radius;
                if (OnRadiusChanged != null)
                    OnRadiusChanged(Radius);
            }

            // Angle
            if (Angle != oldAngle)
            {
                oldAngle = Angle;
                if (OnAngleChanged != null)
                    OnAngleChanged(Angle);
            }

            // Height
            if (Height != oldHeight)
            {
                oldHeight = Height;
                if (OnHeightChanged != null)
                    OnHeightChanged(Height);
            }

            // On Ground
            if (OnGround != oldOnGround)
            {
                oldOnGround = OnGround;
                if (OnOnGroundChanged != null)
                    OnOnGroundChanged(OnGround);
            }

            // Fly Mode
            if (FlyMode != oldFlyMode)
            {
                oldFlyMode = FlyMode;
                if (OnFlyModeChanged != null)
                    OnFlyModeChanged(FlyMode);
            }

            // Tilt
            if (Tilt != oldTilt)
            {
                oldTilt = Tilt;
                if (OnTiltChanged != null)
                    OnTiltChanged(Tilt);
            }

            // Move
            if (Move != oldMove)
            {
                oldMove = Move;
                if (OnMoveChanged != null)
                    OnMoveChanged(Move);
            }

            // Head
            if (Head != oldHead)
            {
                oldHead = Head;
                if (OnHeadChanged != null)
                    OnHeadChanged(Head);
            }

            #endregion
        }

        private Vector3 PhysicalUpdate(Vector3 velocitydirection, TimeSpan elapsedtime, bool gravity, bool flymode, bool sprint)
        {
            Vector3 exforce = !flymode ? Player.ExternalForce : Vector3.Zero;

            if (gravity && !flymode)
            {
                exforce += new Vector3(0, 0, -20f) * Player.Mass;
            }

            Vector3 externalPower = ((exforce * exforce) / (2 * Player.Mass)) * (float)elapsedtime.TotalSeconds;
            externalPower *= new Vector3(Math.Sign(exforce.X), Math.Sign(exforce.Y), Math.Sign(exforce.Z));

            Vector3 friction = new Vector3(1, 1, 0.1f) * Player.FRICTION;
            Vector3 powerdirection = new Vector3();

            if (flymode)
            {
                velocitydirection += new Vector3(0, 0, (float)Math.Sin(Player.Tilt) * Move.Y);
                friction = Vector3.One * Player.FRICTION;
            }

            powerdirection += externalPower;
            powerdirection += (Player.POWER * (sprint ? 2 : 1) * velocitydirection);
            if (lastJump && (OnGround || flymode))
            {
                Vector3 jumpDirection = new Vector3(0, 0, 1);
                jumpDirection.Z = 1f;
                jumpDirection.Normalize();
                powerdirection += jumpDirection * Player.JUMPPOWER;
            }
            lastJump = false;


            Vector3 VelocityChange = (2.0f / Player.Mass * (powerdirection - friction * Player.Velocity)) *
                                     (float)elapsedtime.TotalSeconds;

            return new Vector3(
                (float)(VelocityChange.X < 0 ? -Math.Sqrt(-VelocityChange.X) : Math.Sqrt(VelocityChange.X)),
                (float)(VelocityChange.Y < 0 ? -Math.Sqrt(-VelocityChange.Y) : Math.Sqrt(VelocityChange.Y)),
                (float)(VelocityChange.Z < 0 ? -Math.Sqrt(-VelocityChange.Z) : Math.Sqrt(VelocityChange.Z)));
        }

        internal void Unload()
        {
            localChunkCache.Flush();
        }

        private Coordinate oldPosition;

        /// <summary>
        /// Position des Spielers.
        /// </summary>
        public Coordinate Position
        {
            get { return Player.Position; }
        }

        private float oldRadius = 0f;

        /// <summary>
        /// Radius des Spielers.
        /// </summary>
        public float Radius
        {
            get { return Player.Radius; }
        }

        private float oldAngle = 0f;

        /// <summary>
        /// Winkel des Spielers (Standposition).
        /// </summary>
        public float Angle
        {
            get { return Player.Angle; }
        }

        private float oldHeight = 0f;

        /// <summary>
        /// Höhe des Spielers.
        /// </summary>
        public float Height
        {
            get { return Player.Height; }
        }

        private bool oldOnGround = false;

        /// <summary>
        /// Gibt an, ob der Spieler auf dem Boden steht.
        /// </summary>
        public bool OnGround
        {
            get { return Player.OnGround; }
        }

        private bool oldFlyMode = false;

        /// <summary>
        /// Gibt an, ob der Flugmodus aktiviert ist.
        /// </summary>
        public bool FlyMode
        {
            get { return Player.FlyMode; }
            set { Player.FlyMode = value; }
        }

        /// <summary>
        /// Gibt an, ob der Spieler sprintet.
        /// </summary>
        public bool Sprint
        {
            get { return Player.Sprint; }
            set { Player.Sprint = value; }
        }

        /// <summary>
        /// Gibt an, ob der Spieler ?.
        /// </summary>
        public bool Crouch
        {
            get { return Player.Crouch; }
            set { Player.Crouch = value; }
        }

        private float oldTilt = 0f;

        /// <summary>
        /// Winkel der Kopfstellung.
        /// </summary>
        public float Tilt
        {
            get { return Player.Tilt; }
        }

        /// <summary>
        /// Das Inventar des Spielers.
        /// </summary>
        public IEnumerable<InventorySlot> Inventory
        {
            get { return Player.Inventory; }
        }

        private Vector2 oldMove = Vector2.Zero;

        /// <summary>
        /// Bewegungsvektor des Spielers.
        /// </summary>
        public Vector2 Move { get; set; }

        private Vector2 oldHead = Vector2.Zero;

        /// <summary>
        /// Kopfbewegeungsvektor des Spielers.
        /// </summary>
        public Vector2 Head { get; set; }

        /// <summary>
        /// Den Spieler hüpfen lassen.
        /// </summary>
        public void Jump()
        {
            lastJump = true;
        }

        /// <summary>
        /// Lässt den Spieler mit einem Block Interagieren. (zur Zeit Block löschen)
        /// </summary>
        /// <param name="blockIndex">Die Position des Blocks.</param>
        public void Interact(Index3 blockIndex)
        {
            lastInteract = blockIndex;
        }

        /// <summary>
        /// Lässt den Spieler ein Werkzeug auf einen Block anwenden. (zur Zeit Block setzen)
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="tool">Das anzuwendende Wekzeug.</param>
        /// <param name="orientation"></param>
        public void Apply(Index3 blockIndex, InventorySlot tool, OrientationFlags orientation)
        {
            lastApply = blockIndex;
            lastOrientation = orientation;
            lastTool = tool;
        }

        /// <summary>
        /// Debug-Methode. Nicht zur Benutzung im Spiel.
        /// </summary>
        public void AllBlocksDebug()
        {
            var blockDefinitions = DefinitionManager.GetBlockDefinitions();

            foreach (var blockDefinition in blockDefinitions)
            {
                var slot =
                    Player.Inventory.Where(s => s.Definition == blockDefinition && s.Amount < blockDefinition.StackLimit)
                        .FirstOrDefault();

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
        }

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Position"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<Coordinate> OnPositionChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Radius"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<float> OnRadiusChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Angle"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<float> OnAngleChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Height"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<float> OnHeightChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="OnGround"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<bool> OnOnGroundChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="FlyMode"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<bool> OnFlyModeChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Tilt"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<float> OnTiltChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Move"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<Vector2> OnMoveChanged;

        /// <summary>
        /// Event, das eintritt, wenn <see cref="Head"/> geändert wird.
        /// </summary>
        public event PropertyChangedDelegate<Vector2> OnHeadChanged;

        /// <summary>
        /// Delegat zur Änderung eines Attributs.
        /// </summary>
        /// <typeparam name="T">Typ des Attributs.</typeparam>
        /// <param name="value">Neuer Wert des Attributs.</param>
        public delegate void PropertyChangedDelegate<T>(T value);
    }
}