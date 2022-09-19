using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.UI.Controls
{
    /// <summary>
    /// Control for grouping other controls.
    /// </summary>
    public class GroupBox : Control
    {
        private Brush borderColor = SolidColorBrush.Black;

        /// <summary>
        /// Gets or sets the border color brush.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the border size.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the orientation for the inner stack panel.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the head line for the group box.
        /// </summary>
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

        /// <inheritdoc cref="Control.Children"/>
        public new ControlCollection Children => contentPanel.Controls;

        private readonly StackPanel outerPanel;
        private readonly StackPanel contentPanel;
        private readonly Label headlineLabel;

        /// <summary>
        /// Initializes a new instance of the <see cref="GroupBox"/> control.
        /// </summary>
        /// <param name="manager">The <see cref="engenious.UI.BaseScreenComponent" />.</param>
        /// <param name="style">The style to use for this control.</param>
        public GroupBox(string style = "", BaseScreenComponent manager = null)
            : base(style, manager)
        {
            outerPanel = new()
            {
                Padding = Border,
                Background = BorderColor
            };

            StackPanel innerPanel = new()
            {
                Orientation = Orientation.Vertical,
                Background = SolidColorBrush.White,
            };

            StackPanel headlinePanel = new()
            {
                Orientation = Orientation.Vertical,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            headlineLabel = new Label()
            {
                Text = Headline,
                Height = 0,
            };

            headlinePanel.Controls.Add(headlineLabel);
            headlinePanel.Controls.Add(new Line()
            {
                Color = SolidColorBrush.Black,
            });
            innerPanel.Controls.Add(headlinePanel);

            contentPanel = new StackPanel()
            {
                Orientation = Orientation,
                Background = SolidColorBrush.White,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };

            innerPanel.Controls.Add(contentPanel);

            outerPanel.Controls.Add(innerPanel);
            base.Children.Add(outerPanel);
        }
    }
}
