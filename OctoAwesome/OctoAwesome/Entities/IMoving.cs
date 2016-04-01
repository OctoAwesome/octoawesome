using Microsoft.Xna.Framework;

namespace OctoAwesome.Entities
{
    /// <summary>
    /// Schnittstelle für alle sich bewegenden Entitäten.
    /// </summary>
    public interface IMoving
    {
        /// <summary>
        /// Geschwindikeit der Entität als Vektor
        /// </summary>
        Vector3 Velocity { get; set; }
    }
}
