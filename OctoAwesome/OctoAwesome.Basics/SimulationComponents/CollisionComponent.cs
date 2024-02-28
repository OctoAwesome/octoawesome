using engenious;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with collisions of entities.
    /// </summary>
    [SerializationId()]
    public sealed class CollisionComponent : SimulationComponent<Entity>
    {
        /// <inheritdoc />
        public override void Update(GameTime gameTime) { }

        /// <inheritdoc />
        protected override void OnAdd(Entity value) => throw new System.NotImplementedException();

        /// <inheritdoc />
        protected override void OnRemove(Entity value) => throw new System.NotImplementedException();
    }
}
