using engenious;

using OctoAwesome.Basics.Entities;
using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Chunking;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.EntityComponents;
using OctoAwesome.Information;
using OctoAwesome.Location;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;
using OctoAwesome.Services;

using System;

namespace OctoAwesome.Basics.Definitions.Items
{
    /// <summary>
    /// Wauzi egg item.
    /// </summary>
    public class WauziItem : Item, IDisposable
    {
        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        /// <summary>
        /// Initializes a new instance of the <see cref="WauziItem"/> class.
        /// </summary>
        /// <param name="definition">The wauzi item egg definition.</param>
        /// <param name="materialDefinition">The material the egg is made of.</param>
        public WauziItem(WauziItemDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            simulationRelay = new Relay<Notification>();

            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        /// <inheritdoc />
        public override int Interact(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining)
        {
            BlockInteractionService.CalculatePositionAndRotation(hitInfo, out var facingDirection, out var _);

            WauziEntity wauzi = new WauziEntity();

            PositionComponent position = new PositionComponent() { Position = new Coordinate(0, facingDirection, new Vector3(0, 0, 0)) };
            wauzi.Components.Add(position);

            simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Add, wauzi));

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
