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
            CameraPosition = new Vector3(world.World.Player.Position.X, world.World.Player.Position.Y + 4f, world.World.Player.Position.Z);
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
                new Vector3(world.World.Player.Position.X + lookX, world.World.Player.Position.Y + 4f + height, world.World.Player.Position.Z + lookY),
                CameraUpVector);

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 0f), Projection, View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new Ray(nearPoint, direction);
        }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix Projection { get; private set; }

        public Ray PickRay { get; set; }
    }
}
