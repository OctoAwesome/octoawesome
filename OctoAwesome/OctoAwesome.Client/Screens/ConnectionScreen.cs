using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonoGameUi;
using engenious;
using OctoAwesome.Client.Components;
using OctoAwesome.Runtime;

namespace OctoAwesome.Client.Screens
{
    internal sealed class ConnectionScreen : BaseScreen
    {
        public new ScreenComponent Manager => (ScreenComponent)base.Manager;

        private ISettings settings;
        private OctoGame game;

        public ConnectionScreen(ScreenComponent manager) : base(manager)
        {
            settings = Manager.Game.Settings;
            game = Manager.Game;
            Padding = new Border(0, 0, 0, 0);

            Title = Languages.OctoClient.CreateUniverse;

            SetDefaultBackground();

            var panel = new StackPanel(manager)
            {
                VerticalAlignment = VerticalAlignment.Stretch,
                HorizontalAlignment = HorizontalAlignment.Stretch,
                Margin = Border.All(50),
                Background = new BorderBrush(Color.White * 0.5f),
                Padding = Border.All(10)
            };
            Controls.Add(panel);

            var input = new Textbox(manager)
            {
                Text = "localhost",
                HorizontalAlignment = HorizontalAlignment.Stretch,
                //VerticalAlignment = VerticalAlignment.Stretch,
                Background = new BorderBrush(Color.LightGray, LineType.Solid, Color.Black)
            };
            panel.Controls.Add(input);

            var createButton = Button.TextButton(manager, Languages.OctoClient.Connect);
            createButton.HorizontalAlignment = HorizontalAlignment.Center;
            createButton.VerticalAlignment = VerticalAlignment.Center;
            createButton.Visible = true;
            createButton.LeftMouseClick += (s, e) =>
            {
                game.Settings.Set("server", input.Text);
                ((ContainerResourceManager)game.ResourceManager)
                    .CreateManager(game.ExtensionLoader, game.DefinitionManager, game.Settings, true);
                
                PlayMultiplayer(manager);
            };
            panel.Controls.Add(createButton);

        }

        private void PlayMultiplayer(ScreenComponent manager)
        {
            Manager.Player.SetEntity(null);

            Manager.Game.Simulation.LoadGame(Guid.Empty);
            //settings.Set("LastUniverse", levelList.SelectedItem.Id.ToString());

            Player player = Manager.Game.Simulation.LoginPlayer(Guid.Empty);
            Manager.Game.Player.SetEntity(player);

            Manager.NavigateToScreen(new GameScreen(manager));
        }
    }
}
