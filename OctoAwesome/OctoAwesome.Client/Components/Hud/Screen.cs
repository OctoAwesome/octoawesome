using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Hud
{
    internal abstract class Screen : UiElement
    {
        public bool RequiresPointer { get; set; }

        private List<Control> controls = new List<Control>();

        public List<Control> Controls
        {
            get
            {
                return controls;
            }
        }

        public Screen(IScreenManager screenManager) : base(screenManager)
        {
            RequiresPointer = true;
        }
    }
}
