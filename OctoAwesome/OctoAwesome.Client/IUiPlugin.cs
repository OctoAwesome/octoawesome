using MonoGameUi;
using System.Collections.Generic;

namespace OctoAwesome.Client
{
    /// <summary>
    /// Basisschnittstelle für eigene Plugins. Muss implemenetiert werden, damit Plugin erkannt wird.
    /// </summary>
    public interface IUiPlugin
    {
        void MainMenuAdd(IScreenManager screenManager, ICollection<Control> controls);
    }
}
