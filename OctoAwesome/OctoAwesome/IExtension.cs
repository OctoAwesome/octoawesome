namespace OctoAwesome
{
    /// <summary>
    /// Interface for all mod plugin extensions.
    /// </summary>
    public interface IExtension
    {
        /// <summary>
        /// Gets the name of the extension.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description of the extension.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Register the components in the extension loader.
        /// </summary>
        /// <param name="extensionLoader">The extension loader the extension was registered into.</param>
        /// <param name="typeContainer">The type container to get types with.</param>
        void Register(IExtensionLoader extensionLoader, ITypeContainer typeContainer);

        /// <summary>
        /// Registers type in the given type container for this extension.
        /// </summary>
        /// <param name="typeContainer">The type container to register the types in.</param>
        void Register(ITypeContainer typeContainer);
    }
}
