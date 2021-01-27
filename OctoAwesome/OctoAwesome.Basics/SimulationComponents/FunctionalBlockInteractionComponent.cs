using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using engenious;
using OctoAwesome.Services;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Definitions;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class FunctionalBlockInteractionComponent : SimulationComponent<
        FunctionalBlock, 
        SimulationComponentRecord<FunctionalBlock, InventoryComponent>, 
        InventoryComponent>
    {
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<FunctionalBlock, InventoryComponent> value)
        {
            
        }
    }
}
