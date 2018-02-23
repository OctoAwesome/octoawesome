using engenious;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    /// <summary>
    /// Entity movement statespace.
    /// </summary>
    public sealed class MoveableComponent : EntityComponent
    {
        /// <summary>
        /// Delta position.
        /// </summary>
        public Vector3 PositionMove { get; set; }
        /// <summary>
        /// Velocity.
        /// </summary>
        public Vector3 Velocity { get; set; }

        public Vector3 ExternalForces { get; set; }
        public Vector3 ExternalPowers { get; set; }

        // Power
        public int JumpTime { get; set; }
        public float Power { get; set; }
        public Vector3 PowerDirection { get; set; }
        public List<(float power, Vector3 direction)> Powers { get; }
        // Force
        public Vector3 Force { get; set; }
        public List<Vector3> Forces { get; }

        public MoveableComponent(Entity entity) : base(entity)
        {
            Forces = new List<Vector3>();
            Powers = new List<(float power, Vector3 direction)>();
        }
        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            writer.Write(Mass);
        }
        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            Mass = reader.ReadSingle();
        }
    }
}
