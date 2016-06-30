using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Screens
{
    internal sealed class InventoryScreen : Screen
    {
        private PlayerComponent player;

        public InventoryScreen(ScreenComponent manager) : base(manager)
        {
            player = manager.Player;
            IsOverlay = true;

            Texture2D panelBackground = manager.Game.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);

            HorizontalAlignment = HorizontalAlignment.Center;
            VerticalAlignment = VerticalAlignment.Center;
            Width = 600;
            Height = 400;

            Label headLine = new Label(manager);
            headLine.Text = Languages.OctoClient.Inventory;
            headLine.Font = Skin.Current.HeadlineFont;
            headLine.HorizontalAlignment = HorizontalAlignment.Left;
            headLine.VerticalAlignment = VerticalAlignment.Top;
            Controls.Add(headLine);

            Button closeButton = Button.TextButton(manager, Languages.OctoClient.Close);
            closeButton.LeftMouseClick += (s, e) => { manager.NavigateBack(); };
            Controls.Add(closeButton);

            //counter = new LabelControl(ScreenManager)
            //{
            //    Font = ScreenManager.NormalText,
            //    Color = Color.Black,
            //    Position = new Index2(((ScreenManager.ScreenSize.X - 600) / 2) + 100,
            //        ((ScreenManager.ScreenSize.Y - 400) / 2) + 140),
            //};
            //Controls.Add(counter);

            Title = Languages.OctoClient.Inventory;
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (Manager.CanGoBack && (args.Key == Keys.Escape || args.Key == Keys.I))
            {
                args.Handled = true;
                Manager.NavigateBack();
            }

            base.OnKeyDown(args);
        }

        protected override void OnNavigatedTo(NavigationEventArgs args)
        {
            Manager.FreeMouse();
            base.OnNavigatedTo(args);
        }
    }
}
