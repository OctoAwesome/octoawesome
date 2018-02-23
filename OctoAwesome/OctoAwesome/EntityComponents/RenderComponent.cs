using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public class RenderComponent : EntityComponent
    {
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string TextureName { get; set; }
        public float BaseZRotation { get; set; }
        public RenderComponent(Entity entity) : base(entity)
        {
        }
    }
}
