using MonoGameUi;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        private Dictionary<string, Texture2D> toolTextures;

        private Button[] buttons = new Button[EntityComponents.PlayerComponent.TOOLCOUNT];

        private Image[] images = new Image[EntityComponents.PlayerComponent.TOOLCOUNT];

        private Brush buttonBackgroud;

        private Brush activeBackground;

        public PlayerComponent Player { get; set; }

        public Label activeToolLabel;

        public ToolbarControl(ScreenComponent screenManager)
            : base(screenManager)
        {
            Player = screenManager.Player;
            toolTextures = new Dictionary<string, Texture2D>();

            buttonBackgroud = new BorderBrush(Color.Black);
            activeBackground = new BorderBrush(Color.Red);

            foreach (var item in DefinitionManager.Instance.GetItemDefinitions())
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

            for (int i = 0; i < EntityComponents.PlayerComponent.TOOLCOUNT; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

            activeToolLabel = new Label(screenManager);
            activeToolLabel.VerticalAlignment = VerticalAlignment.Top;
            activeToolLabel.HorizontalAlignment = HorizontalAlignment.Center;
            activeToolLabel.Background = new BorderBrush(Color.Black * 0.3f);
            activeToolLabel.TextColor = Color.White;
            grid.AddControl(activeToolLabel, 0, 0, EntityComponents.PlayerComponent.TOOLCOUNT);

            for (int i = 0; i < EntityComponents.PlayerComponent.TOOLCOUNT; i++)
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

            if (Player.ActorHost == null) return;

            // Aktualisierung des aktiven Buttons
            for (int i = 0; i < EntityComponents.PlayerComponent.TOOLCOUNT; i++)
            {
                if (Player.ActorHost.PlayerInventory.Tools != null && 
                    Player.ActorHost.PlayerInventory.Tools.Length > i && 
                    Player.ActorHost.PlayerInventory.Tools[i] != null && 
                    Player.ActorHost.PlayerInventory.Tools[i].Definition != null)
                {
                    images[i].Texture = toolTextures[Player.ActorHost.PlayerInventory.Tools[i].Definition.GetType().FullName];

                    if (Player.ActorHost.ActiveTool == Player.ActorHost.PlayerInventory.Tools[i])
                        buttons[i].Background = activeBackground;
                    else
                        buttons[i].Background = buttonBackgroud;
                }
                else
                {
                    images[i].Texture = null;
                    buttons[i].Background = buttonBackgroud;
                }
            }

            // Aktualisierung des ActiveTool Labels
            activeToolLabel.Text = Player.ActorHost.ActiveTool != null ? 
                string.Format("{0} ({1})", Player.ActorHost.ActiveTool.Definition.Name, Player.ActorHost.ActiveTool.Amount) : 
                string.Empty;

            activeToolLabel.Visible = !(activeToolLabel.Text == string.Empty);

            base.OnUpdate(gameTime);
        }
    }
}
