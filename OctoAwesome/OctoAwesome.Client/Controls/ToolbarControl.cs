using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGameUi;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Controls
{
    internal class ToolbarControl : Panel
    {
        private Dictionary<string, Texture2D> toolTextures;

        private Button[] buttons = new Button[OctoAwesome.Player.TOOLCOUNT];

        private Image[] images = new Image[OctoAwesome.Player.TOOLCOUNT];

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
                using (MemoryStream stream = new MemoryStream())
                {
                    System.Drawing.Bitmap bitmap = item.Icon;
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    toolTextures.Add(item.GetType().FullName, Texture2D.FromStream(ScreenManager.GraphicsDevice, stream));
                }
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

            for (int i = 0; i < OctoAwesome.Player.TOOLCOUNT; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

            activeToolLabel = new Label(screenManager);
            activeToolLabel.VerticalAlignment = VerticalAlignment.Top;
            activeToolLabel.HorizontalAlignment = HorizontalAlignment.Center;
            activeToolLabel.Background = new BorderBrush(Color.Black * 0.3f);
            activeToolLabel.TextColor = Color.White;
            grid.AddControl(activeToolLabel, 0, 0, OctoAwesome.Player.TOOLCOUNT);

            for (int i = 0; i < OctoAwesome.Player.TOOLCOUNT; i++)
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
            for (int i = 0; i < OctoAwesome.Player.TOOLCOUNT; i++)
            {
                if (Player.ActorHost.Player.Tools != null && 
                    Player.ActorHost.Player.Tools.Length > i && 
                    Player.ActorHost.Player.Tools[i] != null && 
                    Player.ActorHost.Player.Tools[i].Definition != null)
                {
                    images[i].Texture = toolTextures[Player.ActorHost.Player.Tools[i].Definition.GetType().FullName];

                    if (Player.ActorHost.ActiveTool == Player.ActorHost.Player.Tools[i])
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

            base.OnUpdate(gameTime);
        }
    }
}
