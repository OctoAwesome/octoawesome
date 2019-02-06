using engenious;
using OctoAwesome.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.EntityComponents
{
    public sealed class PositionComponent : EntityComponent
    {
        public Coordinate Position { get => position; set => SetValue(ref position, value); }

        public float Direction { get; set; }

        private Coordinate position;

        public PositionComponent()
        {
            Sendable = true;
        }

        public override void Serialize(BinaryWriter writer, IDefinitionManager definitionManager)
        {
            base.Serialize(writer, definitionManager);
            // Position
            writer.Write(Position.Planet);
            writer.Write(Position.GlobalBlockIndex.X);
            writer.Write(Position.GlobalBlockIndex.Y);
            writer.Write(Position.GlobalBlockIndex.Z);
            writer.Write(Position.BlockPosition.X);
            writer.Write(Position.BlockPosition.Y);
            writer.Write(Position.BlockPosition.Z);
            writer.Write(Position.ChunkIndex.X);
            writer.Write(Position.ChunkIndex.Y);
            writer.Write(Position.ChunkIndex.Z);
        }

        public override void Deserialize(BinaryReader reader, IDefinitionManager definitionManager)
        {
            base.Deserialize(reader, definitionManager);

            // Position
            int planet = reader.ReadInt32();
            int blockX = reader.ReadInt32();
            int blockY = reader.ReadInt32();
            int blockZ = reader.ReadInt32();
            float posX = reader.ReadSingle();
            float posY = reader.ReadSingle();
            float posZ = reader.ReadSingle();
            int chunkIndexX = reader.ReadInt32();
            int chunkIndexY = reader.ReadInt32();
            int chunkIndexZ = reader.ReadInt32();

            position = new Coordinate(planet, new Index3(blockX, blockY, blockZ), new Vector3(posX, posY, posZ));
            //Position.ChunkIndex = new Index3(chunkIndexX, chunkIndexY, chunkIndexX);
        }

        protected override void OnPropertyChanged<T>(T value, string callerName)
        {
            base.OnPropertyChanged(value, callerName);

            if(callerName == nameof(Position))
            {
                var updateNotification = new PropertyChangedNotification
                {
                    Issuer = nameof(PositionComponent),
                    Property = callerName
                };

                using(var stream = new MemoryStream())
                using (var writer = new BinaryWriter(stream))
                {
                    Serialize(writer, null);
                    updateNotification.Value = stream.ToArray();
                }

                Update(updateNotification);
            }
        }

        public override void OnUpdate(SerializableNotification notification)
        {
            base.OnUpdate(notification);

            if(notification is PropertyChangedNotification changedNotification)
            {
                if(changedNotification.Issuer == nameof(PositionComponent))
                {
                    if(changedNotification.Property == nameof(Position))
                    {
                        using(var stream = new MemoryStream(changedNotification.Value))
                        using(var reader = new BinaryReader(stream))
                        {
                            Deserialize(reader, null);
                        }
                    }
                }
            }
        }
    }
}
