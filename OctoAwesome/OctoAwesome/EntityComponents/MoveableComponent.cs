using engenious;
using OctoAwesome.Ecs;

namespace OctoAwesome.EntityComponents
{
    [ComponentConfig(1000, 1000)]
    public sealed class MoveableComponent : Component<MoveableComponent>
    {
        public Vector3 Velocity;
        public Vector2 Move;
        public float Mass;
        public Vector3 Force;
        public float JumpForce;
        public bool Jumping;
        public Vector3 TransientForce;
        public override void CopyTo(MoveableComponent other)
        {
            other.Velocity = Velocity;
            other.Move = Move;
            other.Mass = Mass;
            other.Force = Force;
        }
    }

    [ComponentConfig(2000, 2000)]
    public sealed class PositionComponent : Component<PositionComponent>
    {
        public Coordinate Coordinate;
        public Vector3 Dimensions;
        public float Radius;
        public float Height;
        public IPlanet Planet;
        public ILocalChunkCache LocalChunkCache;
        public bool OnGround;

        public override void CopyTo(PositionComponent other)
        {
            other.Coordinate = Coordinate;
            other.Dimensions = Dimensions;
            other.Radius = Radius;
            other.Height = Height;
            other.LocalChunkCache = LocalChunkCache;
            other.Planet = Planet;
            other.OnGround = OnGround;
        }
    }
}
