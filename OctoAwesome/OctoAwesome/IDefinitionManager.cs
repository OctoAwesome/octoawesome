using System.Collections.Generic;

namespace OctoAwesome
{
    public interface IDefinitionManager
    {
        IEnumerable<IItemDefinition> GetItemDefinitions();

        IEnumerable<IResourceDefinition> GetResourceDefinitions();

        IEnumerable<IBlockDefinition> GetBlockDefinitions();

        IBlockDefinition GetBlockDefinitionByIndex(ushort index);

        ushort GetBlockDefinitionIndex(IBlockDefinition definition);
    }
}
