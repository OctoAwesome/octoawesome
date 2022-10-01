using System;

using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Controls
{
    internal class NumericTextbox : Textbox
    {
        public NumericTextbox()
        {
        }

        protected override void OnKeyTextPress(KeyTextEventArgs args)
        {
            if (args.Character.IsAscii && char.IsNumber((char)args.Character.Value))
            {
                base.OnKeyTextPress(args);
            }
            else
            {
                args.Handled = true;
            }
        }
    }
}
