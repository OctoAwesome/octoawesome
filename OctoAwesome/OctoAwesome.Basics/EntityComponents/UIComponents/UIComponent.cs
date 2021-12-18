using engenious.UI;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    /// <summary>
    /// Base component for ui interactions.
    /// </summary>
    public abstract class UIComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        /// <summary>
        /// Gets the screen component for interacting with UI elements.
        /// </summary>
        protected BaseScreenComponent ScreenComponent { get; }

        /// <summary>
        /// Gets the asset component for loading asset resources.
        /// </summary>
        public AssetComponent AssetComponent { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UIComponent"/> class.
        /// </summary>
        protected UIComponent()
        {
            ScreenComponent = TypeContainer.Get<BaseScreenComponent>();
            AssetComponent = TypeContainer.Get<AssetComponent>();
        }

    }
}
