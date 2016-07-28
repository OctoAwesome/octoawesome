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
        

        // private Texture2D[] toolTextures;
        // private Dictionary<IItemDefinition, Texture2D> toolTextures;
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

            activeToolLabel = new Label(screenManager);
            activeToolLabel.VerticalAlignment = VerticalAlignment.Top;
            activeToolLabel.HorizontalAlignment = HorizontalAlignment.Center;
            activeToolLabel.Background = new BorderBrush(Color.Black * 0.3f);
            activeToolLabel.TextColor = Color.White;
            activeToolLabel.Visible = false;
            Controls.Add(activeToolLabel);

            // toolTextures = new Texture2D[Player.Tools.Length];
            // int index = 0;
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

            grid.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Fixed, Height = 50 });

            for (int i = 0; i < OctoAwesome.Player.TOOLCOUNT; i++)
            {
                grid.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Fixed, Width = 50 });
            }

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
                grid.AddControl(buttons[i], i, 0);
            }
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            if (!Visible || !Enabled)
                return;

            if (Player.ActorHost == null) return;

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

            base.OnUpdate(gameTime);
        }

        //protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        //{
        //    if (!Visible || !Enabled)
        //        return;

        //    if (Player.ActorHost == null) return;

        //    if (Player.Tools != null && Player.Tools.Count > 0) // > 0 Check erforderlich da durch einen Bug ActiveTool auch gesetzt bleibt wenn kein Tool mehr vorhanden ist
        //    {
        //        int width = Player.Tools.Count * 32 + (Player.Tools.Count - 1) * 10;
        //        int offset = (contentArea.Width - width) / 2;
        //        int index = 0;

        //        if (Player.ActorHost.ActiveTool != null)
        //        {
        //            activeToolLabel.Visible = true;
        //            activeToolLabel.Text = Player.ActorHost.ActiveTool.Definition.Name;
        //        }

        //        foreach (var tool in Player.Tools)
        //        {
        //            batch.Draw(Skin.Pix, new Rectangle(offset + (index * 42) - 2 + contentArea.X, contentArea.Height - 60 - 2 + contentArea.Y, 36, 36),
        //                Player.ActorHost.ActiveTool == tool ? Color.Gold : new Color(Color.White, 0.8f));
        //            batch.Draw(toolTextures[tool.Definition.GetType().FullName], new Rectangle(offset + (index * 42) + contentArea.X, contentArea.Height - 60 + contentArea.Y, 32, 32),
        //               Player.ActorHost.ActiveTool == tool ? Color.White : new Color(Color.White, 0.8f));

        //            index++;
        //        }
        //    }
        //    else
        //    {
        //        activeToolLabel.Visible = false;
        //    }
        //}
    }
}
