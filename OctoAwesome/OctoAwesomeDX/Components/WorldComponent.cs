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
        public World World { get; private set; }

        public WorldComponent(Game game, InputComponent input)
            : base(game)
        {
            World = new Model.World(input);
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }
    }
}
