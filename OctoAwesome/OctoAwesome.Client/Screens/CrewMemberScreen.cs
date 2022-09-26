using System;
using System.Diagnostics;
using System.Linq;
using engenious.UI;
using OctoAwesome.Client.Components;
using engenious.Graphics;
using OctoAwesome.Client.Crew;
using engenious.UI.Controls;
using OctoAwesome.UI.Components;
using OctoAwesome.UI.Screens;
using OctoAwesome.Client.UI.Components;

namespace OctoAwesome.Client.Screens
{
    internal class CrewMemberScreen : OctoDecoratedScreen
    {

        public CrewMemberScreen(AssetComponent assets, CrewMember member)
            : base(assets)
        {
            VerticalAlignment = VerticalAlignment.Stretch;
            HorizontalAlignment = HorizontalAlignment.Stretch;

            Title = UI.Languages.OctoClient.CreditsCrew + ": " + member.Username;

            SpriteFont boldFont = Skin.Current.BoldFont;

            Padding = new Border(0, 0, 0, 0);

            SetDefaultBackground();

            //The Panel
            var panelBackground = assets.LoadTexture("panel");
            Debug.Assert(panelBackground != null, nameof(panelBackground) + " != null");
            Panel panel = new Panel()
            {
                MaxWidth = 750,
                Background = NineTileBrush.FromSingleTexture(panelBackground, 30, 30),
                Padding = new Border(15, 15, 15, 15),
            };
            Controls.Add(panel);

            //The Main Stack - Split the Panel in half Horizontal
            StackPanel horizontalStack = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                Orientation = Orientation.Horizontal
            };
            panel.Controls.Add(horizontalStack);


            //The Profile Image
            Image profileImage = new Image()
            {
                Height = 200,
                Width = 200,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Padding = new Border(0, 0, 10, 0)
            };
            profileImage.Texture = assets.LoadTexture(typeof(CrewMember), $"Crew.{member.PictureFilename ?? "base"}");
            horizontalStack.Controls.Add(profileImage);

            //The Text Stack
            StackPanel textStack = new StackPanel();
            textStack.VerticalAlignment = VerticalAlignment.Stretch;
            textStack.HorizontalAlignment = HorizontalAlignment.Left;
            textStack.Width = 430;
            horizontalStack.Controls.Add(textStack);

            //The Username & Alias
            string usernameText = member.Username;
            if (member.Alias != member.Username)
                usernameText += " (" + member.Alias + ")";
            Label username = new Label()
            {
                Text = usernameText,
                Font = Skin.Current.HeadlineFont,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };
            textStack.Controls.Add(username);

            //Achievements
            string achievementString = string.Join(", ", member.AchievementList.Select(a => a.ToString()));

            StackPanel achievementStack = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal,
            };
            textStack.Controls.Add(achievementStack);

            Label achievementsTitle = new Label() { Text = UI.Languages.OctoClient.Achievements + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };
            achievementStack.Controls.Add(achievementsTitle);
            Label achievements = new Label() { Text = achievementString, HorizontalAlignment = HorizontalAlignment.Left };
            achievementStack.Controls.Add(achievements);

            // Links
            string linkString = string.Join(", ", member.Links.Select(a => a.Title));

            StackPanel linkStack = new StackPanel()
            {
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                Orientation = Orientation.Horizontal,
            };
            textStack.Controls.Add(linkStack);

            Label linkTitle = new Label() { Text = UI.Languages.OctoClient.Links + ": ", Font = boldFont, HorizontalAlignment = HorizontalAlignment.Left };
            linkStack.Controls.Add(linkTitle);

            foreach (var link in member.Links)
            {
                if (CheckHttpUrl(link.Url))
                {
                    Button linkButton = new TextButton(link.Title);
                    linkButton.LeftMouseClick += (s, e) => UI.Tools.OpenUrl(link.Url);
                    linkStack.Controls.Add(linkButton);
                }
            }

            Panel descriptionPanel = new Panel()
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
            };
            textStack.Controls.Add(descriptionPanel);

            Label description = new Label()
            {
                Text = member.Description,
                WordWrap = true,
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Left,
                HorizontalTextAlignment = HorizontalAlignment.Left,
                VerticalTextAlignment = VerticalAlignment.Top,
            };
            description.InvalidateDimensions();
            descriptionPanel.Controls.Add(description);

            panel.Width = 700;
        }

        private bool CheckHttpUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var tmp) && (tmp.Scheme == Uri.UriSchemeHttp || tmp.Scheme == Uri.UriSchemeHttps);
        }
    }
}
