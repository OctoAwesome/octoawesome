using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

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
        public static T Get<T>(string key) {


            
            // var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

            var config = Configuration;

            var valueConfig = config.AppSettings.Settings[key].Value;

            return (T) Convert.ChangeType(valueConfig, typeof(T));


        }
        

        /// <summary>
        /// Überprüft, ob die angegebene Einstellung existeiert.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns></returns>
        public static bool KeyExists(string key)
        {
            // var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);

            var config = Configuration;

            return config.AppSettings.Settings.AllKeys.Contains(key);
        }

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>

        public static void Set(string key, string value)
        {


            

            var config = Configuration;


            if (config.AppSettings.Settings.AllKeys.Contains(key))
                config.AppSettings.Settings[key].Value = value;
            else
                config.AppSettings.Settings.Add(key, value);
            config.Save(ConfigurationSaveMode.Modified, false);
        }

        public static void Set(string key, int value)
        {
            Set(key, Convert.ToString(value));
        }

        public static void Set(string key, bool value)
        {
            Set(key, Convert.ToString(value));
        }

        private static Configuration Configuration
        {
            get
            {
                //StackOverflow: http://stackoverflow.com/questions/1083927/configurationmanager-openexeconfiguration-loads-the-wrong-file

                // ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "EXECONFIG_PATH" };
                // Configuration config = ConfigurationManager.OpenMappedExeConfiguration(map, ConfigurationUserLevel.None);

                var config = ConfigurationManager.OpenExeConfiguration(Assembly.GetEntryAssembly().Location);


                Console.WriteLine(config.FilePath);
                return config;
            }
        }
    }
}
