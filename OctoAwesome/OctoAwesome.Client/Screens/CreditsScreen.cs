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

            Listbox<string> crewList = new Listbox<string>(manager);
            crewList.VerticalAlignment = VerticalAlignment.Stretch;
            crewList.Background = new BorderBrush(new Color(Color.Gray, 0.4f));
            crewList.Padding = new Border(10, 10, 10, 10);
            crewList.Margin = new Border(20, 20, 20, 20);
            crewList.Height = MaxHeight - 10;
            crewList.SelectedItemBrush = new BorderBrush(Color.GhostWhite);
            crewList.TemplateGenerator = (item) =>
            {
                return new Label(manager) { Text = item, Width = 600, Padding = new Border(5,5,5,5)};
            };

            crewList.LeftMouseClick += (s, e) =>
            {
                if (crewList.SelectedItem != null)
                {
                    manager.NavigateToScreen(new CrewMemberScreen(manager, crew.First(item => item.Username == crewList.SelectedItem)));
                }
            };
            
            foreach(CrewMember member in crew)
            {
                crewList.Items.Add(member.Username);
            }

            Controls.Add(crewList);
        }

        private void CrewList_LeftMouseClick(Control sender, MouseEventArgs args)
        {
            throw new NotImplementedException();
        }
    }
}
