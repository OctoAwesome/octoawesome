using OctoAwesome.Components;
using OctoAwesome.EntityComponents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents;

public class RelevantPositionsComponent : Component, IEntityComponent
{
    public List<PositionComponent> Positions { get; set; }

    public RelevantPositionsComponent()
    {
        Sendable = false;
    }
}