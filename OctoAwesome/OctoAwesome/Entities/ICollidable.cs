using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Entities
{
    interface ICollidable : IBody
    {
        /// <summary>
        /// Die Masse der Entität. 
        /// </summary>
        float Mass { get; set; }

        /// <summary>
        /// Kraft die von aussen auf die Entität wirkt.
        /// </summary>
        Vector3 ExternalForce { get; set; }

        /// <summary>
        /// Gibt an, ob der Spieler an Boden ist
        /// </summary>
        bool OnGround { get; set; }
    }
}
