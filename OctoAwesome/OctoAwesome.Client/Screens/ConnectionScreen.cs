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

            //Panel panel = new Panel(manager)
            //{
            //    VerticalAlignment = VerticalAlignment.Stretch,
            //    HorizontalAlignment = HorizontalAlignment.Stretch,
            //    Margin = Border.All(50),
            //    Background = new BorderBrush(Color.White * 0.5f),
            //    Padding = Border.All(10)
            //};
            //Controls.Add(panel);

            var createButton = Button.TextButton(manager, Languages.OctoClient.Connect);
            createButton.HorizontalAlignment = HorizontalAlignment.Center;
            createButton.VerticalAlignment = VerticalAlignment.Center;
            createButton.Visible = true;
            createButton.LeftMouseClick += (s, e) =>
            {
                ((ContainerResourceManager)game.ResourceManager)
                    .CreateManager(game.ExtensionLoader, game.DefinitionManager, game.Settings, true);

                manager.NavigateToScreen(new GameScreen(manager));
            };
            Controls.Add(createButton);
        }
    }
}
