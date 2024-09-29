using OctoAwesome.Definitions.Items;

using System.Linq;

namespace OctoAwesome.Definitions
{
    public class ItemDefinition : IItemDefinition
    {
        public string DisplayName { get; init; }
        public string Icon { get; init; }

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
        public bool CanMineMaterial(IMaterialDefinition material) => throw new System.NotImplementedException();
        public Item? Create(IMaterialDefinition material) => throw new System.NotImplementedException();

    }
}
