using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;

namespace OctoAwesome.Basics.SimulationComponents
{
    [EntityFilter(typeof(ControllableComponent),typeof(InventoryComponent))]
    public class BlockInteractionComponent : SimulationComponent<ControllableComponent,InventoryComponent>
    {
        private Simulation simulation;

        public BlockInteractionComponent(Simulation simulation)
        {
            this.simulation = simulation;
        }

        protected override bool AddEntity(Entity entity)
        {
            return true;
        }

        protected override void RemoveEntity(Entity entity)
        {

        }

        protected override void UpdateEntity(GameTime gameTime, Entity entity, ControllableComponent controller, InventoryComponent inventory)
        {
            if (controller.InteractBlock.HasValue)
            {
                ushort lastBlock = entity.Cache.GetBlock(controller.InteractBlock.Value);
                entity.Cache.SetBlock(controller.InteractBlock.Value, 0);

                if (lastBlock != 0)
                {
                    var blockDefinition = simulation.ResourceManager.DefinitionManager.GetBlockDefinitionByIndex(lastBlock);

                    var slot = inventory.Inventory.FirstOrDefault(s => s.Definition == blockDefinition);

                    // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
                    if (slot == null)
                    {
                        slot = new InventorySlot()
                        {
                            Definition = blockDefinition,
                            Amount = 0
                        };
                        inventory.Inventory.Add(slot);

                        var toolbar = entity.Components.GetComponent<ToolBarComponent>();
                        if (toolbar != null)
                        {
                            for (int i = 0; i < toolbar.Tools.Length; i++)
                            {
                                if (toolbar.Tools[i] == null)
                                {
                                    toolbar.Tools[i] = slot;
                                    break;
                                }
                            }
                        }
                    }
                    slot.Amount += 125;
                }
                controller.InteractBlock = null;
            }
        }
    }
}
