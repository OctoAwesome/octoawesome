using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    public interface IScreenManager
    {
        Texture2D Pix { get; }

        SpriteFont NormalText { get; }

        Index2 ScreenSize { get; }

        ContentManager Content { get; }

        GraphicsDevice GraphicsDevice { get; }

        void Close();
    }
}
