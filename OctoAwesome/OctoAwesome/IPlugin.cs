using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome
{
    /// <summary>
    /// Basisschnittstelle für eigene Plugins. Muss implemenetiert werden, damit Plugin erkannt wird.
    /// </summary>
    public interface IPlugin
    {
        /// <summary>
        /// Wird einmal nach dem Laden des Plugins ausgeführt.
        /// </summary>
        /// <param name="manager">Der <see cref="ActionManager"/>, der verwendet werden kann, um Aktionen im Plugin zu registrieren</param>
        void OnLoaded(ActionManager manager);
    }
}
