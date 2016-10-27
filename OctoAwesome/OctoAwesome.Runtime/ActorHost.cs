using System;
using System.Diagnostics;
using System.Linq;
using engenious;
using engenious.Input;
using OctoAwesome.Ecs;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Kapselung eines Spielers.
    /// </summary>
    public class ActorHost : IPlayerController
    {
        private Index3? lastInteract = null;
        private Index3? lastApply = null;
        private OrientationFlags lastOrientation = OrientationFlags.None;
        private Index3 _oldIndex;
        

        /// <summary>
        /// Der Spieler dieses ActorHosts.
        /// </summary>
        public PlayerComponent Player { get; private set; }

        public Entity PlayerEntity;
        public PositionComponent PlayerPosition;
        public MoveableComponent PlayerMoveable;
        public LookComponent PlayerLook;
        public JumpComponent PlayerJump;

        /// <summary>
        /// Das zur Zeit aktive Werkzeug.
        /// </summary>
        public InventorySlot ActiveTool { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler bereit ist.
        /// </summary>
        public bool ReadyState { get; private set; }

        /// <summary>
        /// Erzeugt einen neuen ActorHost.
        /// </summary>
        /// <param name="player">Der Player</param>
        public ActorHost(Entity player)
        {

            PlayerEntity = player;
            PlayerPosition = player.Get<PositionComponent>();
            PlayerMoveable = player.Get<MoveableComponent>();
            PlayerLook = player.Get<LookComponent>();
            Player = player.Get<PlayerComponent>();
            PlayerJump = player.Get<JumpComponent>();
            PlayerPosition.Planet = ResourceManager.Instance.GetPlanet(PlayerPosition.Coordinate.Planet);
            //Player = player;
            //planet = ResourceManager.Instance.GetPlanet(Player.Position.Planet);

            PlayerMoveable.LocalChunkCache = new LocalChunkCache(ResourceManager.Instance.GlobalChunkCache, 2, 1);
            _oldIndex = PlayerPosition.Coordinate.ChunkIndex;

            ActiveTool = null;
            ReadyState = false;

            Initialize();
        }

        /// <summary>
        /// Initialisiert den ActorHost und lädtc die Chunks rund um den Spieler.
        /// </summary>
        public void Initialize()
        {
            PlayerPosition.LocalChunkCache.SetCenter(PlayerPosition.Planet, new Index2(PlayerPosition.Coordinate.ChunkIndex), (success) =>
            {
                ReadyState = success;
            });
        }

        /// <summary>
        /// Aktualisiert den Spieler (Bewegung, Interaktion)
        /// </summary>
        /// <param name="frameTime">Die aktuelle Zeit.</param>
        public void Update(GameTime frameTime)
        {
            //Beam me up
            KeyboardState ks = Keyboard.GetState();
            if (ks.IsKeyDown(Keys.P))
            {
                PlayerPosition.Coordinate += new Vector3(0, 0, 10);
            }

            if (PlayerPosition.Coordinate.ChunkIndex != _oldIndex)
            {
                _oldIndex = PlayerPosition.Coordinate.ChunkIndex;
                ReadyState = false;
                PlayerPosition.LocalChunkCache.SetCenter(PlayerPosition.Planet, new Index2(PlayerPosition.Coordinate.ChunkIndex), (success) =>
                {
                    ReadyState = success;
                });
            }



            // #endregion

            #region Block Interaction

            //if (lastInteract.HasValue)
            //{
            //    ushort lastBlock = localChunkCache.GetBlock(lastInteract.Value);
            //    localChunkCache.SetBlock(lastInteract.Value, 0);

            //    if (lastBlock != 0)
            //    {
            //        var blockDefinition = DefinitionManager.Instance.GetBlockDefinitionByIndex(lastBlock);

            //        var slot = Player.Inventory.FirstOrDefault(s => s.Definition == blockDefinition);

            //        Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
            //        if (slot == null)
            //        {
            //            slot = new InventorySlot()
            //            {
            //                Definition = blockDefinition,
            //                Amount = 0
            //            };
            //            Player.Inventory.Add(slot);

            //            for (int i = 0; i < Player.Tools.Length; i++)
            //            {
            //                if (Player.Tools[i] == null)
            //                {
            //                    Player.Tools[i] = slot;
            //                    break;
            //                }
            //            }
            //        }
            //        slot.Amount += 125;
            //    }
            //    lastInteract = null;
            //}

            //if (lastApply.HasValue)
            //{
            //    if (ActiveTool != null)
            //    {
            //        Index3 add = new Index3();
            //        switch (lastOrientation)
            //        {
            //            case OrientationFlags.SideWest: add = new Index3(-1, 0, 0); break;
            //            case OrientationFlags.SideEast: add = new Index3(1, 0, 0); break;
            //            case OrientationFlags.SideSouth: add = new Index3(0, -1, 0); break;
            //            case OrientationFlags.SideNorth: add = new Index3(0, 1, 0); break;
            //            case OrientationFlags.SideBottom: add = new Index3(0, 0, -1); break;
            //            case OrientationFlags.SideTop: add = new Index3(0, 0, 1); break;
            //        }

            //        if (ActiveTool.Definition is IBlockDefinition)
            //        {
            //            IBlockDefinition definition = ActiveTool.Definition as IBlockDefinition;

            //            Index3 idx = lastApply.Value + add;
            //            var boxes = definition.GetCollisionBoxes(localChunkCache, idx.X, idx.Y, idx.Z);
            //            float gap = 0.01f;
            //            var playerBox = new BoundingBox(
            //                new Vector3(
            //                    Player.Position.GlobalBlockIndex.X + Player.Position.BlockPosition.X - Player.Radius + gap,
            //                    Player.Position.GlobalBlockIndex.Y + Player.Position.BlockPosition.Y - Player.Radius + gap,
            //                    Player.Position.GlobalBlockIndex.Z + Player.Position.BlockPosition.Z + gap),
            //                new Vector3(
            //                    Player.Position.GlobalBlockIndex.X + Player.Position.BlockPosition.X + Player.Radius - gap,
            //                    Player.Position.GlobalBlockIndex.Y + Player.Position.BlockPosition.Y + Player.Radius - gap,
            //                    Player.Position.GlobalBlockIndex.Z + Player.Position.BlockPosition.Z + Player.Height - gap)
            //                );

            //            Nicht in sich selbst reinbauen
            //            bool intersects = false;
            //            foreach (var box in boxes)
            //            {
            //                var newBox = new BoundingBox(idx + box.Min, idx + box.Max);
            //                if (newBox.Min.X < playerBox.Max.X && newBox.Max.X > playerBox.Min.X &&
            //                    newBox.Min.Y < playerBox.Max.Y && newBox.Max.X > playerBox.Min.Y &&
            //                    newBox.Min.Z < playerBox.Max.Z && newBox.Max.X > playerBox.Min.Z)
            //                    intersects = true;
            //            }

            //            if (!intersects)
            //            {
            //                localChunkCache.SetBlock(idx, DefinitionManager.Instance.GetBlockDefinitionIndex(definition));

            //                ActiveTool.Amount -= 125;
            //                if (ActiveTool.Amount <= 0)
            //                {
            //                    Player.Inventory.Remove(ActiveTool);
            //                    for (int i = 0; i < Player.Tools.Length; i++)
            //                    {
            //                        if (Player.Tools[i] == ActiveTool)
            //                            Player.Tools[i] = null;
            //                    }
            //                    ActiveTool = null;
            //                }
            //            }
            //        }

            //        TODO: Fix Interaction;)
            //        ushort block = _manager.GetBlock(lastApply.Value);
            //        IBlockDefinition blockDefinition = BlockDefinitionManager.GetForType(block);
            //        IItemDefinition itemDefinition = ActiveTool.Definition;

            //        blockDefinition.Hit(blockDefinition, itemDefinition.GetProperties(null));
            //        itemDefinition.Hit(null, blockDefinition.GetProperties(block));
            //    }

            //    lastApply = null;
            //}

            #endregion
        }

        internal void Unload()
        {
            PlayerPosition.LocalChunkCache.Flush();
        }

        /// <summary>
        /// Position des Spielers.
        /// </summary>
        public Coordinate Position => PlayerPosition.Coordinate;

        /// <summary>
        /// Radius des Spielers.
        /// </summary>
        public float Radius => PlayerPosition.Radius;

        /// <summary>
        /// Winkel des Spielers (Standposition).
        /// </summary>
        public float Angle => PlayerLook.Angle;

        /// <summary>
        /// Höhe des Spielers.
        /// </summary>
        public float Height => PlayerPosition.Height;

        /// <summary>
        /// Gibt an, ob der Spieler auf dem Boden steht.
        /// </summary>
        public bool OnGround => PlayerPosition.OnGround;

        /// <summary>
        /// Winkel der Kopfstellung.
        /// </summary>
        public float Tilt => PlayerLook.Tilt;

        /// <summary>
        /// Bewegungsvektor des Spielers.
        /// </summary>
        public Vector2 Move { get { return PlayerMoveable.Move; } set { PlayerMoveable.Move = value; } }

        /// <summary>
        /// Kopfbewegeungsvektor des Spielers.
        /// </summary>
        public Vector2 Head { get { return PlayerLook.Head; } set { PlayerLook.Head = value; } }

        /// <summary>
        /// Den Spieler hüpfen lassen.
        /// </summary>
        public void Jump()
        {
            PlayerJump.Jump = true;
        }

        /// <summary>
        /// Lässt den Spieler einen Block entfernen.
        /// </summary>
        /// <param name="blockIndex"></param>
        public void Interact(Index3 blockIndex)
        {
            lastInteract = blockIndex;
        }

        /// <summary>
        /// Setzt einen neuen Block.
        /// </summary>
        /// <param name="blockIndex"></param>
        /// <param name="orientation"></param>
        public void Apply(Index3 blockIndex, OrientationFlags orientation)
        {
            lastApply = blockIndex;
            lastOrientation = orientation;
        }

        /// <summary>
        /// DEBUG METHODE: NICHT FÜR VERWENDUNG IM SPIEL!
        /// </summary>
        public void AllBlocksDebug()
        {
            var blockDefinitions = DefinitionManager.Instance.GetBlockDefinitions();

            foreach (var blockDefinition in blockDefinitions)
            {

                var slot = Player.Inventory.FirstOrDefault(s => s.Definition == blockDefinition);

                // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
                if (slot == null)
                {
                    slot = new InventorySlot()
                    {
                        Definition = blockDefinition,
                        Amount = 0
                    };
                    Player.Inventory.Add(slot);
                }
                slot.Amount += 125;
            }
        }
    }
}
