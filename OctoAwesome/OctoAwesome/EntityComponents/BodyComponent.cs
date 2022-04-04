using OctoAwesome.Components;
using System.IO;

namespace OctoAwesome.EntityComponents
{

    public sealed class BodyComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {

        public float Mass { get; set; }

        /// <summary>
        /// Der Radius des Spielers in Blocks.
        /// </summary>
        public float Radius { get; set; }

        /// <summary>
        /// Die Körperhöhe des Spielers in Blocks
        /// </summary>
        public float Height { get; set; }
        public BodyComponent()
        {
            Mass = 1; //1kg
            Radius = 1;
            Height = 1;
        }
        public override void Serialize(BinaryWriter writer)
        {
            base.Serialize(writer);

            writer.Write(Mass);
            writer.Write(Radius);
            writer.Write(Height);
        }
        public override void Deserialize(BinaryReader reader)
        {
            base.Deserialize(reader);

            Mass = reader.ReadSingle();
            Radius = reader.ReadSingle();
            Height = reader.ReadSingle();
        }
    }
}
