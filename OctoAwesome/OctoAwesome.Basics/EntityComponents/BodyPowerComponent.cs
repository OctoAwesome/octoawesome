﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Basics.EntityComponents
{
    public sealed class BodyPowerComponent : PowerComponent
    {
        public int JumpTime { get; set; }


        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(JumpTime);
        }

        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            JumpTime = reader.ReadInt32();
        }
    }
}
