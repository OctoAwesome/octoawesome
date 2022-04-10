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
        T Get<T>(string key, T defaultValue);

        /// <summary>
        /// Gets a setting value as an array associated to a key.
        /// </summary>
        /// <param name="key">The identification key for the setting to query.</param>
        /// <returns>The value array of the setting.</returns>
        T[] GetArray<T>(string key);

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