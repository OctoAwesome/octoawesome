using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OctoAwesome
{
    public static class SettingsManager
    {
        public static string Get(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return config.AppSettings.Settings[key].Value;
        }

        public static bool KeyExists(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return config.AppSettings.Settings.AllKeys.Contains(key);
        }

        public static void Set(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            config.AppSettings.Settings[key].Value = value;
            config.Save(ConfigurationSaveMode.Full, true);
        }
    }
}
