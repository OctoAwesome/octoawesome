using System.Xml.Serialization;

namespace OctoAwesome.Client.UI
{
    /// <summary>
    /// Class for common resource pack information.
    /// </summary>
    public sealed class ResourcePack
    {
        /// <summary>
        /// Gets or sets the path to the resource pack.
        /// </summary>
        [XmlIgnore]
        public string Path { get; set; }

        /// <summary>
        /// Gets or sets the resource pack name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the resource pack author.
        /// </summary>
        public string Author { get; set; }

        /// <summary>
        /// Gets or sets the resource pack description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the resource pack version number.
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the resource pack icon.
        /// </summary>
        public string Icon { get; set; }
    }
}
