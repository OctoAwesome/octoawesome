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
        public Chest(Coordinate position)
        {
            Components.AddComponent(new InventoryComponent());
            Components.AddComponent(new PositionComponent()
            {
                Position = position
            });

            Components.AddComponent(new BodyComponent() { Height = 0.005f, Radius = 0.002f });
            Components.AddComponent(new BoxCollisionComponent());
            Components.AddComponent(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchest", BaseZRotation = -90 }, true);

        }
    }
}
