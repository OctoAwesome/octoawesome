using System.Collections.Generic;
using System.Linq;

namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base Interface for all definitions.
    /// </summary>
    public interface IDefinition
    {
        /// <summary>
        /// Gets the name of the definition.
        /// </summary>
        string DisplayName { get; }

        /// <summary>
        /// Gets the name of the icon resource.
        /// </summary>
        string Icon { get; }

        [Newtonsoft.Json.JsonProperty("@types")]
        public string[] Type => IDefinition.GetTypeProp(this).ToArray();

        static IEnumerable<string> GetTypeProp(object instance)
        {
            var realType = instance.GetType();

            if (realType.IsAssignableTo(typeof(IBlockDefinition)))
                yield return "core.block";

            if (realType.IsAssignableTo(typeof(IMaterialDefinition)))
                yield return "core.material";

            if (realType.IsAssignableTo(typeof(IFluidMaterialDefinition)))
                yield return "core.material_fluid";
            if (realType.IsAssignableTo(typeof(IGasMaterialDefinition)))
                yield return "core.material_gas";
            if (realType.IsAssignableTo(typeof(IFoodMaterialDefinition)))
                yield return "core.material_food";
            if (realType.IsAssignableTo(typeof(ISolidMaterialDefinition)))
                yield return "core.material_solid";
            if (realType.IsAssignableTo(typeof(IItemDefinition)))
                yield return "core.item";
        }

    }
}
