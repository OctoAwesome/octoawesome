using MonoGameUi;
using OctoAwesome.Client.Components;
using System.Collections.Generic;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        private Dictionary<string, Texture2D> toolTextures;
        // TODO: dynamisch aus der aktuellen toolbar generieren
        //private Button[] buttons = new Button[ToolBarComponent.TOOLCOUNT];
        private Button[] buttons = new Button[10];

        // TODO: dynamisch aus der aktuellen toolbar generieren
        //private Image[] images = new Image[ToolBarComponent.TOOLCOUNT];
        private Image[] images = new Image[10];

        private Brush buttonBackgroud;

        private Brush activeBackground;

        public PlayerComponent Player { get; set; }

        public Label activeToolLabel;

        public ToolbarControl(ScreenComponent screenManager) : base(screenManager)
        {
            Player = screenManager.Player;
            toolTextures = new Dictionary<string, Texture2D>();

            buttonBackgroud = new BorderBrush(Color.Black);
            activeBackground = new BorderBrush(Color.Red);

            foreach (var item in screenManager.Game.DefinitionManager.GetDefinitions())
            {
                Texture2D texture = screenManager.Game.Assets.LoadTexture(item.GetType(), item.Icon);
                toolTextures.Add(item.GetType().FullName, texture);
            }

            Grid grid = new Grid(screenManager)
            {
                Margin = new Border(0, 0, 0, 0),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Bottom
            };
            Controls.Add(grid);

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Auto, Height = 1 });
            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            // TODO: dynamisch aus der aktuellen toolbar generieren
            //for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            for (int i = 0; i < 10; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

            activeToolLabel = new Label(screenManager);
            activeToolLabel.VerticalAlignment = VerticalAlignment.Top;
            activeToolLabel.HorizontalAlignment = HorizontalAlignment.Center;
            activeToolLabel.Background = new BorderBrush(Color.Black * 0.3f);
            activeToolLabel.TextColor = Color.White;
            // TODO: dynamisch aus der aktuellen toolbar generieren
            //grid.AddControl(activeToolLabel, 0, 0, ToolBarComponent.TOOLCOUNT);
            // TODO: bezieht sich die rowspan auf die anzahl der tools ?
            grid.AddControl(activeToolLabel, 0, 0, 10);

            //for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            for (int i = 0; i < 10; i++)
            {
                buttons[i] = new Button(screenManager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    Background = buttonBackgroud,
                    HoveredBackground = null,
                    PressedBackground = null,
                };
                buttons[i].Content = images[i] = new Image(screenManager)
                {
                    Width = 42,
                    Height = 42,
                };
                grid.AddControl(buttons[i], i, 1);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled)
                return;

            if (Player.CurrentEntity == null) return;

            //TODO: wieder hinzufügen, oder anders lösen
            // Aktualisierung des aktiven Buttons
            // TODO: dynamisch aus der aktuellen toolbar generieren
            //for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            //for (int i = 0; i < 10; i++)
            //{
            //    if (Player.Toolbar.Tools != null &&
            //        Player.Toolbar.Tools.Length > i &&
            //        Player.Toolbar.Tools[i] != null &&
            //        Player.Toolbar.Tools[i].Definition != null)
            //    {
            //        images[i].Texture = toolTextures[Player.Toolbar.Tools[i].Definition.GetType().FullName];

            //        if (Player.Toolbar.ActiveTool == Player.Toolbar.Tools[i])
            //            buttons[i].Background = activeBackground;
            //        else
            //            buttons[i].Background = buttonBackgroud;
            //    }
            //    else
            //    {
            //        images[i].Texture = null;
            //        buttons[i].Background = buttonBackgroud;
            //    }
            //}

            //TODO: wieder hinzufügen, oder anders lösen
            // Aktualisierung des ActiveTool Labels
            //activeToolLabel.Text = Player.Toolbar.ActiveTool != null ?
            //    string.Format("{0} ({1})", Player.Toolbar.ActiveTool.Definition.Name, Player.Toolbar.ActiveTool.Amount) :
            //    string.Empty;

            activeToolLabel.Visible = !(activeToolLabel.Text == string.Empty);

            base.OnUpdate(gameTime);
        }
    }
}
