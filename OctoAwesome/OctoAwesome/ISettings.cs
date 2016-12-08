namespace OctoAwesome
{
    /// <summary>
    /// Interface zur Verwaltung der Anwendungseinstellungen
    /// </summary>
    public interface ISettings
    {
        /// <summary>
        /// Gibt den Wert einer Einstellung zurück.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns>Der Wert der Einstellung.</returns>
        T Get<T>(string key);

        /// <summary>
        /// Gibt den Wert einer Einstellung zurück.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="defaultValue">Default-Wert, der zurückgegeben wird, wenn der key nicht vorhanden ist.</param>
        /// <returns>Der Wert der Einstellung oder der Default-Wert.</returns>
        T Get<T>(string key, T defaultValue);

        /// <summary>
        /// Gibt das Array einer Einstellung zurück
        /// </summary>
        /// <typeparam name="T">Art der Einstellung</typeparam>
        /// <param name="key">Schlüssel der Einstellung</param>
        /// <returns>Das Array der Einstellung</returns>
        T[] GetArray<T>(string key);

        /// <summary>
        /// Überprüft, ob die angegebene Einstellung existeiert.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <returns></returns>
        bool KeyExists(string key);

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        void Set(string key, string value);

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        void Set(string key, int value);

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="value">Der Wert der Einstellung.</param>
        void Set(string key, bool value);

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="values">Der Wert der Einstellung.</param>
        void Set(string key, string[] values);

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="values">Der Wert der Einstellung.</param>
        void Set(string key, int[] values);

        /// <summary>
        /// Setzt den Wert einer Eigenschaft.
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung.</param>
        /// <param name="values">Der Wert der Einstellung.</param>
        void Set(string key, bool[] values);

        /// <summary>
        /// Löscht eine Eigenschaft aus den Einstellungen
        /// </summary>
        /// <param name="key">Der Schlüssel der Einstellung</param>
        void Delete(string key);
    }
}