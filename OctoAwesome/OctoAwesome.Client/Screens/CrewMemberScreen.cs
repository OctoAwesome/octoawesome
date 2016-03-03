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
    class CrewMemberScreen : BaseScreen
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
                Texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/background_notext.png", manager.GraphicsDevice),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch
            };
            Controls.Add(background);

            //The Panel
            Texture2D panelBackground = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/panel.png", manager.GraphicsDevice);
            Panel panel = new Panel(manager)
            {
                MaxWidth = 750,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = new Border(15, 15, 15, 15),
            };

            Controls.Add(panel);

            //The Vertical Stack - Split the Panel in half Vertical
            StackPanel verticalStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Vertical,
            };
            // panel.Controls.Add(verticalStack);



            //The Main Stack - Split the Panel in half Horizontal
            StackPanel horizontalStack = new StackPanel(manager)
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            panel.Controls.Add(horizontalStack);


            //The Profile Image
            Image profileImage = new Image(manager)
            {
                Height = 200,
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Border(0, 0, 10, 0)
            };
            if (member.PictureFilename == null)
                profileImage.Texture = manager.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/Crew/base.png", manager.GraphicsDevice);
            else profileImage.Texture = manager.Content.LoadTexture2DFromFile(member.PictureFilename, manager.GraphicsDevice);
            horizontalStack.Controls.Add(profileImage);



            //The Text Stack
            StackPanel textStack = new StackPanel(manager);
            textStack.VerticalAlignment = VerticalAlignment.Stretch;
            textStack.HorizontalAlignment = HorizontalAlignment.Left;
            textStack.Width = 430;
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

            foreach (CrewMember.Achievements achievement in member.AchievementList)
            {
                achievementString += " " + achievement.ToString();
                if (member.AchievementList.IndexOf(achievement) != member.AchievementList.Count - 1) achievementString += ", ";
            }
            StackPanel achievementStack = new StackPanel(manager);
            achievementStack.VerticalAlignment = VerticalAlignment.Top;
            achievementStack.HorizontalAlignment = HorizontalAlignment.Left;
            achievementStack.Orientation = Orientation.Horizontal;
            textStack.Controls.Add(achievementStack);

            Label achievementsTitle = new Label(manager) { Text = Languages.OctoClient.Achievements + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };

            Label achievements = new Label(manager) { Text = achievementString, HorizontalAlignment = HorizontalAlignment.Left };

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
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalTextAlignment = HorizontalAlignment.Left,
                VerticalTextAlignment = VerticalAlignment.Top,

            };
            Description.InvalidateDimensions();
            DescriptionPanel.Controls.Add(Description);

            panel.Width = 700;

            //The Back Button
            Button backButton = Button.TextButton(manager, Languages.OctoClient.Back);
            backButton.VerticalAlignment = VerticalAlignment.Top;
            backButton.HorizontalAlignment = HorizontalAlignment.Left;
            backButton.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            backButton.Margin = new Border(10, 10, 10, 10);
            Controls.Add(backButton);

            Title = member.Alias;
        }
    }
}
