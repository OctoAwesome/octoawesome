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
            CameraPosition = new Vector3(world.World.Player.Position.X, 60, world.World.Player.Position.Y + 10);
            CameraUpVector = Vector3.Up;

            View = Matrix.CreateLookAt(
                CameraPosition, 
                new Vector3(world.World.Player.Position.X, 50, world.World.Player.Position.Y),
                CameraUpVector);
        }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }
    }
}
