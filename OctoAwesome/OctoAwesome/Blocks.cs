namespace OctoAwesome
{
    /// <summary>
    /// Konstanten und Masken für die Blöcke
    /// </summary>
    public class Blocks
    {
        /// <summary>
        /// Konstante, die Luft repräsentiert
        /// </summary>
        public const ushort Air = 0;

        /// <summary>
        /// Maske für die Typen-Informationen
        /// </summary>
        public const ushort TypeMask = (1 << 15) - 1;

        /// <summary>
        /// Maske die das Bit markiert, das angibt, ob der Block geupdatet werden muss
        /// </summary>
        public const ushort UpdateMask = (1 << 15);
    }
}
