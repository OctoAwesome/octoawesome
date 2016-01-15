using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Runtime
{
    public static class PluginManager
    {
        private static List<IPlugin> plugins;

        public static IEnumerable<IPlugin> LoadPlugins()
        {
            if (plugins == null)
            {
                plugins = new List<IPlugin>();
                plugins.AddRange(ExtensionManager.GetInstances<IPlugin>());

                foreach (var plugin in plugins)
                    plugin.OnLoaded();
            }

            return plugins;
        }
    }
}
