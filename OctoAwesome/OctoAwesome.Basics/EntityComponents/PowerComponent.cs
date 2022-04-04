using engenious;
using System.IO;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents
{

    public abstract class PowerComponent : Component, IEntityComponent
    {

        public float Power { get; set; }
        public Vector3 Direction { get; set; }

        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Power);
        }
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Power = reader.ReadSingle();
        }
    }
}
