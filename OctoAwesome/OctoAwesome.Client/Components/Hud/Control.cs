using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Control : UiElement
    {
        public Index2 Position { get; set; }

        public Index2 Size { get; set; }

        public bool Enabled { get; set; }

        public bool Visible { get; set; }

        public Control(HudComponent hud) : base(hud)
        {
            Enabled = true;
            Visible = true;
        }

        
    }
}
