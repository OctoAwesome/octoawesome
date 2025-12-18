using dotVariant;

using OctoAwesome.Information;

namespace OctoAwesome.SumTypes
{
    public enum SelectionType
    {
        Hit,
        Interact
    }

    /// <summary>
    /// Selection variant of either <see cref="HitInfo"/>, <see cref="ApplyInfo"/>, <see cref="ComponentContainer"/>.
    /// </summary>
    [Variant]
    public partial class Selection
    {
        public SelectionType SelectionType { get; set; }

        static partial void VariantOf(HitInfo hitInfo, ComponentContainer entity);
    }
}
