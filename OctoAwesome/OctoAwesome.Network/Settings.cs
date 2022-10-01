﻿//using OpenTK;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;

namespace OctoAwesome.Network
{
    /// <summary>
    /// A class for managing settings.
    /// </summary>
    public class Settings : ISettings
    {
        /// <summary>
        /// Gets the file info of the file the settings are loaded from and saved to.
        /// </summary>
        public FileInfo FileInfo { get; init; }
        private readonly Dictionary<string, string> dictionary;

        /// <summary>
        /// Initializes a new <see cref="Settings"/> class.
        /// </summary>
        /// <param name="fileInfo">The file to load the settings from.</param>
        public Settings(FileInfo fileInfo)
        {
            FileInfo = fileInfo;
            dictionary = InternalLoad(fileInfo);
        }
        /// <summary>
        /// Initializes a new <see cref="Settings"/> class using some default settings.
        /// </summary>
        public Settings()
        {
            dictionary = new Dictionary<string, string>()
            {
                ["ChunkRoot"] = "ServerMap",
                ["Viewrange"] = "4",
                ["DisablePersistence"] = "false",
                ["LastUniverse"] = ""
            };
            FileInfo = null!;
        }

        /// <inheritdoc />
        public void Delete(string key)
            => dictionary.Remove(key);

        /// <inheritdoc />
        public T Get<T>(string key)
        {
            if (TryGet<T>(key, out var value))
                return value;
            throw new KeyNotFoundException();
        }

        /// <inheritdoc />
        public T? Get<T>(string key, T? defaultValue)
        {
            return TryGet<T>(key, out var value) ? value : defaultValue;
        }

        /// <inheritdoc />
        public T[] GetArray<T>(string key)
        {
            if (TryGetArray<T>(key, out var values))
                return values;
            throw new KeyNotFoundException();
        }

        /// <inheritdoc />
        public bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value)
        {
            if (dictionary.TryGetValue(key, out var val))
            {
                value = (T)Convert.ChangeType(val, typeof(T));
                return true;
            }

            value = default;
            return false;
        }

        /// <inheritdoc />
        public bool TryGetArray<T>(string key, [MaybeNullWhen(false)] out T[] values)
        {
            if (dictionary.TryGetValue(key, out var val))
            {
                values = DeserializeArray<T>(val);
                return true;
            }

            values = default;
            return false;
        }

        /// <inheritdoc />
        public bool KeyExists(string key)
            => dictionary.ContainsKey(key);

        /// <inheritdoc />
        public void Set(string key, string value)
        {
            if (dictionary.ContainsKey(key))
                dictionary[key] = value;
            else
                dictionary.Add(key, value);

            Save();
        }

        /// <inheritdoc />
        public void Set(string key, int value)
            => Set(key, value.ToString());

        /// <inheritdoc />
        public void Set(string key, bool value)
            => Set(key, value.ToString());

        /// <inheritdoc />
        public void Set(string key, string[] values)
            => Set(key, "[" + string.Join(",", values) + "]");

        /// <inheritdoc />
        public void Set(string key, int[] values)
            => Set(key, values.Select(i => i.ToString()).ToArray());

        /// <inheritdoc />
        public void Set(string key, bool[] values)
            => Set(key, values.Select(b => b.ToString()).ToArray());

        /// <summary>
        /// Load the settings from disk.
        /// </summary>
        public void Load()
        {
            if (FileInfo == null)
                throw new ArgumentException("No file info was specified");

            dictionary.Clear();

            foreach (var entry in InternalLoad(FileInfo))
                dictionary.Add(entry.Key, entry.Value);
        }

        /// <summary>
        /// Save the settings to disk.
        /// </summary>
        public void Save()
        {
            if (FileInfo == null)
                throw new ArgumentException("No file info was specified");

            FileInfo.Delete();
            using (var writer = new StreamWriter(FileInfo.OpenWrite()))
            {
                writer.Write(System.Text.Json.JsonSerializer.Serialize(dictionary, new JsonSerializerOptions() { WriteIndented = true }));
            }
        }

        private Dictionary<string, string> InternalLoad(FileInfo fileInfo)
        {
            using var reader = new StreamReader(fileInfo.OpenRead());
            return JsonSerializer.Deserialize<Dictionary<string, string>>(reader.ReadToEnd()) ?? new ();
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
    }
}