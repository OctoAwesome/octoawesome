﻿using engenious;
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
        private InventoryComponent inventoryComponent;
        private AnimationComponent animationComponent;
        private TransferUIComponent transferUiComponent;


        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);
            //Doesnt get called
        }

        public Chest(Coordinate position)
        {
            inventoryComponent = new InventoryComponent();
            animationComponent = new AnimationComponent();
            transferUiComponent = new TransferUIComponent(inventoryComponent);
            transferUiComponent.Closed += TransferUiComponentClosed;
            Components.AddComponent(inventoryComponent);
            Components.AddComponent(new PositionComponent()
            {
                Position = position
            });

            Components.AddComponent(new BodyComponent() { Height = 0.4f, Radius = 0.2f });
            Components.AddComponent(new BoxCollisionComponent(new[] { new BoundingBox(new Vector3(0, 0, 0), new Vector3(1, 1, 1)) }));
            Components.AddComponent(new RenderComponent() { Name = "Chest", ModelName = "chest", TextureName = "texchestmodel", BaseZRotation = -90 }, true);
            Components.AddComponent(transferUiComponent, true);
            Components.AddComponent(animationComponent);

            //Simulation.Entities.FirstOrDefault(x=>x.)
        }

        private void TransferUiComponentClosed(object sender, engenious.UI.NavigationEventArgs e)
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
