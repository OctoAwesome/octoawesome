using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.UI.Controls
{
    /// <summary>
    /// Control for displaying entity health.
    /// </summary>
    public class HealthBarControl : ProgressBar
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HealthBarControl"/> control.
        /// </summary>
        /// <param name="manager">The <see cref="engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="style">The style to use for this control.</param>
        public HealthBarControl(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            Background = Skin.Current.HorizontalScrollBackgroundBrush;
        }
    }
}
