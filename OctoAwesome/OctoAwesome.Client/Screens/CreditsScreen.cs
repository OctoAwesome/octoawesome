using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctoAwesome.Client.Components;
using MonoGameUi;

namespace OctoAwesome.Client.Screens
{
    class CreditsScreen : Screen
    {
        String selectedItem;
        ScreenComponent Manager;
        public CreditsScreen(ScreenComponent manager) :base(manager)
        {
            Manager = manager;

            Padding = new Border(0, 0, 0, 0);

            Image background = new Image(manager);
            background.Texture = Manager.Content.Load<Texture2D>("Textures/background_notext");
            background.VerticalAlignment = VerticalAlignment.Stretch;
            background.HorizontalAlignment = HorizontalAlignment.Stretch;
            Controls.Add(background);

            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            List<CrewMember> crew = CrewMember.getCrew(manager.Content);

            ScrollContainer crewScroll = new ScrollContainer(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Border(10, 10, 10, 10),
                CanFocus = false
            };

            StackPanel crewList = new StackPanel(manager) {
                MinWidth = 700,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Vertical,
            };
            crewScroll.Content = crewList;

            foreach(CrewMember member in crew)
            {
                Panel memberPanel = new Panel(manager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    MinHeight = 30,
                    Background = new BorderBrush(Color.White),
                    Margin = new Border(5, 5, 5, 5),
                    HoveredBackground = new BorderBrush(Color.LightGray)

                };

                memberPanel.LeftMouseClick += (s, e) =>
                {
                    manager.NavigateToScreen(new CrewMemberScreen(manager, member));
                };

                Label name = new Label(manager)
                {
                    Text = member.Username,
                    VerticalAlignment = VerticalAlignment.Stretch,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Padding = new Border(5, 5, 5, 5)
                };

                memberPanel.Controls.Add(name);
                crewList.Controls.Add(memberPanel);

            }
            

            Controls.Add(crewScroll);
        }

        private void CrewList_LeftMouseClick(Control sender, MouseEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
