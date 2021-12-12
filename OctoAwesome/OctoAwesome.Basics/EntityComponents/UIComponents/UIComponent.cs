using engenious.UI;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.Components;

namespace OctoAwesome.Basics.EntityComponents.UIComponents
{
    public abstract class UIComponent : Component, IEntityComponent, IFunctionalBlockComponent
    {
        protected BaseScreenComponent ScreenComponent { get; }
        public AssetComponent AssetComponent { get; }

        protected UIComponent()
        {
            ScreenComponent = TypeContainer.Get<BaseScreenComponent>();
            AssetComponent = TypeContainer.Get<AssetComponent>();
        }

    }
}
