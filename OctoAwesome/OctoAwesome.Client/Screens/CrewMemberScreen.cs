using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MonoGameUi;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OctoAwesome.Client.Components;

namespace OctoAwesome.Client.Screens
{
    class CrewMemberScreen : Screen
    {
        public CrewMemberScreen(ScreenComponent manager, CrewMember member) : base(manager)
        {
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            SpriteFont boldFont = manager.Content.Load<SpriteFont>("BoldFont");

            Padding = new Border(0, 0, 0, 0);

            //The Background Image
            Image background = new Image(manager)
            {
                Texture = manager.Content.Load<Texture2D>("Textures/background_notext"),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Controls.Add(background);

            //The Panel
            Texture2D panelBackground = manager.Content.Load<Texture2D>("Textures/panel");
            Panel panel = new Panel(manager)
            {
                Width = 1000,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
            };

            Controls.Add(panel);
            
            //The Vertical Stack - Split the Panel in half Vertical
            StackPanel verticalStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Vertical,
                Padding = new Border(20, 20, 20, 20),
            };
            panel.Controls.Add(verticalStack);

            

                //The Main Stack - Split the Panel in half Horizontal
                StackPanel horizontalStack = new StackPanel(manager)
                {
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Orientation = Orientation.Horizontal,
                    MaxHeight = 300,
                };
                verticalStack.Controls.Add(horizontalStack);


                    //The Profile Image
                    Image profileImage = new Image(manager)
                    {
                        MaxHeight = 200,
                        MaxWidth = 200,
                        VerticalAlignment = VerticalAlignment.Top
                    };  
                    if (member.Picture == null)
                        profileImage.Texture = manager.Content.Load<Texture2D>("Textures/Crew/Base");
                    else profileImage.Texture = member.Picture;
                    horizontalStack.Controls.Add(profileImage);


                    //The Text Stack
                    StackPanel textStack = new StackPanel(manager);
                    textStack.VerticalAlignment = VerticalAlignment.Stretch;
                    textStack.HorizontalAlignment = HorizontalAlignment.Stretch;
                    horizontalStack.Controls.Add(textStack);

                        //The Username
                        Label username = new Label(manager)
                        {
                            Text = member.Username,
                            Font = manager.Content.Load<SpriteFont>("HeadlineFont"),
                            HorizontalAlignment = HorizontalAlignment.Left,
                            VerticalAlignment = VerticalAlignment.Top
                        };
                        textStack.Controls.Add(username);

                        //The Alias
                        Label alias = new Label(manager)
                        {
                            Text = member.Alias,
                            HorizontalAlignment = HorizontalAlignment.Left
                        };
                        textStack.Controls.Add(alias);

                        //Achievements
                        string achievementString = "";

                        foreach(CrewMember.Achievements achievement in member.AchievementList)
                        {
                            achievementString += " " + achievement.ToString();
                            if (member.AchievementList.IndexOf(achievement) != member.AchievementList.Count - 1) achievementString += ", ";
                        }
                        StackPanel achievementStack = new StackPanel(manager);
                        achievementStack.VerticalAlignment = VerticalAlignment.Top;
                        achievementStack.HorizontalAlignment = HorizontalAlignment.Left;
                        achievementStack.Orientation = Orientation.Horizontal;
                        textStack.Controls.Add(achievementStack);

                        Label achievementsTitle = new Label(manager) { Text = "Achievements: ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };

                        Label achievements = new Label(manager) { Text = achievementString , HorizontalAlignment = HorizontalAlignment.Left};

                        achievementStack.Controls.Add(achievementsTitle);
                        achievementStack.Controls.Add(achievements);

            Panel DescriptionPanel = new Panel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            textStack.Controls.Add(DescriptionPanel);

            Label Description = new Label(manager)
            {
                Text = member.Description,
                WordWrap = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalTextAlignment = VerticalAlignment.Top,
                Width = 700
            };
            Description.InvalidateDimensions();
            DescriptionPanel.Controls.Add(Description);

            //The Back Button
            Button backButton = Button.TextButton(manager, "Back");
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);
        }
    }
}
