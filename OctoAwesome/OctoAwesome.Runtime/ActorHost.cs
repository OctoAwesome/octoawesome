using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Kapselung eines Spielers.
    /// </summary>
    public class ActorHost : EntityHost, IPlayerController
    {
        private bool lastJump = false;

        private Index3? lastInteract = null;
        private Index3? lastApply = null;
        private OrientationFlags lastOrientation = OrientationFlags.None;

        /// <summary>
        /// Der Spieler dieses ActorHosts.
        /// </summary>
        public Player Player { get; private set; }

        /// <summary>
        /// Das zur Zeit aktive Werkzeug.
        /// </summary>
        public InventorySlot ActiveTool { get; set; }

        /// <summary>
        /// Erzeugt einen neuen ActorHost.
        /// </summary>
        /// <param name="player">Der Player</param>
        public ActorHost(Player player) : base(player)
        {
            Player = player;
            ActiveTool = null;
        }
        
        /// <summary>
        /// Aktualisiert den Spieler (Bewegung, Interaktion)
        /// </summary>
        /// <param name="frameTime">Die aktuelle Zeit.</param>
        protected override void BeforeUpdate(GameTime frameTime)
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

            if (Player.FlyMode)
            {
                velocitydirection += new Vector3(0, 0, (float)Math.Sin(Player.Tilt) * Move.Y);
                // friction = Vector3.One * Player.FRICTION;
            }

            Player.Velocity += PhysicalUpdate(velocitydirection, frameTime.ElapsedGameTime, !Player.FlyMode, lastJump);
            lastJump = false;

            #endregion

            #region Block Interaction

            if (lastInteract.HasValue)
            {
                ushort lastBlock = localChunkCache.GetBlock(lastInteract.Value);
                localChunkCache.SetBlock(lastInteract.Value, 0);

                if (lastBlock != 0)
                {
                    var blockDefinition = DefinitionManager.Instance.GetBlockDefinitionByIndex(lastBlock);

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
                        localChunkCache.SetBlock(lastApply.Value + add, DefinitionManager.Instance.GetBlockDefinitionIndex(definition));

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

        /// <summary>
        /// Position des Spielers.
        /// </summary>
        public Coordinate Position
        {
            get { return Player.Position; }
        }

        /// <summary>
        /// Radius des Spielers.
        /// </summary>
        public float Radius
        {
            get { return Player.Radius; }
        }

        /// <summary>
        /// Winkel des Spielers (Standposition).
        /// </summary>
        public float Angle
        {
            get { return Player.Angle; }
        }

        /// <summary>
        /// Höhe des Spielers.
        /// </summary>
        public float Height
        {
            get { return Player.Height; }
        }

        /// <summary>
        /// Gibt an, ob der Spieler auf dem Boden steht.
        /// </summary>
        public bool OnGround
        {
            get { return Player.OnGround; }
        }

        /// <summary>
        /// Winkel der Kopfstellung.
        /// </summary>
        public float Tilt
        {
            get { return Player.Tilt; }
        }

        /// <summary>
        /// Bewegungsvektor des Spielers.
        /// </summary>
        public Vector2 Move { get; set; }

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
        }
    }
}
