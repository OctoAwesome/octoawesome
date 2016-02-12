using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace OctoAwesome.Runtime
{
    public sealed class UniverseSerializer : IUniverseSerializer
    {
        public IUniverse Deserialize(Stream stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Universe));
            return serializer.Deserialize(stream) as IUniverse;
        }

        public void Serialize(Stream stream, IUniverse universe)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Universe));
            serializer.Serialize(stream, universe);
        }
    }
}
