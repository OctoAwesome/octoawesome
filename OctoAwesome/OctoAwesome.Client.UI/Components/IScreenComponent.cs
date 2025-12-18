
using engenious;

using OctoAwesome.Components;
using OctoAwesome.UI.Screens;

using System;
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

        /// <summary>
        /// Associates the screen with this screen components that are contained in the component container.
        /// </summary>
        /// <param name="componentContainer">The component container that contains the ui component.</param>
        void Add(ComponentContainer componentContainer);

        /// <summary>
        /// Removes the associated entity by it's id from this components.
        /// </summary>
        /// <param name="entityId">The unique id of the entity.</param>
        void RemoveEntity(Guid entityId);

    }
}