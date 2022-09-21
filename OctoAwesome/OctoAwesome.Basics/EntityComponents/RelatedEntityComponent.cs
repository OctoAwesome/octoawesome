using OctoAwesome.Components;

using System;
using System.IO;

namespace OctoAwesome.Basics.EntityComponents;
internal class RelatedEntityComponent : Component, IEntityComponent
{
    public Guid RelatedEntityId { get; set; }

    public override void Serialize(BinaryWriter writer)
    {
        base.Serialize(writer);
        writer.Write(RelatedEntityId.ToString());
    }

    public override void Deserialize(BinaryReader reader)
    {
        base.Deserialize(reader);
        RelatedEntityId = Guid.Parse(reader.ReadString());
    }

}
