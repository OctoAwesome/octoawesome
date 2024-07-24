using System.Linq;

namespace OctoAwesome.Definitions
{
    public class MaterialDefinition : IMaterialDefinition
    {

        public int Hardness { get; set; }
        public int Density { get; set; }
        public string DisplayName { get; set; }
        public string Icon { get; set; }
    }
}
