using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Entities
{
    /// <summary>
    /// Dog Entity
    /// TODO: Move to OctoAwesome.Basics
    /// </summary>
    public class Dog : Entity
    {
        public Dog(Coordinate coordinate)
        {
            Position = coordinate;
            Radius = 0.5f;
            Height = 1f;
            Mass = 100;
        }
    }
}
