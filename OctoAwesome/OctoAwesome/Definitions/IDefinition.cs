namespace OctoAwesome.Definitions
{
    /// <summary>
    /// Base Interface for all Definitions
    /// </summary>
    public interface IDefinition
    {
        /// <summary>
        /// Gets the name of the Definition.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the name of the Icon Resource.
        /// </summary>
        string Icon { get; }
    }
}
