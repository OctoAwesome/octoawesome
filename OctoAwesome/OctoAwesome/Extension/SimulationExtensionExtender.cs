
using OctoAwesome.Caching;

using System;
using System.Collections.Generic;

namespace OctoAwesome
{
    public class SimulationExtensionExtender : BaseExtensionExtender<Simulation>
    {
        private readonly List<Action<Simulation>> simulationExtender;

        public SimulationExtensionExtender()
        {
            simulationExtender = new();
        }

        /// <summary>
        /// Adds a new Extender for the simulation.
        /// </summary>
        /// <param name="extenderDelegate"></param>
        public override void RegisterExtender<T>(Action<T> extenderDelegate)
        {
            simulationExtender.Add(GenericCaster<Action<T>, Action<Simulation>>.Cast(extenderDelegate));
        }

        /// <summary>
        /// Extend a Simulation
        /// </summary>
        /// <param name="simulation">Simulation</param>
        public override void Execute<T>(T simulation)
        {
            foreach (var extender in simulationExtender)
                extender(simulation);
        }
    }
}
