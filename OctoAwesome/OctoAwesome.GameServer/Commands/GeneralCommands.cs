
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
        public static byte[] GetUniverse(CommandParameter parameter) // TODO: use parameter for multi universe server?
        {
            var universe = TypeContainer.Get<SimulationManager>().GetUniverse();
            return Serializer.Serialize(universe);
        }

        /// <summary>
        /// Gets the planet.
        /// </summary>
        /// <param name="parameter">This is currently ignored.</param>
        /// <returns>The planet with id 0 - for now.</returns>
        public static byte[] GetPlanet(CommandParameter parameter) // TODO: use parameter for actual planet server?
        {
            return Serializer.Serialize(TypeContainer.Get<SimulationManager>().GetPlanet(0));
        }
    }
}
