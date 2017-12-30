namespace OctoAwesome
{
    /// <summary>
    /// Repräsentiert die physikalischen Eigenschaften eines Blocks/Items/...
    /// </summary>
    public class PhysicalProperties
    {
        /// <summary>
        /// Härte
        /// </summary>
        public float Hardness { get; set; }

        /// <summary>
        /// Dichte in kg/dm^3
        /// </summary>
        public float Density { get; set; }

        /// <summary>
        /// Granularität
        /// </summary>
        public float Granularity { get; set; }

        /// <summary>
        /// Bruchzähigkeit
        /// </summary>
        public float FractureToughness { get; set; }
    }
}
