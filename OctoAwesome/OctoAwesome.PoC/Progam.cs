﻿using OctoAwesome.Caching;
using OctoAwesome.Notifications;
using OctoAwesome.PoC.Rx;
using OctoAwesome.Runtime;
using OpenTK.Graphics.ES11;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace OctoAwesome.PoC
{
    public static class Program
    {
        static void Main()
        {

            object a = 12;

            var i 
                = GenericCaster<int, object>
                .Cast(a);

            using var network = new Relay<Notification>();
            using var hub = new Rx.UpdateHub();

            Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(TimeSpan.FromSeconds(3));
                    network.OnNext(new BlockChangedNotification());
                }
            });

            using var sub = hub.AddSource(network, DefaultChannels.Network);

            using var sub2 = hub.ListenOn(DefaultChannels.Network).Subscribe(n => Console.WriteLine(n is not null));
            using var sub4 = hub.ListenOn(DefaultChannels.Network).Subscribe(n => throw new ArgumentException());
            using var sub3 = hub.ListenOn(DefaultChannels.Network).Subscribe(n => Console.WriteLine(n.SenderId));

            Console.ReadLine();
        }
    }
}
