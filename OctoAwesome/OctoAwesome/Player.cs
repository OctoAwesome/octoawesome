using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Xml.Serialization;
using System;
using System.IO;

namespace OctoAwesome
{
    public sealed class Player : Entity
    {
        public const int SELECTIONRANGE = 8;

        public const float POWER = 600f;

        public const float JUMPPOWER = 400000f;

        public const float FRICTION = 60f;

        public float Radius { get; set; }

        // TODO: Angle immer hübsch kürzen
        public float Angle { get; set; }

        public float Height { get; set; }

        [XmlIgnore]
        public bool OnGround { get; set; }

        public float Tilt { get; set; }

        public int InventorySlots { get; set; }

        public bool FlyMode { get; set; }

        [XmlIgnore]
        public List<InventorySlot> Inventory { get; set; }

        public Player()
        {
            Position = new Coordinate(0, new Index3(0, 0, 100), Vector3.Zero);
            Velocity = new Vector3(0, 0, 0);
            Inventory = new List<InventorySlot>();
            Radius = 0.75f;
            Angle = 0f;
            Height = 3.5f;
            Mass = 100;
            FlyMode = false;
        }

        public override void SetData(byte[] data)
        {
            using (MemoryStream stream = new MemoryStream(data))
            {
                BinaryReader br = new BinaryReader(stream);
                Position = new Coordinate(
                    br.ReadInt32(),
                    new Index3(br.ReadInt32(), br.ReadInt32(), br.ReadInt32()),
                    new Vector3(br.ReadSingle(), br.ReadSingle(), br.ReadSingle()));
                Radius = br.ReadSingle();
                Angle = br.ReadSingle();
                Height = br.ReadSingle();
            }
        }

        public override byte[] GetData()
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryWriter bw = new BinaryWriter(stream);
                bw.Write(Position.Planet);
                bw.Write(Position.GlobalBlockIndex.X);
                bw.Write(Position.GlobalBlockIndex.Y);
                bw.Write(Position.GlobalBlockIndex.Z);
                bw.Write(Position.BlockPosition.X);
                bw.Write(Position.BlockPosition.Y);
                bw.Write(Position.BlockPosition.Z);
                bw.Write(Radius);
                bw.Write(Angle);
                bw.Write(Height);

                byte[] buffer = new byte[stream.Length];
                stream.Seek(0, SeekOrigin.Begin);
                stream.Read(buffer, 0, buffer.Length);

                return buffer;
            }
        }
    }
}