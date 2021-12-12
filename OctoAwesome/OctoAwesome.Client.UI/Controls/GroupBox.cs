using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.UI.Controls
{

    public class GroupBox : Control
    {
        private Brush borderColor = SolidColorBrush.Black;
        public Brush BorderColor
        {
            get => borderColor;
            set
            {
                if (outerPanel is not null)
                    outerPanel.Background = value;
                borderColor = value;
            }
        }

        private Border border = Border.All(2);
        public Border Border
        {
            get => border;
            set
            {
                if (outerPanel is not null)
                    outerPanel.Padding = value;
                border = value;
            }
        }
        private Orientation orientation = Orientation.Vertical;
        public Orientation Orientation
        {
            get => orientation;
            set
            {
                if (contentPanel is not null)
                    contentPanel.Orientation = value;
                orientation = value;
            }
        }

        private string headline;
        public string Headline
        {
            get => headline;
            set
            {
                headline = value ?? string.Empty;
                if (headlineLabel is not null)
                {
                    headlineLabel.Height = string.IsNullOrEmpty(headline) ? 0 : null;
                    headlineLabel.Text = headline;
                }
            }
        }
        public new ControlCollection Children => contentPanel.Controls;

        private readonly StackPanel outerPanel;
        private readonly StackPanel contentPanel;
        private readonly Label headlineLabel;
        public GroupBox(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            outerPanel = new(manager)
                         {
                             Padding = Border,
                             Background = BorderColor
                         };

            var innerPanel = new StackPanel(manager)
                             {
                                 Orientation = Orientation.Vertical,
                                 Background = SolidColorBrush.White,
                             };

            StackPanel headlinePanel = new(manager)
                                       {
                                           Orientation= Orientation.Vertical,
                                           HorizontalAlignment = HorizontalAlignment.Stretch,
                                       };

            headlineLabel = new Label(manager)
            {
                Text = Headline,
                Height = 0,
            };

            headlinePanel.Controls.Add(headlineLabel);
            headlinePanel.Controls.Add(new Line(manager)
            {
                Color = SolidColorBrush.Black,
            });
            innerPanel.Controls.Add(headlinePanel);

            contentPanel = new StackPanel(manager)
            {
                Orientation = Orientation,
                Background = SolidColorBrush.White,
                HorizontalAlignment= HorizontalAlignment.Stretch,
            };

            innerPanel.Controls.Add(contentPanel);

            outerPanel.Controls.Add(innerPanel);
            base.Children.Add(outerPanel);
        }
    }
}
