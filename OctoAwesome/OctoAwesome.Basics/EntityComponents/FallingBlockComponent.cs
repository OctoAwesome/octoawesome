using OctoAwesome.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents;
public class FallingBlockComponent : Component, IEntityComponent
{
    public ushort DefinitionIndex { get; set; }
    public Guid GroupId { get; set; }

    public FallingBlockComponent()
    {
        Sendable = true;
    }
}
