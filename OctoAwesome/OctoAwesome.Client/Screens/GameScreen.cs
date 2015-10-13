using MonoGameUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Client.Controls;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Screens
{
    internal sealed class GameScreen : Screen
    {
        public GameScreen(ScreenComponent manager) : base(manager)
        {
            SceneControl scene = new SceneControl(manager);
            scene.HorizontalAlignment = HorizontalAlignment.Stretch;
            scene.VerticalAlignment = VerticalAlignment.Stretch;
            Controls.Add(scene);
        }
    }
}
