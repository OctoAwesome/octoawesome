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
            IsOverlay = true;
            //Height = 500;
            Width = 1000;
            VerticalAlignment = VerticalAlignment.Center;
            HorizontalAlignment = HorizontalAlignment.Center;

            SpriteFont boldFont = manager.Content.Load<SpriteFont>("BoldFont");

            Padding = new Border(20, 20, 20, 20);

            Texture2D panelBackground = manager.Content.Load<Texture2D>("Textures/panel");
            Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30);

            //The Main Stack - Split the Panel in half
            StackPanel helperStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Vertical,
                Padding = new Border(20, 20, 20, 20)
            };
            Controls.Add(helperStack);

            //The Main Stack - Split the Panel in half
            StackPanel mainStack = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Orientation = Orientation.Horizontal,
                MaxHeight = 300,
            };
            helperStack.Controls.Add(mainStack);

            //The Profile Image
            Image profileImage = new Image(manager);
            if (member.Picture == null)
                profileImage.Texture = manager.Content.Load<Texture2D>("Textures/Crew/Base");
            else profileImage.Texture = member.Picture;
            profileImage.MaxHeight = 300;
            profileImage.MaxWidth = 300;
            profileImage.VerticalAlignment = VerticalAlignment.Top;
            mainStack.Controls.Add(profileImage);

            //The Text Stack
            StackPanel textStack = new StackPanel(manager);
            textStack.VerticalAlignment = VerticalAlignment.Stretch;
            textStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.Controls.Add(textStack);

            //The Username
            Label username = new Label(manager) { Text = member.Username };
            username.Font = manager.Content.Load<SpriteFont>("HeadlineFont");
            username.HorizontalAlignment = HorizontalAlignment.Left;
            username.VerticalAlignment = VerticalAlignment.Top;
            textStack.Controls.Add(username);

            //The Alias
            Label alias = new Label(manager) { Text = "AKA: " + member.Alias, HorizontalAlignment = HorizontalAlignment.Left };
            textStack.Controls.Add(alias);

            //The Description
            Label desc = new Label(manager) { Text = member.Description, WordWrap = true, VerticalAlignment = VerticalAlignment.Center, HorizontalAlignment = HorizontalAlignment.Left};
            helperStack.Controls.Add(desc);

            string achievementString = "";

            foreach(CrewMember.Achievements achievement in member.AchievementList)
            {
                achievementString += " " + achievement.ToString() + ",";
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

            ///Close Button
            Label close = new Label(manager) { Text = " x" };
            close.VerticalAlignment = VerticalAlignment.Top;
            close.HorizontalAlignment = HorizontalAlignment.Right;
            close.LeftMouseClick += (s, e) =>
            {
                manager.NavigateBack();
            };
            close.Background = new BorderBrush(Color.Red);
            close.Height = 30;
            close.Width = 30;
            Controls.Add(close);
        }
    }
}
