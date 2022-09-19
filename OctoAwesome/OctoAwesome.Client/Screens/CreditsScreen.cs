using System.Collections.Generic;
using OctoAwesome.Client.Components;
using engenious.UI;
using OctoAwesome.Client.Crew;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Screens;

namespace OctoAwesome.Client.Screens
{
    internal class CreditsScreen : BaseScreen
    {
        public CreditsScreen(AssetComponent assets)
            : base(assets)
        {
            Padding = new Border(0, 0, 0, 0);

            Title = UI.Languages.OctoClient.CreditsCrew;

            SetDefaultBackground();

            List<CrewMember> crew = CrewMember.GetCrew(assets);

            ScrollContainer crewScroll = new ScrollContainer()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                Margin = new Border(10, 10, 10, 10),
                CanFocus = false
            };

            StackPanel crewList = new StackPanel()
            {
                MinWidth = 700,
                Padding = new Border(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Stretch,
                Orientation = Orientation.Vertical,
            };
            crewScroll.Content = crewList;

            foreach (CrewMember member in crew)
            {
                Button memberButton = new TextButton(member.Username);
                memberButton.HorizontalAlignment = HorizontalAlignment.Stretch;
                memberButton.Margin = new Border(5, 5, 5, 5);

                memberButton.LeftMouseClick += (s, e) =>
                {
                    ScreenManager.NavigateToScreen(new CrewMemberScreen(assets, member));
                };

                crewList.Controls.Add(memberButton);
            }


            Controls.Add(crewScroll);
        }
    }
}
