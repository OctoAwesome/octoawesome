﻿using System.IO;

namespace OctoAwesome.Basics.EntityComponents
{
    public sealed class BodyPowerComponent : PowerComponent
    {
        public int JumpTime { get; set; }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);

            writer.Write(JumpTime);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            JumpTime = reader.ReadInt32();
        }
    }
}
