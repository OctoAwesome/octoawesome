using OctoAwesome.Definitions.Items;
using System.Linq;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Interface for definition of an item.
    /// </summary>
    public interface IItemDefinition : IDefinition
    {
        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();
    }
}
