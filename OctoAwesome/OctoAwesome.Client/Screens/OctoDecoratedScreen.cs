using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Screens;

namespace OctoAwesome.Client.Screens;

internal class OctoDecoratedScreen : BaseScreen
{
    public new ScreenComponent ScreenManager => (ScreenComponent)base.ScreenManager;

    public OctoDecoratedScreen(AssetComponent assets)
        : base(assets)
    {
    }
}