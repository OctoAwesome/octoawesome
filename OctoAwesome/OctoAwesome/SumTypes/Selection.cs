using dotVariant;

namespace OctoAwesome.SumTypes
{
    /// <summary>
    /// Selection variant of either <see cref="BlockInfo"/>, <see cref="Entity"/>.
    /// </summary>
    [Variant]
    public partial class Selection
    {
        static partial void VariantOf(HitInfo hitInfo, ApplyInfo applyInfo, ComponentContainer entity);
    }
}
