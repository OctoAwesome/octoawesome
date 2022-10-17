using engenious;

using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;
using OctoAwesome.Services;

using System;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Chest item for inventories.
    /// </summary>
    public class ChestItem : Item, IDisposable
    {
        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChestItem"/> class.
        /// </summary>
        /// <param name="definition">The chest item definition.</param>
        /// <param name="materialDefinition">The material definition the chest is made out of.</param>
        public ChestItem(ChestItemDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            simulationRelay = new Relay<Notification>();

            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        /// <inheritdoc />
        public override int Apply(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining)
        {
            BlockInteractionService.CalculatePositionAndRotation(hitInfo, out var index3, out var rot);

            Chest chest = new(new Coordinate(0, index3, new Vector3(0.5f, 0.5f, 0.0f)), rot);

            var notification = new FunctionalBlockNotification
            {
                Entity = chest,
                Type = EntityNotification.ActionType.Add
            };

            simulationRelay.OnNext(notification);
            return 0;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            simulationSource.Dispose();
            simulationRelay.Dispose();
        }
    }
}
