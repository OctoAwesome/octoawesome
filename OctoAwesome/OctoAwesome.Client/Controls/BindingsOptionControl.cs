using engenious;
using engenious.Input;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.Components;
using OctoAwesome.Client.Screens;
using OctoAwesome.Client.UI.Components;

using System.Linq;

namespace OctoAwesome.Client.Controls
{
    internal sealed class BindingsOptionControl : Panel
    {
        private readonly AssetComponent assets;
        private readonly ISettings settings;
        private readonly KeyMapper keyMapper;

        public BindingsOptionControl(AssetComponent assets, KeyMapper keyMapper, ISettings settings)
        {
            this.assets = assets;
            this.settings = settings;
            this.keyMapper = keyMapper;
            ScrollContainer bindingsScroll = new ScrollContainer();
            Controls.Add(bindingsScroll);

            StackPanel bindingsStack = new StackPanel()
            {
                Orientation = Orientation.Vertical,
                Padding = new Border(20, 20, 20, 20),
                Width = 650
            };
            bindingsScroll.Content = bindingsStack;

            //////////////////////////////KeyBindings////////////////////////////////////////////
            var bindings = keyMapper.GetBindings();
            foreach (var binding in bindings)
            {
                StackPanel bindingStack = new StackPanel()
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Height = 35
                };

                Label lbl = new Label()
                {
                    Text = binding.DisplayName,
                    Width = 480
                };

                Label bindingKeyLabel = new Label()
                {
                    Text = binding.Keys.First().ToString(),
                    HorizontalAlignment = HorizontalAlignment.Right,
                    Width = 90,
                    Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Gray),
                    Tag = new object[] { binding.Id, binding.Keys.First() }
                };
                bindingKeyLabel.LeftMouseClick += BindingKeyLabel_LeftMouseClick;

                bindingStack.Controls.Add(lbl);
                bindingStack.Controls.Add(bindingKeyLabel);
                bindingsStack.Controls.Add(bindingStack);
            }
        }

        private void BindingKeyLabel_LeftMouseClick(Control sender, MouseEventArgs args)
        {
            if (sender.Tag is null)
                return;
            object[] data = (object[])sender.Tag;

            string id = (string)data[0];
            var oldKey = (Keys)data[1];

            var lbl = (Label)sender;

            MessageScreen screen = new MessageScreen(assets, UI.Languages.OctoClient.PressKey, "", UI.Languages.OctoClient.Cancel);
            screen.KeyDown += (s, a) =>
            {
                keyMapper.RemoveKey(id, oldKey);
                keyMapper.AddKey(id, a.Key);
                data[1] = a.Key;
                settings.Set("KeyMapper-" + id, a.Key.ToString());
                lbl.Text = a.Key.ToString();
                ScreenManager.NavigateBack();
            };
            ScreenManager.NavigateToScreen(screen);
        }
    }
}
