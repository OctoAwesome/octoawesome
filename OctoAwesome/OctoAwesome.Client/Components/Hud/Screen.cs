using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Screen : UiElement
    {
        private List<Control> controls = new List<Control>();

        protected List<Control> Controls
        {
            get
            {
                return controls;
            }
        }

        public Screen(HudComponent hud) : base(hud)
        {

        }
    }
}
