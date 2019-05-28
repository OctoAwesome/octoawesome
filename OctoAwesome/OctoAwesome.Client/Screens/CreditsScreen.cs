using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OctoAwesome.Client.Components;
using engenious.UI;
using engenious.UI.Controls;
using engenious;
using OctoAwesome.Client.Crew;

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
                Padding = new Border(10, 10, 10, 10),
                CanFocus = false
            };

            StackPanel crewList = new StackPanel(manager) {
                MinWidth = 700,
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Vertical,
                Padding = new Border(0,0,10,0)
            };
            crewScroll.Content = crewList;

            foreach(CrewMember member in crew)
            {
                Button memberButton = new TextButton(manager, member.Username);
                memberButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                memberButton.Margin = new Border(5, 5, 5, 5);

                memberButton.LeftMouseClick += (s, e) =>
                {
                    manager.NavigateToScreen(new CrewMemberScreen(manager, member));
                };

                crewList.Controls.Add(memberButton);
            }
            

            Controls.Add(crewScroll);
        }
    }
}
