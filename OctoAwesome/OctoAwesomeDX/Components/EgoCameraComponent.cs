using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{
    internal sealed class EgoCameraComponent : DrawableGameComponent
    {
        private WorldComponent world;

        public EgoCameraComponent(Game game, WorldComponent world)
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
            CameraPosition = new Vector3(world.World.Player.Position.X, 51, world.World.Player.Position.Y);
            CameraUpVector = Vector3.Up;

            float height = (float)Math.Sin(world.World.Player.Tilt);
            float distance = (float)Math.Cos(world.World.Player.Tilt);

            float lookX = (float)Math.Cos(world.World.Player.Angle) * distance;
            float lookY = (float)Math.Sin(world.World.Player.Angle) * distance;

            float strafeX = (float)Math.Cos(world.World.Player.Angle + MathHelper.PiOver2);
            float strafeY = (float)Math.Sin(world.World.Player.Angle + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new Vector3(strafeX, 0, strafeY), new Vector3(lookX, height, lookY));

            View = Matrix.CreateLookAt(
                CameraPosition,
                new Vector3(world.World.Player.Position.X + lookX, 51 + height, world.World.Player.Position.Y + lookY),
                CameraUpVector);
        }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }
    }
}
