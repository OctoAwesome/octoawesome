using Microsoft.Xna.Framework;
using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class WorldComponent : GameComponent
    {
        private InputComponent input;

        public World World { get; private set; }

        public Vector3? SelectedBox { get; set; }

        public bool Dirty { get; set; }

        public WorldComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;

            World = new Model.World(input, 1);
            SelectedBox = null;
            Dirty = false;
        }

        public override void Update(GameTime gameTime)
        {
            if (input.ApplyTrigger && SelectedBox.HasValue)
            {
                World.DeleteBlock(
                    (int)SelectedBox.Value.X, 
                    (int)SelectedBox.Value.Y, 
                    (int)SelectedBox.Value.Z);
                Dirty = true;
            }

            World.Update(gameTime);
        }
    }
}
