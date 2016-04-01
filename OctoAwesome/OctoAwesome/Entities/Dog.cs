using OctoAwesome.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Dog Entity
    /// TODO: Move to OctoAwesome.Basics
    /// </summary>
    public class Dog : ControllableEntity
    {
        /// <summary>
        /// Erzeugt eine neue Instanz eines Hundes.
        /// </summary>
        /// <param name="coordinate">Die Position des Hundes.</param>
        public Dog(Coordinate coordinate)
        {
            Position = coordinate;
            Radius = 0.5f;
            Height = 1f;
            Mass = 100;
        }
    }
}
