﻿using System;

namespace OctoAwesome.Notifications
{
    /// <summary>
    /// Interface for update hub implementation for managing observers and observables.
    /// </summary>
    public interface IUpdateHub
    {
        /// <summary>
        /// Subscribes to the given observable on the specified channel.
        /// </summary>
        /// <param name="notification">The observable to subscribe on.</param>
        /// <param name="channel">The observing channel.</param>
        /// <returns>
        /// A reference to an interface that allows observers to stop receiving notifications
        /// before the provider has finished sending them.
        /// </returns>
        IDisposable AddSource(IObservable<Notification> notification, string channel);

        /// <summary>
        /// Gets an observable to listen for notifications on the specified channel.
        /// </summary>
        /// <param name="channel">The channel to listen on for notifications.</param>
        /// <returns>An observable to listen for notifications on the specified channel.</returns>
        IObservable<Notification> ListenOn(string channel);
    }
}
