namespace OctoAwesome
{
    /// <summary>
    /// Interface for all Mod Plugin Extensions.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets the Extension Name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the Extension Description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Register the Components in the ExtensionsLoader
        /// </summary>
        /// <param name="extensionLoader">ExtensionsLoader</param>
        void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer);

        void Register(ITypeContainer typeContainer);
    }
}
