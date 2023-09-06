using OctoAwesome.Components;

using System;

namespace OctoAwesome.Basics.EntityComponents;
[Nooson]
[SerializationId()]
internal partial class RelatedEntityComponent : Component, IEntityComponent
{
    public Guid RelatedEntityId { get; set; }


}
