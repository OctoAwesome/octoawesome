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
    internal class CrewMemberScreen : BaseScreen
    {
        private AssetComponent assets;

        public CrewMemberScreen(ScreenComponent manager, CrewMember member) : base(manager)
        {
            assets = manager.Game.Assets;

            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            Title = Languages.OctoClient.CreditsCrew + ": " + member.Username;

            SpriteFont boldFont = manager.Content.Load<SpriteFont>("BoldFont");            

            Padding = new Border(0, 0, 0, 0);

            SetDefaultBackground();

            //The Panel
            Texture2D panelBackground = assets.LoadTexture(typeof(ScreenComponent), "panel");
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
                profileImage.Texture = assets.LoadTexture(typeof(ScreenComponent), "base");
            else profileImage.Texture = assets.LoadTexture(typeof(ScreenComponent), member.PictureFilename);
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

        }
    }
}
