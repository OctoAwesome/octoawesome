namespace OctoAwesome.Caching
{
    /// <summary>
    /// Cache loading mode.
    /// </summary>
    public enum LoadingMode
    {
        /// <summary>
        /// Load if item does not exist in cache.
        /// </summary>
        LoadIfNotExists,
        /// <summary>
        /// Only return items when in cache.
        /// </summary>
        OnlyCached
    }
}
