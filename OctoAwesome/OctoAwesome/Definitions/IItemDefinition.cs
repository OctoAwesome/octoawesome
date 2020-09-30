using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using System;

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
