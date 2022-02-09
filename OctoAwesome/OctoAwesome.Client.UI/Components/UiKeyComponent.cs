using OctoAwesome.Components;
using OctoAwesome.EntityComponents;

using System;
using System.Collections.Generic;

namespace OctoAwesome.UI.Components;

public class UiKeyComponent : Component, IEntityComponent, IFunctionalBlockComponent
{
    public string PrimaryKey { get; set; }
}

