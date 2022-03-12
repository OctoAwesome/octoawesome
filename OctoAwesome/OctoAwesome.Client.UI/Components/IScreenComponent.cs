
using engenious;

using OctoAwesome.Components;
using OctoAwesome.UI.Screens;

using System.Collections.Generic;

namespace OctoAwesome.UI.Components
{
    public interface IScreenComponent
    {
        ComponentList<UIComponent> Components { get; }

        void Exit();
        void ReloadAssets();
        void UnloadAssets();

        void Add(BaseScreen screen);
        void Remove(BaseScreen screen);
    }
}