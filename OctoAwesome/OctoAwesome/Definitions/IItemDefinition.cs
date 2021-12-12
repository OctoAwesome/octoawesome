using OctoAwesome.Definitions.Items;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface für die Definition eînes Items
    /// </summary>
    public interface IItemDefinition : IDefinition
    {

        bool CanMineMaterial(IMaterialDefinition material);
        Item Create(IMaterialDefinition material);
    }
}
