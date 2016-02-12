using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Ein Universum von OctoAwesome. Ein Universum beinhaltet verschiedene Planeten und entspricht einem Speicherstand.
    /// </summary>
    [Serializable]
    public class Universe : IUniverse
    {
        public Universe()
        {
        }

        public Universe(Guid id, string name, int seed)
        {
            Id = id;
            Name = name;
            Seed = seed;
        }

        /// <summary>
        /// ID des Universums
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Der Name des Universums
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Universe Seed
        /// </summary>
        public int Seed { get; set; }
    }
}
