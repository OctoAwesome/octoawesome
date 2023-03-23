
using OctoAwesome.Network;
using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome.GameServer.Commands
{
    /// <summary>
    /// Contains general remote commands.
    /// </summary>
    public static class GeneralCommands
    {
        /// <summary>
        /// Gets the current universe.
        /// </summary>
        /// <param name="parameter">This is currently ignored.</param>
        /// <returns>The universe data.</returns>
        public static ISerializable GetUniverse(ITypeContainer tc, CommandParameter parameter) // TODO: use parameter for multi universe server?
        {
            return tc.Get<SimulationManager>().GetUniverse();

        }

        /// <summary>
        /// Gets the planet.
        /// </summary>
        /// <param name="parameter">This is currently ignored.</param>
        /// <returns>The planet with id 0 - for now.</returns>
        public static ISerializable GetPlanet(ITypeContainer tc, CommandParameter parameter) // TODO: use parameter for actual planet server?
        {
            return tc.Get<SimulationManager>().GetPlanet(0);
        }
    }
}
