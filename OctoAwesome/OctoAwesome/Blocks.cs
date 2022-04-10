namespace OctoAwesome
{
    /// <summary>
    /// Constants and masks for blocks.
    /// </summary>
    public class Blocks
    {
        /// <summary>
        /// Constant block id for air blocks.
        /// </summary>
        public const ushort Air = 0;

        /// <summary>
        /// Mask for block type information.
        /// </summary>
        public const ushort TypeMask = (1 << 15) - 1;

        /// <summary>
        /// Mask for bit that represents whether the block needs to be updated or not.
        /// </summary>
        public const ushort UpdateMask = (1 << 15); // TODO: do something with it or remove it
    }
}
