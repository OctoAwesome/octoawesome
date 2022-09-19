using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Components;
using System;

namespace OctoAwesome.UI.Screens
{
    /// <summary>
    /// Screen container control page for OctoAwesome screen.
    /// </summary>
    public abstract class BaseScreen : Screen
    {
        protected readonly AssetComponent assets;

        private readonly Button backButton;
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseScreen"/> class.
        /// </summary>
        /// <param name="assets">The asset component to load the assets from.</param>
        public BaseScreen(AssetComponent assets)
        {
            this.assets = assets;
            backButton = new TextButton("Back")
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                TabStop = false,
            };
            backButton.LeftMouseClick += (s, e) =>
            { 
                ScreenManager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);
        }

        /// <summary>
        /// Adds the ui component to this base screen
        /// </summary>
        /// <param name="uiComponent">The ui component to add</param>
        public virtual void AddUiComponent(UIComponent uiComponent)
        {

        }
        /// <summary>
        /// Removes the ui component from this base screen
        /// </summary>
        /// <param name="uiComponent">The ui component to remove</param>
        public virtual void RemoveUiComponent(UIComponent uiComponent)
        {
        }

        /// <summary>
        /// Sets the default background to background_new
        /// </summary>
        protected void SetDefaultBackground()
        {
            Background = new TextureBrush(assets.LoadTexture("background_new"), TextureBrushMode.Stretch);
        }

        /// <inheritdoc/>
        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (ScreenManager.CanGoBack && (args.Key == Keys.Back || args.Key == Keys.Escape))
            {
                args.Handled = true;
                ScreenManager.NavigateBack();
            }

            base.OnKeyPress(args);
        }

        /// <summary>
        /// Adds a <see cref="Control"/> to the <see cref="Grid"/> with a new <see cref="Label"/>
        /// </summary>
        /// <param name="grid">The grid that should contain the label and control</param>
        /// <param name="name">The display name used for the label</param>
        /// <param name="c">The control that should belong to the label</param>
        protected void AddLabeledControl(Grid grid, string name, Control c)
        {
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto });
            grid.AddControl(new Label() { Text = name, TabStop = false }, 0, grid.Rows.Count - 1);
            grid.AddControl(c, 1, grid.Rows.Count - 1);
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 10 });
        }

        /// <summary>
        /// Creates a button with the title
        /// </summary>
        /// <param name="title">The title for the button</param>
        /// <returns>The button that was created</returns>
        protected Button GetButton(string title)
        {
            Button button = new TextButton(title, Style, ScreenManager)
            {
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            return button;
        }

    }
}
