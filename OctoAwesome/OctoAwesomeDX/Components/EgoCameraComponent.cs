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
                MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000f);
        }

        public override void Update(GameTime gameTime)
        {
            CameraPosition = new Vector3(
                world.World.Player.Position.LocalPosition.X,
                world.World.Player.Position.LocalPosition.Y,
                world.World.Player.Position.LocalPosition.Z + 3.2f);
            CameraUpVector = new Vector3(0, 0, 1f);

            float height = (float)Math.Sin(world.World.Player.Tilt);
            float distance = (float)Math.Cos(world.World.Player.Tilt);

            float lookX = (float)Math.Cos(world.World.Player.Angle) * distance;
            float lookY = -(float)Math.Sin(world.World.Player.Angle) * distance;

            float strafeX = (float)Math.Cos(world.World.Player.Angle + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(world.World.Player.Angle + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            View = Matrix.CreateLookAt(
                CameraPosition,
                new Vector3(
                    world.World.Player.Position.LocalPosition.X + lookX,
                    world.World.Player.Position.LocalPosition.Y + lookY,
                    world.World.Player.Position.LocalPosition.Z + 3.2f + height),
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
