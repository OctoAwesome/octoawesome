using Microsoft.Xna.Framework;
using OctoAwesome.Entities;
using System.Xml.Serialization;

namespace OctoAwesome
{
    /// <summary>
    /// Basisklasse für alle selbständigen Wesen
    /// </summary>
    public abstract class Entity: IPosition, IRotatable
    {
        private float angle = 0f;

        /// <summary>
        /// Die Position der Entität
        /// </summary>
        public Coordinate Position { get; set; }

        /// <summary>
        /// Blickwinkel in der horizontalen Achse
        /// </summary>
        public float Angle
        {
            get { return angle; }
            set { angle = MathHelper.WrapAngle(value); }
        }

        /// <summary>
        /// Blickwinkel in der vertikalen Achse
        /// </summary>
        public float Tilt { get; set; }
    }
}
