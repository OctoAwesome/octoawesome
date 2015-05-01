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
        public World World { get; private set; }

        public ActorHost Player { get { return World.Player; } }

        public WorldComponent(Game game, InputComponent input)
            : base(game)
        {
            World = new World(input); 
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }

        protected override void Dispose(bool disposing)
        {
            World.Save();

            base.Dispose(disposing);
        }
    }
}
