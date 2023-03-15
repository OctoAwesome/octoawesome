using OctoAwesome.Components;

using System;

namespace OctoAwesome.Basics.EntityComponents;
[Nooson]
internal partial class RelatedEntityComponent : Component, IEntityComponent
{
    public Guid RelatedEntityId { get; set; }


}
