using MonoGameUi;
using OctoAwesome.Client.Screens;
using System;
using engenious;
using System.Collections.Generic;
using engenious.Graphics;
using OctoAwesome.Common;
using System.Reflection;
using System.Linq;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent, IAssetRelatedComponent, IUserInterfaceExtensionManager
    {
        public new OctoGame Game { get; private set; }

        public PlayerComponent Player { get { return Game.Player; } }

        public CameraComponent Camera { get { return Game.Camera; } }

        public IDefinitionManager DefinitionManager => Game.DefinitionManager;

        public IEnumerable<Func<Control>> GameScreenExtension { get { return gamescreenextension; } }
        public IEnumerable<Func<Control>> InventoryScreenExtension { get { return inventoryscreenextension; } }
        public IEnumerable<(bool right, Func<string> updatefunc)> DebugControlExtensions { get { return debugscreenextensions; } }

        private List<Func<Control>> gamescreenextension;
        private List<Func<Control>> inventoryscreenextension;
        private List<(bool, Func<string>)> debugscreenextensions;

        public ScreenComponent(OctoGame game) : base(game)
        {
            Game = game;
            TitlePrefix = "OctoAwesome";

            gamescreenextension = new List<Func<Control>>();
            inventoryscreenextension = new List<Func<Control>>();
            debugscreenextensions = new List<(bool, Func<string>)>();
        }
        protected override void LoadContent()
        {
            base.LoadContent();

            ReloadAssets();

            Frame.Background = new BorderBrush(Color.CornflowerBlue);

            NavigateFromTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 0f);
            NavigateToTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 1f);

            NavigateToScreen(new MainScreen(this));


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
            debugscreenextensions.Clear();
        }

        #region IUserinterfaceManager
        public bool RegisterOnGameScreen(Type controltype, params object[] args)
        {
            if (Check(controltype, args, out object[] parameter))
            {
                gamescreenextension.Add(() => InternalCreate(controltype, parameter));
                return true;
            }
            return false;
        }
        public bool RegisterOnInventoryScreen(Type controltype, params object[] args)
        {
            if (Check(controltype, args, out object[] parameter))
            {
                inventoryscreenextension.Add(() => InternalCreate(controltype, parameter));
                return true;
            }
            return false;
        }
        public bool RegisterOnDebugScreen(bool right, Func<string> updatefunc)
        {
            if (updatefunc != null)
            {
                debugscreenextensions.Add((right, updatefunc));
                return true;
            }
            return false;
        }
        public Texture2D LoadTextures(Type type, string key)
        {
            return Game.Assets.LoadTexture(type, key);
        }
        private bool Check(Type controltype, object[] inputargs, out object[] args)
        {
            args = null;

            if (controltype.IsAbstract)
                return false;

            if (!typeof(Control).IsAssignableFrom(controltype))
                return false;

            args = new object[inputargs.Length + 1];
            args[0] = this;
            Array.Copy(inputargs, 0, args, 1, inputargs.Length);

            return true;

            //ConstructorInfo[] contructors = controltype.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
            //foreach(ConstructorInfo constructor in contructors)
            //{
            //    ParameterInfo[] parameters = constructor.GetParameters();
            //    if (parameters.Length == inputargs.Length + 1)
            //    {
            //        if (!Check(parameters, inputargs))
            //            continue;

            //        args = new object[parameters.Length];
            //        args[0] = this;
            //        Array.Copy(inputargs, 0, args, 1, inputargs.Length);
            //        return true;
            //    }
            //    else if (parameters.Length > inputargs.Length + 1)
            //    {
            //        if (!Check(parameters, args))
            //            continue;

            //        int offset = inputargs.Length + 1;
            //        args = new object[parameters.Length];
            //        args[0] = this;
            //        Array.Copy(inputargs, 0, args, 1, inputargs.Length);
            //        for (int i = offset; i < parameters.Length; i++)
            //        {
            //            if (parameters[i].HasDefaultValue)
            //                args[i] = parameters[i].DefaultValue;
            //            else break;

            //        }

            //        return true;
            //    }
            //}
            //return false;
        }
        private bool Check(ParameterInfo[] parameters, object[] args)
        {
            if (!typeof(BaseScreenComponent).IsAssignableFrom(parameters[0].ParameterType))
                return false;

            for (int i = 0; i < args.Length; i++)
            {
                if (!parameters[i + 1].ParameterType.IsAssignableFrom(args[i].GetType()))
                {
                    return false;
                }
            }
            return true;
        }
        private Control InternalCreate(Type controltype, object[] args)
        {
            try
            {
                return (Control) Activator.CreateInstance(controltype, args);
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
