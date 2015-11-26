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
            CameraChunk = player.PlayerController.Position.ChunkIndex;

            CameraPosition = new Vector3(
                player.PlayerController.Position.LocalPosition.X,
                player.PlayerController.Position.LocalPosition.Y,
                player.PlayerController.Position.LocalPosition.Z + 3.2f);
            CameraUpVector = new Vector3(0, 0, 1f);

            float height = (float)Math.Sin(player.PlayerController.Tilt);
            float distance = (float)Math.Cos(player.PlayerController.Tilt);

            float lookX = (float)Math.Cos(player.PlayerController.Angle) * distance;
            float lookY = -(float)Math.Sin(player.PlayerController.Angle) * distance;

            float strafeX = (float)Math.Cos(player.PlayerController.Angle + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(player.PlayerController.Angle + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            View = Matrix.CreateLookAt(
                CameraPosition,
                new Vector3(
                    player.PlayerController.Position.LocalPosition.X + lookX,
                    player.PlayerController.Position.LocalPosition.Y + lookY,
                    player.PlayerController.Position.LocalPosition.Z + 3.2f + height),
                CameraUpVector);

            MinimapView = Matrix.CreateLookAt(
                new Vector3(CameraPosition.X, CameraPosition.Y, 100),
                new Vector3(
                    player.PlayerController.Position.LocalPosition.X,
                    player.PlayerController.Position.LocalPosition.Y,
                    0f),
                new Vector3(
                    (float)Math.Cos(player.PlayerController.Angle), 
                    (float)Math.Sin(-player.PlayerController.Angle), 0f));

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 0f), Projection, View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new Ray(nearPoint, direction);
            Frustum = new BoundingFrustum(View * Projection);
        }

        public Index3 CameraChunk { get; private set; }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix MinimapView { get; private set; }

        public Matrix Projection { get; private set; }

        public Ray PickRay { get; private set; }

        public BoundingFrustum Frustum { get; private set; }
    }
}
