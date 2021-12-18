using engenious;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;
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
        public override int Hit(IMaterialDefinition material, BlockInfo blockInfo, decimal volumeRemaining, int volumePerHit)
        {
            //TODO: Implement Place Chest and remove this item
            var position = blockInfo.Position;
            Chest chest = new(new Coordinate(0, new(position.X, position.Y, position.Z + 1), new Vector3(0.5f, 0.5f, 0.5f)));
            var notification = new FunctionalBlockNotification
            {
                Block = chest,
                Type = FunctionalBlockNotification.ActionType.Add
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
