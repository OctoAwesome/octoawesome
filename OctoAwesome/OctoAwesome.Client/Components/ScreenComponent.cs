using engenious.UI;
using OctoAwesome.Client.Screens;
using System;
using engenious;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Components;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using OctoAwesome.Rx;
using OctoAwesome.Database;
using System.Collections.Generic;
using OctoAwesome.Extension;
using engenious.UI.Controls;
using OctoAwesome.UI.Screens;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent, IAssetRelatedComponent, IScreenComponent
    {
        private readonly ExtensionService extensionService;
        private readonly IDisposable componentSubscription;

        public new OctoGame Game { get; private set; }

        public PlayerComponent Player { get { return Game.Player; } }

        public CameraComponent Camera => Game.Camera;

        public ComponentList<UIComponent> Components { get; private set; }
        private readonly List<BaseScreen> screens;

        public ScreenComponent(OctoGame game, ExtensionService extensionService) : base(game)
        {
            Game = game;
            TitlePrefix = "OctoAwesome";

            screens = new();
            Components = new ComponentList<UIComponent>(null, null, Add, Remove);

            this.extensionService = extensionService;

            componentSubscription
                = game.ResourceManager
                .UpdateHub
                .ListenOn(DefaultChannels.Simulation)
                .Subscribe(OnNext);

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

        public override void Update(GameTime gameTime)
        {
            foreach (var component in Components)
            {
                component.Update(gameTime);
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
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
            Skin.Current.ButtonBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture("buttonLong_brown"), 15, 15);
            Skin.Current.ButtonHoverBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture("buttonLong_beige"), 15, 15);
            Skin.Current.ButtonPressedBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture("buttonLong_beige_pressed"), 15, 15);
            Skin.Current.ButtonDisabledBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture("buttonLong_brown_disabled"), 15, 15);
            Skin.Current.ProgressBarBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture("progress_red"), 10, 8);
            Skin.Current.HorizontalScrollBackgroundBrush = NineTileBrush.FromSingleTexture(Game.Assets.LoadTexture("progress_background"), 10, 8);
        }


        private void OnNext(Notification notification)
        {
            switch (notification)
            {
                case EntityNotification entityNotification:
                    if (entityNotification.Type == EntityNotification.ActionType.Remove)
                        RemoveEntity(entityNotification.EntityId);
                    else if (entityNotification.Type == EntityNotification.ActionType.Add)
                        Add(entityNotification.Entity);
                    break;
                case FunctionalBlockNotification functionalBlockNotification:
                    if (functionalBlockNotification.Type == FunctionalBlockNotification.ActionType.Add)
                        Add(functionalBlockNotification.Block);
                    break;
                default:
                    break;
            }
        }

        public void Add(BaseScreen screen)
        {
            foreach (var component in Components)
            {
                if (component is not UIComponent uiComponent)
                    continue;

                screen.AddUiComponent(uiComponent);
            }

            screens.Add(screen);
            extensionService.ExecuteExtender(screen);
        }

        public void Remove(BaseScreen screen)
        {
            screens.Remove(screen);
        }

        public void Add(UIComponent uiComponent)
        {
            foreach (var screen in screens)
            {
                screen.AddUiComponent(uiComponent);
            }

            extensionService.ExecuteExtender(uiComponent);
        }

        public void Remove(UIComponent uiComponent)
        {
            foreach (var screen in screens)
            {
                screen.RemoveUiComponent(uiComponent);
            }

        }

        private void Add(ComponentContainer componentContainer)
        {
            foreach (var component in Components)
            {
                if (component is not UIComponent uiComponent)
                    continue;

                uiComponent.Add(componentContainer);
            }
        }

        private void RemoveEntity(Guid entityId)
        {
            //TODO: Remove component container?
        }
    }
}
