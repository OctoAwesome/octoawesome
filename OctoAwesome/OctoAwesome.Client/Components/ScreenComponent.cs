using Microsoft.Xna.Framework;
using MonoGameUi;
using OctoAwesome.Client.Screens;
using System;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent
    {
        public PlayerComponent Player { get; private set; }

        public CameraComponent Camera { get; private set; }

        public ScreenComponent(Game game, PlayerComponent player, CameraComponent camera) : base(game)
        {
            Player = player;
            Camera = camera;
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Frame.Background = new BorderBrush(Color.CornflowerBlue);

            NavigateFromTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 0f);
            NavigateToTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 1f);

            NavigateToScreen(new MainScreen(this));
        }

        public void CaptureMouse()
        {
            Player.InputActive = true;
            Game.IsMouseVisible = false;
        }

        public void FreeMouse()
        {
            Player.InputActive = false;
            Game.IsMouseVisible = true;
        }

        

        public void Exit()
        {
            Game.Exit();
        }
    }
}
