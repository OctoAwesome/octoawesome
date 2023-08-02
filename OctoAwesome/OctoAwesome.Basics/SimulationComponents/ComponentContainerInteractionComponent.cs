using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Services;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with block interactions with entities.
    /// </summary>
    [SerializationId(2, 19)]
    public class ComponentContainerInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>,
        ControllableComponent,
        InventoryComponent>
    {
        private readonly Simulation simulation;
        private readonly BlockCollectionService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComponentContainerInteractionComponent"/> class.
        /// </summary>
        /// <param name="simulation">The simulation the block interactions should happen in.</param>
        /// <param name="interactionService">
        /// The interaction service to actually interact with blocks in the simulation.
        /// </param>
        public ComponentContainerInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
        {
            this.simulation = simulation;
            service = interactionService;
        }

        /// <inheritdoc />
        protected override void UpdateValue(GameTime gameTime, SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent> value)
        {
            var entity = value.Value;
            var controller = value.Component1;

            controller
                .Selection?
                .Visit(
                    blockInfo => { },
                    componentContainer => InternalUpdate(controller, entity, componentContainer)
                );
        }

        private void InternalUpdate(ControllableComponent controller, Entity entity, ComponentContainer componentContainer)
        {
        }
    }
}
