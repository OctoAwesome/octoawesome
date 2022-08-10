
using engenious;
using OctoAwesome.UI.Screens;
using System.Collections.Generic;

namespace OctoAwesome.UI.Components
{
    /// <summary>
    /// Base interface for screen components
    /// </summary>
    public interface IScreenComponent
    {
        /// <summary>
        /// Gets the list of components that belong to this screen component
        /// </summary>
        ComponentList<UIComponent> Components { get; }

        /// <summary>
        /// Closes the screen that belongs to this screen component
        /// </summary>
        void Exit();

        /// <summary>
        /// Reloads all assets for this screen
        /// </summary>
        void ReloadAssets();

        /// <summary>
        /// Unloads all the assets that belong to this screen
        /// </summary>
        void UnloadAssets();

        /// <summary>
        /// Associates the screen with this screen component
        /// </summary>
        /// <param name="screen">The screen that should be associated with the component</param>
        void Add(BaseScreen screen);

        /// <summary>
        /// Removes the association of the screen with this screen component
        /// </summary>
        /// <param name="screen">The screen that should be removed from the component</param>
        void Remove(BaseScreen screen);
    }
}