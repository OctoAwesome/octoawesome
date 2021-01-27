using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.FunctionBlocks
{
    public class Chest : FunctionalBlock
    {
        public Chest()
        {
            Components.AddComponent(new InventoryComponent());
            Components.AddComponent(new PositionComponent());
            Components.AddComponent(new RenderComponent());
            Components.AddComponent(new BodyComponent());
            Components.AddComponent(new BoxCollisionComponent());
        }
    }
}
