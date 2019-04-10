using engenious.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Screens
{
    internal abstract class BaseScreen : Screen
    {
        private readonly AssetComponent assets;

        protected Button BackButton;

        public BaseScreen(ScreenComponent manager) : base(manager)
        {
            assets = manager.Game.Assets;
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            if (Manager.CanGoBack)
            {
                BackButton = Button.TextButton(Manager, Languages.OctoClient.Back);
                BackButton.VerticalAlignment = VerticalAlignment.Top;
                BackButton.HorizontalAlignment = HorizontalAlignment.Left;
                BackButton.LeftMouseClick += (s, e) =>
                {
                    Manager.NavigateBack();
                };
                BackButton.Margin = new Border(10, 10, 10, 10);
                Controls.Add(BackButton);
            }

        }

        protected void SetDefaultBackground()
        {
            Background = new TextureBrush(assets.LoadTexture(typeof(ScreenComponent), "background_new"), TextureBrushMode.Stretch);
        }

        protected override void OnKeyPress(KeyEventArgs args)
        {
            if (Manager.CanGoBack && args.Key == Keys.Back)
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyPress(args);
        }

        protected void AddLabeledControl(Grid grid, string name, Control c)
        {
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto });
            grid.AddControl(new Label(Manager) { Text = name }, 0, grid.Rows.Count - 1);
            grid.AddControl(c, 1, grid.Rows.Count - 1);
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 10 });
        }

    }
}
