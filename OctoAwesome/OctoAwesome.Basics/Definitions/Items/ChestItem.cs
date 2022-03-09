using engenious;

using OctoAwesome.Basics.FunctionBlocks;
using OctoAwesome.Definitions;
using OctoAwesome.Definitions.Items;
using OctoAwesome.Notifications;
using OctoAwesome.Rx;

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
            var position = hitInfo.Position;
            var index3 = hitInfo.SelectedSide switch
            {
                OrientationFlags.SideWest => new Index3(position.X - 1, position.Y, position.Z),
                OrientationFlags.SideEast => new(position.X + 1, position.Y, position.Z),
                OrientationFlags.SideSouth => new(position.X, position.Y - 1, position.Z),
                OrientationFlags.SideNorth => new(position.X, position.Y + 1, position.Z),
                OrientationFlags.SideBottom => new(position.X, position.Y, position.Z - 1),
                OrientationFlags.SideTop => new(position.X, position.Y, position.Z + 1),
                _ => new(position.X, position.Y, position.Z + 1),
            };

            float rot;
            var change = position - index3;
            if (change.X > 0)
                rot = 0f;
            else if (change.X < 0)
                rot = (float)Math.PI;
            else if (change.Y < 0)
                rot = (float)Math.PI / 2 * 3;
            else if (change.Y > 0)
                rot = (float)Math.PI / 2;
            else
                rot = 0f;

            Chest chest = new(new Coordinate(0, index3, new Vector3(0.5f, 0.5f, 0.5f)), rot);

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
