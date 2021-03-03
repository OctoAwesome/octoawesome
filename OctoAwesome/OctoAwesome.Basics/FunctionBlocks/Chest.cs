using engenious;
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
        private readonly InventoryComponent inventoryComponent;

        public Chest(Coordinate position)
        {
            inventoryComponent = new InventoryComponent();

            Components.AddComponent(inventoryComponent);
            Components.AddComponent(new PositionComponent()
            {
                Position = position
            });

            Components.AddComponent(new BodyComponent() { Height = 0.005f, Radius = 0.002f });
            Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }));
            Components.AddComponent(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchest", BaseZRotation = -90 }, true);

        }

        protected override void OnInteract(GameTime gameTime)
        {
        }
    }
}
