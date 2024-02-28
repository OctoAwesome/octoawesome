using dotVariant;

namespace OctoAwesome.SumTypes
{
    /// <summary>
    /// Selection variant of either <see cref="HitInfo"/>, <see cref="ApplyInfo"/>, <see cref="ComponentContainer"/>.
    /// </summary>
    [Variant]
    public partial class Selection
    {
        static partial void VariantOf(HitInfo hitInfo, ApplyInfo applyInfo, ComponentContainer entity);
    }
}
