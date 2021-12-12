using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Client
{

    public class Settings : ISettings
    {
        private Configuration _config;

        public Settings()
        {
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null) // Can happen in tests
            {
                ExeConfigurationFileMap map = new ExeConfigurationFileMap { ExeConfigFilename = "EXECONFIG_PATH" };
                _config = ConfigurationManager.OpenMappedExeConfiguration(map,
                    ConfigurationUserLevel.None);
            }
            else
            {
                _config = ConfigurationManager.OpenExeConfiguration(entryAssembly.Location);
            }
        }
        

        public T? Get<T>(string key)
        {
            return Get<T>(key, default);
        }
        public T? Get<T>(string key, T? defaultValue)
        {
            var settingElement = _config.AppSettings.Settings[key];
            if (settingElement == null)
                return defaultValue;
            var valueConfig = settingElement.Value;

            return (T)Convert.ChangeType(valueConfig, typeof(T));
        }
        public T[] GetArray<T>(string key)
        {
            var valueConfig = _config.AppSettings.Settings[key].Value;

            return DeserializeArray<T>(valueConfig);
        }
        public bool KeyExists(string key)
        {
            return _config.AppSettings.Settings.AllKeys.Contains(key);
        }
        public void Set(string key, string value)
        {
            if (_config.AppSettings.Settings.AllKeys.Contains(key))
                _config.AppSettings.Settings[key].Value = value;
            else
                _config.AppSettings.Settings.Add(key, value);
            _config.Save(ConfigurationSaveMode.Modified, false);
        }
        public void Set(string key, int value)
        {
            Set(key, Convert.ToString(value));
        }
        public void Set(string key, bool value)
        {
            Set(key, Convert.ToString(value));
        }
        public void Set(string key, string[] values)
        {
            // For string array serialization we serialize a string in json array format.
            // If a string setting starts with a square bracket it is in fact an array setting.
            // [value1, value2, value3]

            string writeString = "[" + string.Join(",", values) + "]";
            Set(key, writeString);
        }
        public void Set(string key, int[] values)
        {
            string[] strValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                strValues[i] = Convert.ToString(values[i]);
            Set(key, strValues);
        }
        public void Set(string key, bool[] values)
        {
            string[] stringValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                stringValues[i] = Convert.ToString(values[i]);
            Set(key, stringValues);
        }
        private T[] DeserializeArray<T>(string arrayString)
        {
            arrayString = arrayString.Substring(1, arrayString.Length - 2);

            string[] partsString = arrayString.Split(',');
            T[] tArray = new T[partsString.Length];
            for (int i = 0; i < partsString.Length; i++)
                tArray[i] = (T)Convert.ChangeType(partsString[i], typeof(T));

            return tArray;
        }
        public void Delete(string key)
        {
            _config.AppSettings.Settings.Remove(key);
            _config.Save(ConfigurationSaveMode.Modified, false);
        }
    }
}