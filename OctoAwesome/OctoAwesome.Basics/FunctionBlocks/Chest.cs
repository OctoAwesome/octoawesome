using engenious;
using OctoAwesome.Basics.EntityComponents;
using OctoAwesome.Basics.EntityComponents.UIComponents;
using OctoAwesome.EntityComponents;
using OctoAwesome.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.FunctionBlocks
{
    [SerializationId(1, 3)]
    public class Chest : FunctionalBlock
    {
        internal InventoryComponent inventoryComponent;
        internal AnimationComponent animationComponent;
        internal TransferUIComponent transferUiComponent;

        public Chest()
        {

        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            //Doesnt get called
        }

        public Chest(Coordinate position)
        {

            Components.AddComponent(new PositionComponent()
            {
                Position = position
            });


            //Simulation.Entities.FirstOrDefault(x=>x.)
        }

        internal void TransferUiComponentClosed(object sender, engenious.UI.NavigationEventArgs e)
        {
            animationComponent.AnimationSpeed = -60f;
        }

        protected override void OnInteract(GameTime gameTime, Entity entity)
        {
            if (entity is Player p)
            {
                transferUiComponent.Show(p);
                animationComponent.CurrentTime = 0f;
                animationComponent.AnimationSpeed = 60f;
            }
            else
            {
            }
        }
    }
}
