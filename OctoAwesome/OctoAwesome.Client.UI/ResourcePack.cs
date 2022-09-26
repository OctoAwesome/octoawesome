using System;
using System.Xml.Serialization;
using OctoAwesome.Extension;

namespace OctoAwesome.Client.UI
{
    /// <summary>
    /// Class for common resource pack information.
    /// </summary>
    public sealed class ResourcePack
    {
        private string? path, name, author, description, version, icon;

        /// <summary>
        /// Gets or sets the path to the resource pack.
        /// </summary>
        [XmlIgnore]
        public string Path
        {
            get => NullabilityHelper.NotNullAssert(path, $"{nameof(Path)} was not initialized!");
            set => path = NullabilityHelper.NotNullAssert(value, $"{nameof(Path)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the resource pack name.
        /// </summary>
        public string Name
        {
            get => NullabilityHelper.NotNullAssert(name, $"{nameof(Name)} was not initialized!");
            set => name = NullabilityHelper.NotNullAssert(value, $"{nameof(Name)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the resource pack author.
        /// </summary>
        public string Author
        {
            get => NullabilityHelper.NotNullAssert(author, $"{nameof(Author)} was not initialized!");
            set => author = NullabilityHelper.NotNullAssert(value, $"{nameof(Author)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the resource pack description.
        /// </summary>
        public string Description
        {
            get => NullabilityHelper.NotNullAssert(description, $"{nameof(Description)} was not initialized!");
            set => description = NullabilityHelper.NotNullAssert(value, $"{nameof(Description)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the resource pack version number.
        /// </summary>
        public string Version
        {
            get => NullabilityHelper.NotNullAssert(version, $"{nameof(Version)} was not initialized!");
            set => version = NullabilityHelper.NotNullAssert(value, $"{nameof(Version)} cannot be initialized with null!");
        }

        /// <summary>
        /// Gets or sets the resource pack icon.
        /// </summary>
        public string Icon
        {
            get => NullabilityHelper.NotNullAssert(icon, $"{nameof(Icon)} was not initialized!");
            set => icon = NullabilityHelper.NotNullAssert(value, $"{nameof(Icon)} cannot be initialized with null!");
        }
    }
}
