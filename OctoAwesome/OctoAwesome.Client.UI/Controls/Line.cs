using engenious.UI;

namespace OctoAwesome.Client.UI.Controls
{
    /// <summary>
    /// Simple control for drawing lines.
    /// </summary>
    public class Line : Control
    {
        /// <summary>
        /// Gets or sets the color of the line.
        /// </summary>
        public Brush? Color { get => Background; set => Background = value; }

        /// <summary>
        /// Initializes a new instance of teh <see cref="Line"/> class.
        /// </summary>
        /// <param name="style">The style to use for this control.</param>
        /// <param name="manager">The <see cref="engenious.UI.BaseScreenComponent" />.</param>
        public Line(string style = "", BaseScreenComponent? manager = null)
            : base(style, manager)
        {
            Height = 1;
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
