using engenious;
using OctoAwesome.EntityComponents;
using OctoAwesome.Network;
using OctoAwesome.Notifications;
using OctoAwesome.Pooling;
using OctoAwesome.Rx;
using OctoAwesome.Serialization;
using System;
using System.Text;

namespace OctoAwesome.GameServer.Commands
{
    /// <summary>
    /// Contains remote player commands.
    /// </summary>
    public static class PlayerCommands
    {
        private static readonly ConcurrentRelay<Notification> simulationChannel;
        private static readonly ConcurrentRelay<Notification> networkChannel;
        private static readonly IDisposable simulationChannelSub;
        private static readonly IDisposable networkChannelSub;

        static PlayerCommands()
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();

            simulationChannel = new ConcurrentRelay<Notification>();
            networkChannel = new ConcurrentRelay<Notification>();

            simulationChannelSub = updateHub.AddSource(simulationChannel, DefaultChannels.Simulation);
            networkChannelSub = updateHub.AddSource(networkChannel, DefaultChannels.Network);
        }

        /// <summary>
        /// Manifests player received from <see cref="CommandParameter"/>.
        /// </summary>
        /// <param name="parameter">The <see cref="CommandParameter"/> containing the player data.</param>
        /// <returns><c>null</c></returns>
        public static byte[] Whoami(CommandParameter parameter)
        {
            var updateHub = TypeContainer.Get<IUpdateHub>();
            string playername = Encoding.UTF8.GetString(parameter.Data);
            var player = new Player();
            var entityNotificationPool = TypeContainer.Get<IPool<EntityNotification>>();
            var entityNotification = entityNotificationPool.Rent();
            entityNotification.Entity = player;
            entityNotification.Type = EntityNotification.ActionType.Add;

            simulationChannel.OnNext(entityNotification);
            entityNotification.Release();


            var remotePlayer = new RemoteEntity(player);
            remotePlayer.Components.AddIfTypeNotExists(new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 78), new Vector3(0, 0, 0)) });
            remotePlayer.Components.AddIfNotExists(new RenderComponent() { Name = "Wauzi", ModelName = "dog", TextureName = "texdog", BaseZRotation = -90 });
            remotePlayer.Components.AddIfTypeNotExists(new BodyComponent() { Mass = 50f, Height = 2f, Radius = 1.5f });

            Console.WriteLine(playername);
            entityNotification = entityNotificationPool.Rent();
            entityNotification.Entity = remotePlayer;
            entityNotification.Type = EntityNotification.ActionType.Add;

            networkChannel.OnNext(entityNotification);
            entityNotification.Release();
            return Serializer.Serialize(player);
        }
    }
}
