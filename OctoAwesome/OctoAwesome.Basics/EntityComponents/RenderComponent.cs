using OctoAwesome.Entities;

namespace OctoAwesome.EntityComponents
{
    public class RenderComponent : EntityComponent
    {
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string TextureName { get; set; }

        public float BaseZRotation { get; set; }
    }
}
