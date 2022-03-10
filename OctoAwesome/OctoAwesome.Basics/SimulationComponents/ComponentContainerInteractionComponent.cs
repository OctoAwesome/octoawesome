using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.SimulationComponents
{
    public class ComponentContainerInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>,
        ControllableComponent,
        InventoryComponent>
    {
        private readonly Simulation simulation;
        private readonly BlockInteractionService service;

        public ComponentContainerInteractionComponent(Simulation simulation, BlockInteractionService interactionService)
        {
            this.simulation = simulation;
            service = interactionService;
        }

        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;

            controller
                .Selection?
                .Visit(
                    hitInfo => { },
                    applyInfo => { },
                    componentContainer => InternalUpdate(controller, entity, componentContainer)
                );
        }

        private void InternalUpdate(ControllableComponent controller, Entity entity, ComponentContainer componentContainer)
        {
        }
    }
}
