﻿using OctoAwesome.EntityComponents;
using engenious;
using OctoAwesome.Services;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.SimulationComponents
{
    /// <summary>
    /// Component for simulation with block interactions with functional blocks.
    /// </summary>
    public class FunctionalBlockInteractionComponent : SimulationComponent<
        Entity,
        SimulationComponentRecord<Entity, ControllableComponent, InventoryComponent>,
        ControllableComponent,
        InventoryComponent>
    {
        private readonly Simulation simulation;
        private readonly BlockCollectionService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionalBlockInteractionComponent"/> class.
        /// </summary>
        /// <param name="simulation">The simulation the block interactions should happen in.</param>
        /// <param name="interactionService">
        /// The interaction service to actually interact with blocks in the simulation.
        /// </param>
        public FunctionalBlockInteractionComponent(Simulation simulation, BlockCollectionService interactionService)
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
                    functionalBlock => InternalUpdate(controller, entity, functionalBlock),
                    entity => { }
                );
        }

        private void InternalUpdate(ControllableComponent controller, Entity entity, FunctionalBlock functionalBlock)
        {
        }
    }
}
