using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Control
    {
        public Index2 Position { get; set; }

        public Index2 Size { get; set; }

        public bool Enabled { get; set; }

        public bool Visible { get; set; }

        protected HudComponent Hud { get; private set; }

        public Control(HudComponent hud)
        {
            Hud = hud;
            Enabled = true;
            Visible = true;
        }

        public abstract void LoadContent();

        public abstract void Draw(SpriteBatch batch, GameTime gameTime);
    }
}
