using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal class Toolbar : Control
    {
        private Texture2D[] toolTextures;

        public PlayerComponent Player { get; set; }

        public Toolbar(IScreenManager screenManager, PlayerComponent player)
            : base(screenManager)
        {
            Player = player;
        }

        public override void LoadContent()
        {
            toolTextures = new Texture2D[Player.Tools.Length];
            int index = 0;
            foreach (var tool in Player.Tools)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    System.Drawing.Bitmap bitmap = tool.Textures.First();
                    bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    stream.Seek(0, SeekOrigin.Begin);

                    toolTextures[index++] = Texture2D.FromStream(ScreenManager.GraphicsDevice, stream);
                }
            }
        }

        public override void Draw(SpriteBatch batch, GameTime gameTime)
        {
            if (!Visible || !Enabled)
                return;

            batch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied);

            if (Player.Tools != null && Player.Tools.Length > 0)
            {
                int width = Player.Tools.Length * 32 + (Player.Tools.Length - 1) * 10;
                int offset = (Size.X - width) / 2;
                int index = 0;
                foreach (var definition in BlockDefinitionManager.GetBlockDefinitions())
                {
                    batch.Draw(ScreenManager.Pix, new Rectangle(offset + (index * 42) - 2 + Position.X, Size.Y - 60 - 2 + Position.Y, 36, 36),
                        Player.Player.ActiveTool == definition ? Color.White : new Color(Color.White, 0.3f));
                    batch.Draw(toolTextures[index], new Rectangle(offset + (index * 42) + Position.X, Size.Y - 60 + Position.Y, 32, 32),
                        Player.Player.ActiveTool == definition ? Color.White : new Color(Color.White, 0.3f));

                    index++;
                }
            }

            batch.End();
        }
    }
}
