﻿using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using engenious;
using engenious.Graphics;

namespace OctoAwesome.Client.Controls
{
    internal class MinimapControl : Control
    {
        public SceneControl Scene { get; set; }

        public MinimapControl(ScreenComponent screenManager, SceneControl scene)
            : base(screenManager)
        {
            Scene = scene;
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            if (Scene == null || Scene.MiniMapTexture == null)
                return;

            batch.Draw(Skin.Pix, new Rectangle(contentArea.X - 2, contentArea.Y - 2, contentArea.Width + 4, contentArea.Height + 4), Color.Black);
            batch.Draw(Scene.MiniMapTexture, new Rectangle(contentArea.X, contentArea.Y, contentArea.Width, contentArea.Height), Color.White);

            Index2 center = new Index2((contentArea.Width / 2) + contentArea.X, (contentArea.Height / 2) + contentArea.Y);

            batch.Draw(Skin.Pix, new Rectangle(center.X - 1, center.Y - 1, 2, 2), Color.White);
        }
    }
}
