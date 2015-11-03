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
        private InputComponent input;

        private SimulationComponent simulation;

        public ActorHost ActorHost { get { return simulation.Player; } }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public List<InventorySlot> Tools { get; set; }

        public PlayerComponent(Game game, InputComponent input, SimulationComponent simulation)
            : base(game)
        {
            this.simulation = simulation;
            this.input = input;
        }

        public bool InputActive { get; set; }

        public override void Initialize()
        {
            base.Initialize();
            Tools = new List<InventorySlot>();
        }

        public override void Update(GameTime gameTime)
        {
            Tools.Clear();
            Tools.AddRange(ActorHost.Player.Inventory);

            if (InputActive)
            {
                input.UpdateInput(gameTime);

                ActorHost.Head = new Vector2(input.HeadX, input.HeadY);
                ActorHost.Move = new Vector2(input.MoveX, input.MoveY);

                if (input.JumpTrigger)
                    ActorHost.Jump();
                if (input.InteractTrigger && SelectedBox.HasValue)
                {
                    ActorHost.Interact(SelectedBox.Value);
                }
                if (input.ApplyTrigger && SelectedBox.HasValue)
                {
                    ActorHost.Apply(SelectedBox.Value, SelectedSide);
                }
                if (input.ToggleFlyMode)
                {
                    ActorHost.Player.FlyMode = !ActorHost.Player.FlyMode;
                }

                if (Tools != null && Tools.Count > 0 && input.SlotTrigger != null)
                {
                    if (ActorHost.ActiveTool == null) ActorHost.ActiveTool = Tools[0];
                    for (int i = 0; i < Math.Min(Tools.Count, input.SlotTrigger.Length); i++)
                    {
                        if (input.SlotTrigger[i])
                            ActorHost.ActiveTool = Tools[i];
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
                    if (input.SlotLeftTrigger)
                        activeTool--;

                    if (input.SlotRightTrigger)
                        activeTool++;

                    activeTool = (activeTool + Tools.Count) % Tools.Count;
                    ActorHost.ActiveTool = Tools[activeTool];
                }
            }
        }
    }
}
