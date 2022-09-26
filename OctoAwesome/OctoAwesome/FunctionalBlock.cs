using engenious;

using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using OctoAwesome.Notifications;
using OctoAwesome.Serialization;

using OpenTK.Audio.OpenAL;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome
{
    /// <summary>
    /// Base class for functional blocks.
    /// </summary>
    public abstract class FunctionalBlock : ComponentContainer<IFunctionalBlockComponent>
    {

    }
}
