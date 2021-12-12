using System.Diagnostics;

namespace OctoAwesome.Client.UI
{

    public static class Tools
    {
        /// <summary>
        /// Öffnet die gegebene Url im Betriebsystem Browser
        /// </summary>
        /// <param name="url">Die Url zum öffnen</param>
        public static void OpenUrl(string url)
        {
            Process.Start(new ProcessStartInfo(url)
            {
                UseShellExecute = true
            })?.Dispose();
        }
    }
}
