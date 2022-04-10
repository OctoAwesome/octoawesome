namespace OctoAwesome.Client.UI.Components
{
    /// <summary>
    /// Interface for all components, which work with assets from the asset manager(<see cref="AssetComponent"/>).
    /// </summary>
    public interface IAssetRelatedComponent
    {
        /// <summary>
        /// Unloads all assets.
        /// </summary>
        void UnloadAssets();

        /// <summary>
        /// Reloads all assets.
        /// </summary>
        void ReloadAssets();
    }
}
