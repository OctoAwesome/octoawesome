using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Entität, die dauerhaft simuliert werden muss (z.B. Spieler)
    /// </summary>
    public class PermanentEntity : ControllableEntity
    {
        /// <summary>
        /// Activierungsradius
        /// </summary>
        public readonly int ActivationRange;

        public PermanentEntity(int activationRange)
        {
            ActivationRange = activationRange;
        }
    }
}
