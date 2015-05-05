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

        private World world;

        public ActorHost Player { get { return world.Player; } }

        public Index3? SelectedBox { get; set; }

        public PlayerComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;
            world = new World();
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
            if (input.ApplyTrigger)
            {
                Player.Apply();
            }

            world.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            world.Save();

            base.Dispose(disposing);
        }
    }
}
