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
    public class ChestItem : Item, IDisposable
    {
        public override int VolumePerUnit => base.VolumePerUnit;

        public override int StackLimit => base.StackLimit;


        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        public ChestItem(ChestItemDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            simulationRelay = new Relay<Notification>();

            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        public override int Apply(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining)
        {
            BlockInteractionService.CalculatePositionAndRotation(hitInfo, out var index3, out var rot);

            Chest chest = new(new Coordinate(0, index3, new Vector3(0.5f, 0.5f, 0.0f)), rot);

            var notification = new FunctionalBlockNotification
            {
                Block = chest,
                Type = FunctionalBlockNotification.ActionType.Add
            };

            simulationRelay.OnNext(notification);
            return 0;
        }



        public void Dispose()
        {
            simulationSource.Dispose();
            simulationRelay?.Dispose();
        }
    }
}
