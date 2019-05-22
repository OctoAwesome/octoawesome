using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Ein Universum von OctoAwesome. Ein Universum beinhaltet verschiedene Planeten und entspricht einem Speicherstand.
    /// </summary>
    public class Universe : IUniverse
    {
        /// <summary>
        /// ID des Universums
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Der Name des Universums
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Universe Seed
        /// </summary>
        public int Seed { get; set; }

        /// <summary>
        /// Erzeugt eine neue Instanz eines Universums
        /// </summary>
        public Universe()
        {
        }

        /// <summary>
        /// Erzeugt eine neue Instanz eines Universums
        /// </summary>
        /// <param name="id">Die GUID des Universums</param>
        /// <param name="name">Der Name des Universums</param>
        /// <param name="seed">Der Generierungsseed des Universums</param>
        public Universe(Guid id, string name, int seed)
        {
            Id = id;
            Name = name;
            Seed = seed;
        }

        /// <summary>
        /// Deserialisiert ein Universum aus dem angegebenen Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Deserialize(BinaryReader reader)
        {
            var tmpGuid = reader.ReadString();
            Id = new Guid(tmpGuid);
            Name = reader.ReadString();
            Seed = reader.ReadInt32();
        }

        /// <summary>
        /// Serialisiert das Universum in den angegebenen Stream
        /// </summary>
        /// <param name="stream"></param>
        public void Serialize(BinaryWriter writer)
        {
             writer.Write(Id.ToString());
             writer.Write(Name);
             writer.Write(Seed);
        }

 
    }
}
