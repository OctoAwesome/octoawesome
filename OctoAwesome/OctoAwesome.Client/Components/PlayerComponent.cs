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

        public ActorHost Player { get { return simulation.World.Player; } }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public IBlockDefinition[] Tools { get; set; }

        public PlayerComponent(Game game, InputComponent input, SimulationComponent simulation)
            : base(game)
        {
            this.simulation = simulation;
            this.input = input;
        }

        public override void Initialize()
        {
            base.Initialize();
            Tools = BlockDefinitionManager.GetBlockDefinitions().ToArray();
            if (Tools != null && Tools.Length > 0)
                Player.ActiveTool = Tools[0];
        }

        public override void Update(GameTime gameTime)
        {
            Player.Head = new Vector2(input.HeadX, input.HeadY);
            Player.Move = new Vector2(input.MoveX, input.MoveY);

            if (input.JumpTrigger)
                Player.Jump();
            if (input.InteractTrigger && SelectedBox.HasValue)
            {
                Player.Interact(SelectedBox.Value);
            }
            if (input.ApplyTrigger && SelectedBox.HasValue)
            {
                Player.Apply(SelectedBox.Value, SelectedSide);
            }

            if (Tools != null && input.SlotTrigger != null)
            {
                for (int i = 0; i < Math.Min(Tools.Length, input.SlotTrigger.Length); i++)
                {
                    if (input.SlotTrigger[i])
                        Player.ActiveTool = Tools[i];
                }
            }

            // Index des aktiven Werkzeugs ermitteln
            int activeTool = -1;
            if (Tools != null && Player.ActiveTool != null)
            {
                for (int i = 0; i < Tools.Length; i++)
                {
                    if (Tools[i] == Player.ActiveTool)
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

                activeTool = (activeTool + Tools.Length) % Tools.Length;
                Player.ActiveTool = Tools[activeTool];
            }

        }
    }
}
