using System;
using engenious;
using engenious.Helper;

namespace OctoAwesome.Client.Components
{
    internal sealed class CameraComponent : DrawableGameComponent
    {
        private PlayerComponent player;


        public CameraComponent(OctoGame game)
            : base(game)
        {
            player = game.Player;
        }

        public override void Initialize()
        {
            base.Initialize();

            RecreateProjection();
        }

        public void RecreateProjection()
        {
            Projection = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.PiOver4, GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (player == null || player.CurrentEntity == null)
                return;
            
            Coordinate position = player.CurrentEntity.Position;

            CameraChunk = position.ChunkIndex;

            CameraPosition = position.LocalPosition + player.HeadOffset;

            float height = (float)Math.Sin(player.Tilt);
            float distance = (float)Math.Cos(player.Tilt);

            float lookX = (float)Math.Cos(player.Yaw) * distance;
            float lookY = -(float)Math.Sin(player.Yaw) * distance;

            float strafeX = (float)Math.Cos(player.Yaw + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(player.Yaw + MathHelper.PiOver2);

            CameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            View = Matrix.CreateLookAt(CameraPosition, 
                new Vector3(CameraPosition.X + lookX, CameraPosition.Y + lookY, CameraPosition.Z + height), 
                CameraUpVector);

            MinimapView = Matrix.CreateLookAt(
                new Vector3(CameraPosition.X, CameraPosition.Y, 100),
                new Vector3(position.LocalPosition.X, position.LocalPosition.Y, 0f),
                new Vector3((float)Math.Cos(player.Yaw), (float)Math.Sin(-player.Yaw), 0f));

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;

            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 0f), Projection, View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new Ray(nearPoint, direction);
            Frustum = new BoundingFrustum(Projection*View);
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
