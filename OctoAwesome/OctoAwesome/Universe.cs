using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Ein Universum von OctoAwesome. Ein Universum beinhaltet verschiedene Planeten und entspricht einem Speicherstand.
    /// </summary>
    public class Universe : IUniverse
    {
        public Universe(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        /// <summary>
        /// ID des Universums
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Der Name des Universums
        /// </summary>
        public string Name { get; private set; }
    }
}
