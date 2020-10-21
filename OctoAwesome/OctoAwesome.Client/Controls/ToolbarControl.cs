using engenious.UI;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using engenious;
using engenious.Graphics;
using OctoAwesome.EntityComponents;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        public PlayerComponent Player { get; set; }

        private Dictionary<string, Texture2D> toolTextures;

        private Button[] buttons = new Button[ToolBarComponent.TOOLCOUNT];

        private Image[] images = new Image[ToolBarComponent.TOOLCOUNT];

        private Brush buttonBackgroud;

        private Brush activeBackground;

        private Label activeToolLabel;

        private int lastActiveIndex;

        public ToolbarControl(ScreenComponent screenManager)
            : base(screenManager)
        {
            Player = screenManager.Player;
            toolTextures = new Dictionary<string, Texture2D>();

            buttonBackgroud = new BorderBrush(Color.Black);
            activeBackground = new BorderBrush(Color.Red);

            foreach (var item in screenManager.Game.DefinitionManager.Definitions)
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

            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

            activeToolLabel = new Label(screenManager);
            activeToolLabel.VerticalAlignment = VerticalAlignment.Top;
            activeToolLabel.HorizontalAlignment = HorizontalAlignment.Center;
            activeToolLabel.Background = new BorderBrush(Color.Black * 0.3f);
            activeToolLabel.TextColor = Color.White;
            grid.AddControl(activeToolLabel, 0, 0, ToolBarComponent.TOOLCOUNT);

            for (int i = 0; i < ToolBarComponent.TOOLCOUNT; i++)
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

            if (Player.Toolbar.ActiveIndex != lastActiveIndex)
            {
                buttons[lastActiveIndex].Background = buttonBackgroud;
                lastActiveIndex = Player.Toolbar.ActiveIndex;
            }

            buttons[Player.Toolbar.ActiveIndex].Background = activeBackground;
            var definitionName = Player.Toolbar.ActiveTool.Definition.GetType().FullName;

            if (toolTextures.TryGetValue(definitionName, out var texture))
                images[Player.Toolbar.ActiveIndex].Texture = texture;

            // Aktualisierung des ActiveTool Labels
            activeToolLabel.Text = Player.Toolbar.ActiveTool != null ?
                string.Format("{0} ({1})", Player.Toolbar.ActiveTool.Definition.Name, Player.Toolbar.ActiveTool.Amount) :
                string.Empty;

            activeToolLabel.Visible = !(activeToolLabel.Text == string.Empty);

            base.OnUpdate(gameTime);
        }
    }
}
