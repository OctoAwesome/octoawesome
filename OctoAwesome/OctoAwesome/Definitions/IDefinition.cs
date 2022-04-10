namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base Interface for all definitions.
    /// </summary>
    public interface IDefinition
    {
        /// <summary>
        /// Gets the name of the definition.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name of the icon resource.
        /// </summary>
        string Icon { get; }
    }
}
