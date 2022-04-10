using System.Diagnostics;

namespace OctoAwesome.Client.UI
{
    /// <summary>
    /// Helper tools for external UI interaction.
    /// </summary>
    public static class Tools
    {
        /// <summary>
        /// Opens a given Url in the default browser.
        /// </summary>
        /// <param name="url">The url to open.</param>
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            })?.Dispose();
        }
    }
}
