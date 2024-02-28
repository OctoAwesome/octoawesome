using engenious.UI;
using OctoAwesome.Client.Components;
using System;
using System.Diagnostics;
using System.Linq;
using engenious;
using engenious.Input;
using engenious.UI.Controls;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Screens;
using OctoAwesome.Location;

namespace OctoAwesome.Client.Screens
{
    internal class LoadScreen : OctoDecoratedScreen
    {
        private readonly Button deleteButton;
        private readonly Button createButton;
        private readonly Button playButton;
        private readonly Grid mainStack;
        private readonly Listbox<IUniverse> levelList;
        private readonly Label seedLabel;

        private readonly ISettings settings;

        public LoadScreen(AssetComponent assets) : base(assets)
        {
            settings = ScreenManager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);

            Title = UI.Languages.OctoClient.SelectUniverse;

            SetDefaultBackground();

            //Main Panel
            mainStack = new Grid();
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            //Level Stack
            levelList = new Listbox<IUniverse>()
            {
                Background = new BorderBrush(Color.White * 0.5f),
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(10),
                SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f)
            };
            levelList.TemplateGenerator += (x) =>
            {
                var li = new Label()
                {
                    Text = $"{x!.Name} ({x.Seed})",
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = Border.All(10),
                };
                li.LeftMouseDoubleClick += (s, e) => Play();
                return li;
            };
            mainStack.AddControl(levelList, 0, 0);

            //Sidebar
            Panel sidebar = new Panel();
            sidebar.Padding = Border.All(20);
            sidebar.VerticalAlignment = VerticalAlignment.Stretch;
            sidebar.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Background = new BorderBrush(Color.White * 0.5f);
            sidebar.Margin = Border.All(10);
            mainStack.AddControl(sidebar, 1, 0);

            //Universe Info
            seedLabel = new Label();
            seedLabel.Text = "";
            seedLabel.VerticalAlignment = VerticalAlignment.Top;
            seedLabel.HorizontalAlignment = HorizontalAlignment.Left;
            sidebar.Controls.Add(seedLabel);

            //Buttons
            StackPanel buttonStack = new StackPanel();
            buttonStack.VerticalAlignment = VerticalAlignment.Bottom;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            deleteButton = GetButton(UI.Languages.OctoClient.Delete);
            deleteButton.Enabled = false;
            buttonStack.Controls.Add(deleteButton);
            deleteButton.LeftMouseClick += (s, e) =>
            {
                // Ensure that the universe is not loaded.
                Debug.Assert(levelList.SelectedItem != null, "levelList.SelectedItem != null");
                if (ScreenManager.Game.ResourceManager.CurrentUniverse != null &&
                    ScreenManager.Game.ResourceManager.CurrentUniverse.Id == levelList.SelectedItem.Id)
                    return;

                ScreenManager.Game.ResourceManager.DeleteUniverse(levelList.SelectedItem.Id);
                levelList.Items.Remove(levelList.SelectedItem);
                levelList.SelectedItem = null;
                levelList.InvalidateDimensions();
                settings.Set("LastUniverse", "");
            };


            levelList.SelectedItemChanged += (s, e) =>
            {
                seedLabel.Text = "";
                if (levelList.SelectedItem != null)
                {
                    seedLabel.Text = "Seed: " + levelList.SelectedItem.Seed;
                    deleteButton.Enabled = true;
                }
                else
                {
                    deleteButton.Enabled = false;
                }
            };

            createButton = GetButton(UI.Languages.OctoClient.Create);
            createButton.LeftMouseClick += (s, e) => ScreenManager.NavigateToScreen(new CreateUniverseScreen(assets));
            buttonStack.Controls.Add(createButton);

            playButton = GetButton(UI.Languages.OctoClient.Play);
            playButton.LeftMouseClick += (s, e) =>
            {
                if (levelList.SelectedItem == null)
                {
                    MessageScreen msg = new MessageScreen(assets, UI.Languages.OctoClient.Error, UI.Languages.OctoClient.SelectUniverseFirst);
                    ScreenManager.NavigateToScreen(msg);

                    return;
                }

                Play();
            };
            buttonStack.Controls.Add(playButton);

            foreach (var universe in ScreenManager.Game.ResourceManager.ListUniverses())
                levelList.Items.Add(universe);

            // Select first item, or if available - the last played universe
            if (levelList.Items.Count >= 1)
                levelList.SelectedItem = levelList.Items[0];

            Guid lastUniverseId;
            if (Guid.TryParse(settings.Get<string>("LastUniverse"), out lastUniverseId))
            {
                var lastlevel = levelList.Items.FirstOrDefault(u => u.Id == lastUniverseId);
                if (lastlevel != null)
                    levelList.SelectedItem = lastlevel;

            }
        }


        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key == Keys.Enter)
            {
                if (levelList.SelectedItem == null)
                    return;

                Play();

                base.OnKeyDown(args);
            }
        }

        private void Play()
        {
            Debug.Assert(levelList.SelectedItem != null, "levelList.SelectedItem != null");

            ScreenManager.Game.GameService.StartSinglePlayer(levelList.SelectedItem.Id);
            ScreenManager.NavigateToScreen(new LoadingScreen(Assets));
        }
    }
}
