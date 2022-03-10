using engenious;
using engenious.Helper;

using OctoAwesome.Client.Controls;
using OctoAwesome.EntityComponents;

using System;

namespace OctoAwesome.Client.Components
{
    internal sealed class CameraComponent : DrawableGameComponent
    {
        private PlayerComponent player;
        private OctoGame game;

        public CameraComponent(OctoGame game)
            : base(game)
        {
            player = game.Player;
            this.game = game;
        }

        public override void Initialize()
        {
            base.Initialize();

            RecreateProjection();
        }

        public void RecreateProjection(int overrideFOV = 0)
        {

            int fov;
            if (overrideFOV > 0)
                fov = overrideFOV;
            else
                fov = game.Settings.Get<int>("FOV");
            Projection = Matrix.CreatePerspectiveFieldOfView(
                (float)(fov / 180f * Math.PI), GraphicsDevice.Viewport.AspectRatio, 0.1f, 10000f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (player == null || player.CurrentEntity == null)
                return;

            HeadComponent head = player.CurrentEntityHead;
            PositionComponent position = player.Position;

            CameraChunk = position.Position.ChunkIndex;
            var viewCreator = ViewCreator;
            (CameraPosition, var lookAt, CameraUpVector) = viewCreator.CreateCameraUpVectorAndView(head, position.Position);

            var ray = new Ray(lookAt, CameraPosition - lookAt);

            if (!viewCreator.IsFirstPerson && player.CurrentEntity.Components.TryGetComponent<LocalChunkCacheComponent>(out var localChunkCacheComponent))
            {
                Index3 centerblock = player.Position.Position.GlobalBlockIndex;
                Index3 renderOffset = player.Position.Position.ChunkIndex * Chunk.CHUNKSIZE;

                var bi = SceneControl.GetSelectedBlock(centerblock, renderOffset, localChunkCacheComponent.LocalChunkCache, game.DefinitionManager, ray, position.Planet.Size, out _, out _, out _, out var distance);

                var funcBlock = SceneControl.GetSelectedFunctionalBlock(centerblock, renderOffset, game.Simulation.Simulation, ray, position.Planet.Size, out _, out _, out _, out var bestFunctionalBlockDistance);

                if (distance > bestFunctionalBlockDistance)
                    distance = bestFunctionalBlockDistance;

                if ((bi.Block > 0 || funcBlock is not null) && distance < 1)
                {
                    var selectionPoint = (ray.Position + (ray.Direction * distance * 0.9f));
                    CameraPosition = selectionPoint;
                }
            }

            View = Matrix.CreateLookAt(CameraPosition, lookAt, CameraUpVector);

            MinimapView = Matrix.CreateLookAt(
                new Vector3(CameraPosition.X, CameraPosition.Y, 100),
                new Vector3(
                    position.Position.LocalPosition.X,
                    position.Position.LocalPosition.Y,
                    0f),
                new Vector3(
                    (float)Math.Cos(head.Angle),
                    (float)Math.Sin(-head.Angle), 0f));

            float centerX = GraphicsDevice.Viewport.Width / 2;
            float centerY = GraphicsDevice.Viewport.Height / 2;


            Vector3 nearPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 0f), Projection, View, Matrix.Identity);
            Vector3 farPoint = GraphicsDevice.Viewport.Unproject(new Vector3(centerX, centerY, 1f), Projection, View, Matrix.Identity);
            Vector3 direction = farPoint - nearPoint;
            direction.Normalize();
            PickRay = new Ray(nearPoint, direction);
            Frustum = new BoundingFrustum(Projection * View);
        }

        public Index3 CameraChunk { get; private set; }

        public Vector3 CameraPosition { get; private set; }

        public Vector3 CameraUpVector { get; private set; }

        public Matrix View { get; private set; }

        public Matrix MinimapView { get; private set; }

        public Matrix Projection { get; private set; }

        public Ray PickRay { get; private set; }

        public BoundingFrustum Frustum { get; private set; }
        public IViewCreator ViewCreator { get; set; } = new FirstPersonViewCreator();
        public float NearPlaneDistance => 0.1f;
        public float FarPlaneDistance => 10000.0f;
    }

    public class ThirdPersonViewCreator : IViewCreator
    {
        public bool IsFirstPerson => false;

        public (Vector3 cameraPosition, Vector3 lookAt, Vector3 cameraUpVector) CreateCameraUpVectorAndView(HeadComponent head, Coordinate playerPos)
        {
            float height = (float)Math.Sin(head.Tilt);
            float distance = (float)Math.Cos(head.Tilt);

            float lookX = (float)Math.Cos(head.Angle) * distance;
            float lookY = -(float)Math.Sin(head.Angle) * distance;

            float strafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
            var cameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            var lookAt = playerPos.LocalPosition + head.Offset;
            var cameraPosition = new Vector3(
                                (lookAt.X + lookX * -5),
                                (lookAt.Y + lookY * -5),
                                lookAt.Z + height * -5);
            return (cameraPosition, lookAt, cameraUpVector);
        }

    }

    public class DroneViewCreator : IViewCreator
    {
        public bool IsFirstPerson => false;

        public (Vector3 cameraPosition, Vector3 lookAt, Vector3 cameraUpVector) CreateCameraUpVectorAndView(HeadComponent head, Coordinate playerPos)
        {
            float height = (float)Math.Sin(head.Tilt);
            float distance = (float)Math.Cos(head.Tilt);

            float lookX = (float)Math.Cos(head.Angle) * distance;
            float lookY = -(float)Math.Sin(head.Angle) * distance;

            float strafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
            var cameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            var lookAt = playerPos.LocalPosition + head.Offset;
            var cameraPosition = new Vector3(
                                (lookAt.X + lookX * 5),
                                (lookAt.Y + lookY * 5),
                                lookAt.Z + height * 5);
            return (cameraPosition, lookAt, cameraUpVector);
        }

    }
    public class FirstPersonViewCreator : IViewCreator
    {
        public bool IsFirstPerson => true;

        public (Vector3 cameraPosition, Vector3 lookAt, Vector3 cameraUpVector) CreateCameraUpVectorAndView(HeadComponent head, Coordinate playerPos)
        {
            float height = (float)Math.Sin(head.Tilt);
            float distance = (float)Math.Cos(head.Tilt);

            float lookX = (float)Math.Cos(head.Angle) * distance;
            float lookY = -(float)Math.Sin(head.Angle) * distance;

            float strafeX = (float)Math.Cos(head.Angle + MathHelper.PiOver2);
            float strafeY = -(float)Math.Sin(head.Angle + MathHelper.PiOver2);
            var cameraUpVector = Vector3.Cross(new Vector3(strafeX, strafeY, 0), new Vector3(lookX, lookY, height));

            var cameraPosition = playerPos.LocalPosition + head.Offset;
            var lookAt = new Vector3(
                                (cameraPosition.X + lookX),
                                (cameraPosition.Y + lookY),
                                cameraPosition.Z + height);
            return (cameraPosition, lookAt, cameraUpVector);
        }

    }
}
