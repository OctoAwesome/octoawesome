using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class SimulationComponent : GameComponent
    {
        public World World { get; private set; }

        public ActorHost Player { get; private set; }
        
        public SimulationComponent(Game game) : base(game) { }

        public override void Initialize()
        {
            World = new World();

            Player = World.InjectPlayer(new Player());

            base.Initialize();
        }

        public override void Update(GameTime gameTime)
        {
            World.Update(gameTime);
        }
    }
}
