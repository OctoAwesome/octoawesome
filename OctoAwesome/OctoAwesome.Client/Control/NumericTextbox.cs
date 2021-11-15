using engenious.UI;
using engenious.UI.Controls;

namespace OctoAwesome.Client.Controls
{
    internal class NumericTextbox : Textbox
    {
        public NumericTextbox(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
        }

        protected override void OnKeyTextPress(KeyTextEventArgs args)
        {
            if (char.IsNumber(args.Character))
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
