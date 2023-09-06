using OctoAwesome.Components;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents;
[Nooson, SerializationId()]
public partial class InteractKeyComponent : Component, IEntityComponent
{
    public string Key { get; set; }

    public InteractKeyComponent() : base()
    {
        Sendable = true;
    }
}