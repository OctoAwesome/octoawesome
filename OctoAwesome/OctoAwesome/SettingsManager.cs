using System.Configuration;
using System.Linq;
using System.Reflection;

namespace OctoAwesome
{
    /// <summary>
    /// Verwaltet die Anwendungseinstellungen.
    /// TODO: In den Client verschieben
    /// </summary>
    public static class SettingsManager
    {
        /// <summary>
        /// Gibt den Wert einer Einstellung zurück.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns>Der Wert der Einstellung.</returns>
        public static string Get(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return config.AppSettings.Settings[key].Value;
        }

        /// <summary>
        /// Überprüft, ob die angegebene Einstellung existeiert.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns></returns>
        public static bool KeyExists(string key)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            return config.AppSettings.Settings.AllKeys.Contains(key);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        public static void Set(string key, string value)
        {
            var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);
            if (config.AppSettings.Settings.AllKeys.Contains(key))
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified, false);
        }
    }
}
