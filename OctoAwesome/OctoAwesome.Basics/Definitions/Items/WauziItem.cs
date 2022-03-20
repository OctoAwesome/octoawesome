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
    public class WauziItem : Item, IDisposable
    {
        public override int VolumePerUnit => base.VolumePerUnit;

        public override int StackLimit => base.StackLimit;


        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        public WauziItem(WauziItemDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            simulationRelay = new Relay<Notification>();

            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        public override int Apply(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining)
        {
            BlockInteractionService.CalculatePositionAndRotation(hitInfo, out var index3, out var _);

            WauziEntity wauzi = new WauziEntity();

            PositionComponent position = new PositionComponent() { Position = new Coordinate(0, index3, new Vector3(0, 0, 0)) };
            wauzi.Components.AddComponent(position);

            simulationRelay.OnNext(new EntityNotification(EntityNotification.ActionType.Add, wauzi));

            return 0;
        }

        public void Dispose()
        {
            simulationSource.Dispose();
            simulationRelay?.Dispose();
        }
    }
}
