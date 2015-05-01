using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class WorldComponent : GameComponent
    {
        private InputComponent input;

        public World World { get; private set; }

        public Vector3? SelectedBox { get; set; }

        public WorldComponent(Game game, InputComponent input)
            : base(game)
        {
            this.input = input;


            World = new World(input, 1);
            SelectedBox = null;
        }

        public override void Update(GameTime gameTime)
        {
            if (input.ApplyTrigger && SelectedBox.HasValue)
            {
                Index3 pos = new Index3(
                    (int)SelectedBox.Value.X,
                    (int)SelectedBox.Value.Y,
                    (int)SelectedBox.Value.Z);

                // ResourceManager.Instance.SetBlock(pos, null);
            }

            World.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            World.Save();

            base.Dispose(disposing);
        }
    }
}
