using engenious;

using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;

using System;

namespace OctoAwesome.Basics.Definitions.Items
{
    public class FurnaceItem : Item, IDisposable
    {
        public override int VolumePerUnit => base.VolumePerUnit;

        public override int StackLimit => base.StackLimit;


        private readonly Relay<Notification> simulationRelay;
        private readonly IDisposable simulationSource;

        public FurnaceItem(FurnaceItemDefinition definition, IMaterialDefinition materialDefinition)
            : base(definition, materialDefinition)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            simulationRelay = new Relay<Notification>();

            simulationSource = updateHub.AddSource(simulationRelay, DefaultChannels.Simulation);
        }

        public override int Hit(IMaterialDefinition material, IBlockInteraction hitInfo, decimal volumeRemaining, int volumePerHit)
        {
            //TODO: Implement Place Chest and remove this item
            var position = hitInfo.Position;
            Furnace chest = new(new Coordinate(0, new(position.X, position.Y, position.Z + 1), new Vector3(0.5f, 0.5f, 0.5f)));
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
