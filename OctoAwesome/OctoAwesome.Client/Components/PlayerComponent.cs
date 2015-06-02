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

        public IBlockDefinition BlockTool { get; set; }

        public PlayerComponent(Game game, InputComponent input, SimulationComponent simulation)
            : base(game)
        {
            this.simulation = simulation;
            this.input = input;
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

            if (input.Slot1Trigger)
            {
                Player.BlockTool = BlockDefinitionManager.GetBlockDefinitions().ToArray()[1];
            }
            if (input.Slot2Trigger)
            {
                Player.BlockTool = BlockDefinitionManager.GetBlockDefinitions().ToArray()[2];
            }
            if (input.Slot3Trigger)
            {
                Player.BlockTool = BlockDefinitionManager.GetBlockDefinitions().ToArray()[3];
            }
            if (input.Slot4Trigger)
            {
                Player.BlockTool = BlockDefinitionManager.GetBlockDefinitions().ToArray()[5];
            }
            //if (input.Slot5Trigger)
            //{
            //    Player.BlockTool = BlockDefinitionManager.GetBlockDefinitions().ToArray()[4];
            //}
        }
    }
}
