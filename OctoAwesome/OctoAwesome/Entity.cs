using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using System;
using System.IO;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for all entities.
    /// </summary>
    public abstract class Entity : ComponentContainer<IEntityComponent>
    {

    }
}
