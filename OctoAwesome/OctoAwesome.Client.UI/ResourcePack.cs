using System.Xml.Serialization;

namespace OctoAwesome.Client.UI
{

    public sealed class ResourcePack
    {

        [XmlIgnore]
        public string Path { get; set; }
        public string Name { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Icon { get; set; }
    }
}
