using MonoGameUi;
using OctoAwesome.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Basics.Controls
{
    internal class HealthBarControl : ProgressBar
    {
        public HealthBarControl(BaseScreenComponent screenmanager, string style = "") : base(screenmanager, style)
        {
            Background = Skin.Current.HorizontalScrollBackgroundBrush;

            HorizontalAlignment = HorizontalAlignment.Left;
            VerticalAlignment = VerticalAlignment.Bottom;
            Width = 240;
            Height = 78;
            Maximum = 100;
            Value = 40;
            Margin = Border.All(20, 30);
        }
        public override string ToString()
        {
            return "Health";
        }
    }
}
