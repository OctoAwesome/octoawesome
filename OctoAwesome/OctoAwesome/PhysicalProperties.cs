namespace OctoAwesome
{
    /// <summary>
    /// Repräsentiert die physikalischen Eigenschaften eines Blocks/Items/...
    /// </summary>
    public class PhysicalProperties
    {
        /// <summary>
        /// Härte, welche Materialien können abgebaut werden
        /// </summary>
        public float Hardness { get; set; }

        /// <summary>
        /// Dichte in kg/dm^3, Wie viel benötigt (Volumen berechnung) für Crafting bzw. hit result etc....
        /// </summary>
        public float Density { get; set; }

        /// <summary>
        /// Granularität, Effiktivität von "Materialien" Schaufel für hohe Werte, Pickaxe für niedrige
        /// </summary>
        public float Granularity { get; set; }

        /// <summary>
        /// Bruchzähigkeit, Wie schnell geht etwas zu bruch? Haltbarkeit.
        /// </summary>
        public float FractureToughness { get; set; }
    }
}
