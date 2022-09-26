using System.Diagnostics.CodeAnalysis;

namespace OctoAwesome
{
    /// <summary>
    /// Interface managing application settings.
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Gets the setting value associated to a key.
        /// </summary>
        /// <param name="key">The identification key for the setting to query.</param>
        /// <returns>The value of the setting.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gets the setting value associated to a key.
        /// </summary>
        /// <param name="key">The identification key for the setting to query.</param>
        /// <param name="defaultValue">
        /// The default value to fall back to if no setting associated to the key was found.
        /// </param>
        /// <returns>
        /// The value of the setting, or <paramref name="defaultValue"/> if no matching setting was found.
        /// </returns>
        [return: NotNullIfNotNull(parameterName: "defaultValue")]
        T? Get<T>(string key, T? defaultValue);

        /// <summary>
        /// Gets a setting value as an array associated to a key.
        /// </summary>
        /// <param name="key">The identification key for the setting to query.</param>
        /// <returns>The value array of the setting.</returns>
        T[] GetArray<T>(string key);

        /// <summary>
        /// Gets the setting value associated to a key.
        /// </summary>
        /// <param name="key">The identification key for the setting to query.</param>
        /// <param name="value">The value of the setting.</param>
        /// <returns>
        /// <see langword="true"/> if a value associated with the <paramref name="key"/> exists;
        /// <see langword="false"/> otherwise.
        /// </returns>
        bool TryGet<T>(string key, [MaybeNullWhen(false)] out T value);

        /// <summary>
        /// Gets a setting value as an array associated to a key.
        /// </summary>
        /// <param name="key">The identification key for the setting to query.</param>
        /// <param name="values">The value array of the setting.</param>
        /// <returns>
        /// <see langword="true"/> if an array associated with the <paramref name="key"/> exists;
        /// <see langword="false"/> otherwise.
        /// </returns>
        bool TryGetArray<T>(string key, [MaybeNullWhen(false)] out T[] values);
        
        /// <summary>
        /// Checks whether a setting referenced by an identification key exists.
        /// </summary>
        /// <param name="key">The identification key to check for.</param>
        /// <returns>A value indicating whether the setting exists.</returns>
        bool KeyExists(string key);

        /// <summary>
        /// Sets the value of a setting by using an identification key.
        /// </summary>
        /// <param name="key">The identification key for the setting to set.</param>
        /// <param name="value">The new value for the settings.</param>
        void Set(string key, string value);

        /// <summary>
        /// Sets the value of a setting by using an identification key.
        /// </summary>
        /// <param name="key">The identification key for the setting to set.</param>
        /// <param name="value">The new value for the settings.</param>
        void Set(string key, int value);

        /// <summary>
        /// Sets the value of a setting by using an identification key.
        /// </summary>
        /// <param name="key">The identification key for the setting to set.</param>
        /// <param name="value">The new value for the settings.</param>
        void Set(string key, bool value);

        /// <summary>
        /// Sets the value of a setting by using an identification key.
        /// </summary>
        /// <param name="key">The identification key for the setting to set.</param>
        /// <param name="values">The new value for the settings.</param>
        void Set(string key, string[] values);

        /// <summary>
        /// Sets the value of a setting by using an identification key.
        /// </summary>
        /// <param name="key">The identification key for the setting to set.</param>
        /// <param name="values">The new value for the settings.</param>
        void Set(string key, int[] values);

        /// <summary>
        /// Sets the value of a setting by using an identification key.
        /// </summary>
        /// <param name="key">The identification key for the setting to set.</param>
        /// <param name="values">The new value for the settings.</param>
        void Set(string key, bool[] values);

        /// <summary>
        /// Deletes a setting from the settings using an identification key.
        /// </summary>
        /// <param name="key">The identification key to delete the setting of.</param>
        void Delete(string key);
    }
}