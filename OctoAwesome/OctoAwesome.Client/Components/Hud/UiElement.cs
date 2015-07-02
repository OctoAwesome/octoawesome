using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class UiElement
    {
        protected HudComponent Hud { get; private set; }

        public UiElement(HudComponent hud)
        {
            Hud = hud;
        }

        public virtual void LoadContent() { }

        public abstract void Draw(SpriteBatch batch, GameTime gameTime);
    }
}
