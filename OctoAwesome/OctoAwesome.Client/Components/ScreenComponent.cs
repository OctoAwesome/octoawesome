using MonoGameUi;
using OctoAwesome.Client.Screens;
using System;
using engenious;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent
    {
        public new OctoGame Game { get; private set; }

        public PlayerComponent Player { get { return Game.Player; } }

        public CameraComponent Camera { get { return Game.Camera; } }

        public ScreenComponent(OctoGame game) : base(game)
        {
            Game = game;
            TitlePrefix = "OctoAwesome";
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            Skin.Current.ButtonBrush =
                NineTileBrush.FromSingleTexture(
                    Game.Screen.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/buttonLong_brown.png",
                        GraphicsDevice), 15, 15);

            Skin.Current.ButtonHoverBrush =
                NineTileBrush.FromSingleTexture(
                    Game.Screen.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/buttonLong_beige.png",
                        GraphicsDevice), 15, 15);

            Skin.Current.ButtonPressedBrush =
                NineTileBrush.FromSingleTexture(
                    Game.Screen.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/buttonLong_beige_pressed.png",
                        GraphicsDevice), 15, 15);

            Skin.Current.ProgressBarBrush =
                NineTileBrush.FromSingleTexture(
                    Game.Screen.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/progress_red.png",
                        GraphicsDevice), 10, 8);

            Skin.Current.HorizontalScrollBackgroundBrush =
                NineTileBrush.FromSingleTexture(
                    Game.Screen.Content.LoadTexture2DFromFile("./Assets/OctoAwesome.Client/UI/progress_background.png",
                        GraphicsDevice), 10, 8);

            


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
