using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Entities
{
    public interface IControllable
    {
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
    }
}
