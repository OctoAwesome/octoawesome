using MonoGameUi;
using OctoAwesome.Client.Screens;
using System;
using engenious;
using System.Collections.Generic;
using engenious.Graphics;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent, IAssetRelatedComponent, IUserInterfaceExtensionManager
    {
        public new OctoGame Game { get; private set; }

        public PlayerComponent Player { get { return Game.Player; } }

        public CameraComponent Camera { get { return Game.Camera; } }


        public IEnumerable<Func<Control>> GameScreenExtension { get { return gamescreenextension.Values; } }

        public IEnumerable<Func<Control>> InventoryScreenExtension { get { return inventoryscreenextension.Values; } }

        private Dictionary<Type, Func<Control>> gamescreenextension;
        private Dictionary<Type, Func<Control>> inventoryscreenextension;

        public ScreenComponent(OctoGame game) : base(game)
        {
            Game = game;
            TitlePrefix = "OctoAwesome";
        }

        protected override void LoadContent()
        {
            base.LoadContent();

            ReloadAssets();

            Frame.Background = new BorderBrush(Color.CornflowerBlue);

            NavigateFromTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 0f);
            NavigateToTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 1f);

            NavigateToScreen(new MainScreen(this));

            gamescreenextension = new Dictionary<Type, Func<Control>>();
            inventoryscreenextension = new Dictionary<Type, Func<Control>>();


        }

        public void Exit()
        {
            Game.Exit();
        }

        public void UnloadAssets()
        {
            Skin.Current.ButtonBrush = null;
            Skin.Current.ButtonHoverBrush = null;
            Skin.Current.ButtonPressedBrush = null;
            Skin.Current.ProgressBarBrush = null;
            Skin.Current.HorizontalScrollBackgroundBrush = null;
        }

        public void ReloadAssets()
        {
            Skin.Current.ButtonBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture(typeof(ScreenComponent), "buttonLong_brown"), 15, 15);
            Skin.Current.ButtonHoverBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige"), 15, 15);
            Skin.Current.ButtonPressedBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture(typeof(ScreenComponent), "buttonLong_beige_pressed"), 15, 15);
            Skin.Current.ProgressBarBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture(typeof(ScreenComponent), "progress_red"), 10, 8);
            Skin.Current.HorizontalScrollBackgroundBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture(typeof(ScreenComponent), "progress_background"), 10, 8);
        }

        internal void CleanExtensions()
        {
            gamescreenextension.Clear();
            inventoryscreenextension.Clear();
        }

        #region IUserinterfaceManager
        public bool RegisterOnGameScreen(Type controltype, params object[] args)
        {
            if (!gamescreenextension.TryGetValue(controltype, out Func<Control> extension))
            {
                gamescreenextension.Add(controltype, () => InternalCreate(controltype, args));
                return true;
            }
            return false;
        }
        public bool RegisterOnInventoryScreen(Type controltype, params object[] args)
        {
            if (!inventoryscreenextension.TryGetValue(controltype, out Func<Control> extension))
            {
                inventoryscreenextension.Add(controltype, () => InternalCreate(controltype, args));
                return true;
            }
            return false;
        }
        public Texture2D LoadTextures(Type type, string key)
        {
            return Game.Assets.LoadTexture(type, key);
        }
        private Control InternalCreate(Type controltype, object[] args)
        {
            object[] constructorargs = new object[2 + args.Length];
            constructorargs[0] = Game.Screen;
            constructorargs[1] = this;
            Array.Copy(args, 0, constructorargs, 2, args.Length);
            try
            {
                return (Control) Activator.CreateInstance(controltype, constructorargs);
            }
            catch (Exception exception)
            {
                //TODO: Loggen
                return null;
            }
        }
        #endregion
    }
}
