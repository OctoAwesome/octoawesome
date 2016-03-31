using Microsoft.Xna.Framework;


namespace OctoAwesome.Entities
{
    interface IMoving
    {
        /// <summary>
        /// Geschwindikeit der Entität als Vektor
        /// </summary>
        Vector3 Velocity { get; set; }
    }
}
