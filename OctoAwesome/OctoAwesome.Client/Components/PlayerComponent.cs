using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent
    {
        private SimulationComponent simulation;

        #region External Input

        public Vector2 HeadInput { get; set; }

        public Vector2 MoveInput { get; set; }

        public bool InteractInput { get; set; }

        public bool ApplyInput { get; set; }

        public bool JumpInput { get; set; }

        public bool FlymodeInput { get; set; }

        public bool[] SlotInput { get; private set; } = new bool[20];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }

        #endregion

        public ActorHost ActorHost { get; private set; }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public List<InventorySlot> Tools { get; set; }

        public PlayerComponent(Game game, SimulationComponent simulation)
            : base(game)
        {
            this.simulation = simulation;
        }

        public override void Initialize()
        {
            base.Initialize();
            Tools = new List<InventorySlot>();
        }

        public void InsertPlayer()
        {
            Player player = ResourceManager.Instance.LoadPlayer("Adam");
            ActorHost = simulation.InsertPlayer(player);
        }

        public void RemovePlayer()
        {
            if (ActorHost == null)
                return;

            ResourceManager.Instance.SavePlayer(ActorHost.Player);
            simulation.RemovePlayer(ActorHost);
            ActorHost = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (ActorHost == null)
                return;

            Tools.Clear();
            Tools.AddRange(ActorHost.Player.Inventory);

            ActorHost.Head = HeadInput;
            HeadInput = Vector2.Zero;

            ActorHost.Move = MoveInput;
            MoveInput = Vector2.Zero;

            if (JumpInput)
                ActorHost.Jump();
            JumpInput = false;

            if (InteractInput && SelectedBox.HasValue)
                ActorHost.Interact(SelectedBox.Value);
            InteractInput = false;

            if (ApplyInput && SelectedBox.HasValue)
                ActorHost.Apply(SelectedBox.Value, SelectedSide);
            ApplyInput = false;

            if (FlymodeInput)
                ActorHost.Player.FlyMode = !ActorHost.Player.FlyMode;
            FlymodeInput = false;

            if (Tools != null && Tools.Count > 0)
            {
                if (ActorHost.ActiveTool == null) ActorHost.ActiveTool = Tools[0];
                for (int i = 0; i < Math.Min(Tools.Count, SlotInput.Length); i++)
                {
                    if (SlotInput[i])
                        ActorHost.ActiveTool = Tools[i];
                    SlotInput[i] = false;
                }
            }

            // Index des aktiven Werkzeugs ermitteln
            int activeTool = -1;
            if (Tools != null && ActorHost.ActiveTool != null)
            {
                for (int i = 0; i < Tools.Count; i++)
                {
                    if (Tools[i] == ActorHost.ActiveTool)
                    {
                        activeTool = i;
                        break;
                    }
                }
            }

            if (activeTool > -1)
            {
                if (SlotLeftInput)
                    activeTool--;
                SlotLeftInput = false;

                if (SlotRightInput)
                    activeTool++;
                SlotRightInput = false;

                activeTool = (activeTool + Tools.Count) % Tools.Count;
                ActorHost.ActiveTool = Tools[activeTool];
            }
        }
    }
}
