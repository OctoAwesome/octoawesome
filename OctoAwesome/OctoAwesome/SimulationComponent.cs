using engenious;
using OctoAwesome.Entities;
namespace OctoAwesome
{
    /// <summary>
    /// Base class for World wide (web) simulations
    /// </summary>
    public abstract class SimulationComponent : Component
    {
        /// <summary>
        /// Defaultconstructor
        /// </summary>
        public SimulationComponent() : base(true)
        {

        }
        /// <summary>
        /// Constructor with needupdate parameter
        /// </summary>
        /// <param name="needupdate">Indicates if this <see cref="Component"/> should be updated</param>
        public SimulationComponent(bool needupdate) : base(needupdate)
        {

        }
        /// <summary>
        /// Register an <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">Entity to register</param>
        public abstract void Register(Entity entity);
        /// <summary>
        /// Remove an <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity">Entity to remove</param>
        public abstract void Remove(Entity entity);
    }
}
