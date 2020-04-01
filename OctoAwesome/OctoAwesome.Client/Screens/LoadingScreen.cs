using engenious;
using engenious.Input;
using MonoGameUi;
using OctoAwesome.Client.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OctoAwesome.Client.Screens
{
    internal sealed class LoadingScreen : BaseScreen
    {
        public LoadingScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new Border(0, 0, 0, 0);

            Title = "Loading";

            SetDefaultBackground();

            //Main Panel
            var mainStack = new Grid(manager);
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 4 });
            mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);
            var backgroundStack = new Panel(manager)
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10)
            };
            mainStack.AddControl(backgroundStack, 0, 0, 1, 1);

            var text = new Label(manager)
            {
                Text = "Konfuzius sagt: Das mag ich nicht!",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Padding = Border.All(10),
            };
            backgroundStack.Controls.Add(text);
            //Buttons
            StackPanel buttonStack = new StackPanel(manager);
            buttonStack.VerticalAlignment = VerticalAlignment.Center;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Center;
            buttonStack.Orientation = Orientation.Horizontal;
            backgroundStack.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            var cancelButton = GetButton(Languages.OctoClient.Cancel);
            buttonStack.Controls.Add(cancelButton);
            cancelButton.LeftMouseClick += (s, e) =>
            {
            };
        }
    }
}
