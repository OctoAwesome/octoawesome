using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Screens;
using System;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent
    {
        public new OctoGame Game { get; private set; }

        public PlayerComponent Player { get; private set; }

        public CameraComponent Camera { get; private set; }

        public ScreenComponent(OctoGame game, PlayerComponent player, CameraComponent camera) : base(game)
        {
            Game = game;
            Player = player;
            Camera = camera;
            TitlePrefix = "OctoAwesome";
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Frame.Background = new BorderBrush(Color.CornflowerBlue);

            NavigateFromTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 0f);
            NavigateToTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 1f);

            NavigateToScreen(new MainScreen(this));
        }

        public void Exit()
        {
            Game.Exit();
        }
    }
}
