using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Screens
{
    internal class LoadScreen : BaseScreen
    {
        private new ScreenComponent Manager;

        Button deleteButton, createButton, playButton;
        Grid mainStack;
        Listbox<IUniverse> levelList;

        private ISettings settings;

        public LoadScreen(ScreenComponent manager) : base(manager)
        {
            Manager = manager;
            settings = manager.Game.Settings;

            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.SelectUniverse;

            SetDefaultBackground();

            //Main Panel
            mainStack = new Grid(manager);
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 3 });
            mainStack.Columns.Add(new ColumnDefinition() { ResizeMode = ResizeMode.Parts, Width = 1 });
            mainStack.Rows.Add(new RowDefinition() { ResizeMode = ResizeMode.Parts, Height = 1 });
            mainStack.Margin = Border.All(50);
            mainStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            mainStack.VerticalAlignment = VerticalAlignment.Stretch;

            Controls.Add(mainStack);

            //Level Stack
            levelList = new Listbox<IUniverse>(manager);
            levelList.Background = new BorderBrush(Color.White * 0.5f);
            levelList.VerticalAlignment = VerticalAlignment.Stretch;
            levelList.HorizontalAlignment = HorizontalAlignment.Stretch;
            levelList.Margin = Border.All(10);
            levelList.SelectedItemBrush = new BorderBrush(Color.SaddleBrown * 0.7f);
            levelList.TemplateGenerator += (x) =>
            {
                return new Label(manager)
                {
                    Text = string.Format("{0} ({1})", x.Name, x.Seed),
                    HorizontalAlignment = HorizontalAlignment.Stretch,
                    Padding = Border.All(10),
                };
            };
            mainStack.AddControl(levelList, 0, 0);

            //Sidebar
            Panel sidebar = new Panel(manager);
            sidebar.Padding = Border.All(20);
            sidebar.VerticalAlignment = VerticalAlignment.Stretch;
            sidebar.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Background = new BorderBrush(Color.White * 0.5f);
            sidebar.Margin = Border.All(10);
            mainStack.AddControl(sidebar, 1, 0);

            //Universe Info
            Label l = new Label(manager);
            l.Text = " Placeholder ";
            l.VerticalAlignment = VerticalAlignment.Top;
            l.HorizontalAlignment = HorizontalAlignment.Left;
            sidebar.Controls.Add(l);

            //Buttons
            StackPanel buttonStack = new StackPanel(manager);
            buttonStack.VerticalAlignment = VerticalAlignment.Bottom;
            buttonStack.HorizontalAlignment = HorizontalAlignment.Stretch;
            sidebar.Controls.Add(buttonStack);

            //renameButton = getButton("Rename");
            //buttonStack.Controls.Add(renameButton);

            deleteButton = GetButton(Languages.OctoClient.Delete);
            buttonStack.Controls.Add(deleteButton);
            deleteButton.LeftMouseClick += (s, e) =>
            {
                if (levelList.SelectedItem == null)
                {
                    MessageScreen msg = new MessageScreen(manager, Languages.OctoClient.Error, Languages.OctoClient.SelectUniverseFirst);
                    manager.NavigateToScreen(msg);

                    return;
                }

                // Sicherstellen, dass universe nicht geladen ist
                if (ResourceManager.Instance.CurrentUniverse != null && 
                    ResourceManager.Instance.CurrentUniverse.Id == levelList.SelectedItem.Id)
                    return;

                ResourceManager.Instance.DeleteUniverse(levelList.SelectedItem.Id);
                levelList.Items.Remove(levelList.SelectedItem);
                levelList.SelectedItem = null;
                levelList.InvalidateDimensions();
                settings.Set("LastUniverse", "");
            };

            createButton = GetButton(Languages.OctoClient.Create);
            createButton.LeftMouseClick += (s, e) => manager.NavigateToScreen(new CreateUniverseScreen(manager));
            buttonStack.Controls.Add(createButton);

            playButton = GetButton(Languages.OctoClient.Play);
            playButton.LeftMouseClick += (s, e) =>
            {
                if (levelList.SelectedItem == null)
                {
                    MessageScreen msg = new MessageScreen(manager, Languages.OctoClient.Error, Languages.OctoClient.SelectUniverseFirst);
                    manager.NavigateToScreen(msg);
                    
                    return;
                }

                Play();
            };
            buttonStack.Controls.Add(playButton);

            foreach (var universe in ResourceManager.Instance.ListUniverses())
                levelList.Items.Add(universe);

            // Erstes Element auswählen, oder falls vorhanden das letzte gespielte Universum
            if (levelList.Items.Count >= 1)
                levelList.SelectedItem = levelList.Items[0];

            if (settings.KeyExists("LastUniverse") && settings.Get<string>("LastUniverse") != null
                && settings.Get<string>("LastUniverse") != "")
            {
                var lastlevel =  levelList.Items.FirstOrDefault(u => u.Id == Guid.Parse(settings.Get<string>("LastUniverse")));
                if (lastlevel != null)
                    levelList.SelectedItem = lastlevel;

            }
        }

        private Button GetButton(string title)
        {
            Button button = Button.TextButton(Manager, title);
            button.HorizontalAlignment = HorizontalAlignment.Stretch;
            return button;
        }

        protected override void OnKeyDown(KeyEventArgs args)
        {
            if (args.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
            {
                if (levelList.SelectedItem == null)
                    return;

                Play();

                base.OnKeyDown(args);
            }
        }

        private void Play()
        {
            Manager.Player.RemovePlayer();
            Manager.Game.Simulation.LoadGame(levelList.SelectedItem.Id);
            settings.Set("LastUniverse", levelList.SelectedItem.Id.ToString());
            Manager.Game.Player.InsertPlayer();
            Manager.NavigateToScreen(new GameScreen(Manager));
        }
    }
}
