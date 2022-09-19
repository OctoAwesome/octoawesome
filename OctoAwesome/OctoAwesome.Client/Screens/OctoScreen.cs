using engenious.UI.Controls;
using OctoAwesome.Client.Components;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Screens;

namespace OctoAwesome.Client.Screens;

internal class OctoScreen : Screen
{
    protected readonly AssetComponent assets;
    public new ScreenComponent ScreenManager => (ScreenComponent)base.ScreenManager;

    public OctoScreen(AssetComponent assets)
    {
        this.assets = assets;
    }
}