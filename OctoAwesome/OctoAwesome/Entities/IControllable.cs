using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Interface for controllable entities (Dogs, Cats, etc.)
    /// </summary>
    public interface IControllable
    {
        /// <summary>
        /// Horizontal angle of the Entity.
        /// </summary>
        float Azimuth { get; }
        /// <summary>
        /// Position in the world
        /// </summary>
        Coordinate Position { get; }
        /// <summary>
        /// Controller
        /// </summary>
        IEntityController Controller { get; }
        /// <summary>
        /// Register a controller
        /// </summary>
        /// <param name="controller">Controller to register</param>
        void Register(IEntityController controller);
        /// <summary>
        /// Reset the controller
        /// </summary>
        void Reset();
        /// <summary>
        /// Set the Position of the Entity.
        /// </summary>
        /// <param name="position">New Position.</param>
        /// <param name="azimuth">Horizontal angle of the <see cref="Entity"/></param>
        void SetPosition(Coordinate position, float azimuth);
    }
}
