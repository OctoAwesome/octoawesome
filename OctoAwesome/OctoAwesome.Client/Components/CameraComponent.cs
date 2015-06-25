using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class CameraComponent : DrawableGameComponent
    {
        private PlayerComponent player;

        public CameraComponent(Game game, PlayerComponent player)
            : base(game)
        {
            this.player = player;
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
                player.Player.Position.LocalPosition.X,
                player.Player.Position.LocalPosition.Y,
                player.Player.Position.LocalPosition.Z + 3.2f);
            CameraUpVector = new Vector3(0, 0, 1f);

            float height = (float)Math.Sin(player.Player.Tilt);
            float distance = (float)Math.Cos(player.Player.Tilt);

            float lookX = (float)Math.Cos(player.Player.Angle) * distance;
            float lookY = -(float)Math.Sin(player.Player.Angle) * distance;

            float strafeX = (float)Math.Cos(player.Player.Angle + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(player.Player.Angle + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            View = Matrix.CreateLookAt(
                CameraPosition,
                new Vector3(
                    player.Player.Position.LocalPosition.X + lookX,
                    player.Player.Position.LocalPosition.Y + lookY,
                    player.Player.Position.LocalPosition.Z + 3.2f + height),
                CameraUpVector);

            MinimapView = Matrix.CreateLookAt(
                new Vector3(CameraPosition.X, CameraPosition.Y, 100),
                new Vector3(
                    player.Player.Position.LocalPosition.X,
                    player.Player.Position.LocalPosition.Y,
                    0f),
                new Vector3(0f, -1f, 0f));

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 0f), Projection, View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new Ray(nearPoint, direction);
            Frustum = new BoundingFrustum(View * Projection);
        }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix MinimapView { get; private set; }

        public Matrix Projection { get; private set; }

        public Ray PickRay { get; private set; }

        public BoundingFrustum Frustum { get; private set; }
    }
}
