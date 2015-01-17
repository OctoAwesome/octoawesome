using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class Camera3DComponent : DrawableGameComponent
    {
        private WorldComponent world;

        public Camera3DComponent(Game game, WorldComponent world)
            : base(game)
        {
            this.world = world;
        }

        public override void Initialize()
        {
            base.Initialize();

            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 1f, 10000f);
        }

        public override void Update(GameTime gameTime)
        {
            View = Matrix.CreateLookAt(
                new Vector3(world.World.Player.Position.X, 10, world.World.Player.Position.Y + 35), 
                new Vector3(world.World.Player.Position.X, 0, world.World.Player.Position.Y), 
                Vector3.Up);
        }

        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }
    }
}
