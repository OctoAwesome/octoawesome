using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace OctoAwesome.Client
{
    /// <summary>
    /// Manages the application settings.
    /// </summary>
    public class Settings : ISettings
    {
        private Configuration _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="Settings"/> class,
        /// which loads the configuration file for the currently running application.
        /// </summary>
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

        /// <inheritdoc/>
        public T Get<T>(string key)
        {
            if (TryGet<T>(key, out var value))
                return value;
            throw new KeyNotFoundException();
        }

        /// <inheritdoc/>
        public T? Get<T>(string key, T? defaultValue)
        {
            return TryGet<T>(key, out var value) ? value : defaultValue;
        }

        /// <inheritdoc/>
        public T[] GetArray<T>(string key)
        {
            if (TryGetArray<T>(key, out var values))
                return values;
            throw new KeyNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            var settingElement = _config.AppSettings.Settings[key];
            if (settingElement is null)
            {
                value = default;
                return false;
            }

            var valueConfig = settingElement.Value;

            value = (T)Convert.ChangeType(valueConfig, typeof(T));
            return true;
        }

        /// <inheritdoc />
        public bool TryGetArray<T>(string key, [MaybeNullWhen(false)] out T[] values)
        {
            var valueConfig = _config.AppSettings.Settings[key]?.Value;
            if (valueConfig is not null)
            {
                values = DeserializeArray<T>(valueConfig);
                return true;
            }

            values = default;
            return false;
        }

        /// <inheritdoc/>
        public bool KeyExists(string key)
        {
            return _config.AppSettings.Settings.AllKeys.Contains(key);
        }

        /// <inheritdoc/>
        public void Set(string key, string value)
        {
            if (_config.AppSettings.Settings.AllKeys.Contains(key))
                _config.AppSettings.Settings[key].Value = value;
            else
                _config.AppSettings.Settings.Add(key, value);
            _config.Save(ConfigurationSaveMode.Modified, false);
        }

        /// <inheritdoc/>
        public void Set(string key, int value)
        {
            Set(key, Convert.ToString(value));
        }

        /// <inheritdoc/>
        public void Set(string key, bool value)
        {
            Set(key, Convert.ToString(value));
        }

        /// <inheritdoc/>
        public void Set(string key, string[] values)
        {
            // For string array serialization we serialize a string in json array format.
            // If a string setting starts with a square bracket it is in fact an array setting.
            // [value1, value2, value3]

            string writeString = "[" + string.Join(",", values) + "]";
            Set(key, writeString);
        }

        /// <inheritdoc/>
        public void Set(string key, int[] values)
        {
            string[] strValues = new string[values.Length];
            for (int i = 0; i < values.Length; i++)
                strValues[i] = Convert.ToString(values[i]);
            Set(key, strValues);
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Delete(string key)
        {
            _config.AppSettings.Settings.Remove(key);
            _config.Save(ConfigurationSaveMode.Modified, false);
        }
    }
}
