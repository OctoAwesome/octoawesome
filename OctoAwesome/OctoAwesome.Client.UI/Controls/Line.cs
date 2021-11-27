using engenious.UI;

namespace OctoAwesome.Client.UI.Controls
{
    public class Line : Control
    {
        public Brush Color { get => Background; set => Background = value; }

        public Line(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            Height = 1;
            HorizontalAlignment = HorizontalAlignment.Stretch;
        }
    }
}
