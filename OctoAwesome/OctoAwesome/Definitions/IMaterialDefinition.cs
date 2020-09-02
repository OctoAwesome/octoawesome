namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Repräsentiert die physikalischen Eigenschaften eines Blocks/Items/...
    /// </summary>
    public interface IMaterialDefinition : IDefinition
    {
        /// <summary>
        /// Härte, welche Materialien können abgebaut werden
        /// </summary>
        int Hardness { get; }

        /// <summary>
        /// Dichte in kg/m^3, Wie viel benötigt (Volumen berechnung) für Crafting bzw. hit result etc....
        /// </summary>
        int Density { get; }
               

        /// <summary>
        /// Bruchzähigkeit, Wie schnell geht etwas zu Bruch? Haltbarkeit.
        /// </summary>
        int FractureToughness { get; }
    }
}
