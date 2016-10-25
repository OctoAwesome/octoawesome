using MonoGameUi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Controls
{
    internal class HealthBarControl : ProgressBar
    {
        public HealthBarControl(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            Background = Skin.Current.HorizontalScrollBackgroundBrush;
        }
    }
}
