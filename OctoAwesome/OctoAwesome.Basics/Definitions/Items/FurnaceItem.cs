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
    /// Class for furnace items in inventory
    /// </summary>
    public class FurnaceItem : Item, IDisposable
    {
        /// <inheritdoc/>
        public override int VolumePerUnit => base.VolumePerUnit;

        /// <inheritdoc/>
        public override int StackLimit => base.StackLimit;


        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        /// <summary>
        /// Initializes a new instance of the<see cref="FurnaceItem" /> class
        /// </summary>
        public FurnaceItem(FurnaceItemDefinition definition, IMaterialDefinition materialDefinition)
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

            Furnace furnace = new(new Coordinate(0, index3, new Vector3(0.5f, 0.5f, 0f)), rot);
            var notification = new EntityNotification
            {
                Entity = furnace,
                Type = EntityNotification.ActionType.Add
            };

            simulationRelay.OnNext(notification);
            return 0;
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            simulationSource.Dispose();
            simulationRelay.Dispose();
        }
    }
}
