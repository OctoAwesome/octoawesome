using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctoAwesome.Client.Components;
using MonoGameUi;
using engenious;

namespace OctoAwesome.Client.Screens
{
    class CreditsScreen : BaseScreen
    {
        public CreditsScreen(ScreenComponent manager) : base(manager)
        {
            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.CreditsCrew;

            SetDefaultBackground();

            List<CrewMember> crew = CrewMember.getCrew(manager);

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
    }
}
