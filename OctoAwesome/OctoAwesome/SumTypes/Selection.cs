using dotVariant;

namespace OctoAwesome.SumTypes
{
    /// <summary>
    /// Selection variant of either <see cref="BlockInfo"/>, <see cref="FunctionalBlock"/> or <see cref="Entity"/>.
    /// </summary>
    [Variant]
    public partial class Selection
    {
        static partial void VariantOf(BlockInfo blockinfo, FunctionalBlock functionalBlock, Entity entity);
    }
}
