using engenious;
using engenious.UI;
using engenious.UI.Controls;

using OctoAwesome.Client.Screens;
using OctoAwesome.Client.UI.Components;
using OctoAwesome.UI.Components;
using OctoAwesome.Database;
using OctoAwesome.Extension;
using OctoAwesome.Notifications;
using OctoAwesome.Runtime;
using OctoAwesome.Rx;
using OctoAwesome.UI.Screens;

using System;
using System.Collections.Generic;

namespace OctoAwesome.Client.Components
{
    internal sealed class ScreenComponent : BaseScreenComponent, IAssetRelatedComponent, IScreenComponent
    {
        private readonly ExtensionService extensionService;
        private readonly IDisposable componentSubscription;

        public new OctoGame Game { get; private set; }

        public PlayerComponent Player => Game.Player;

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
                = game
                .ResourceManager
                .UpdateHub
                .ListenOn(DefaultChannels.UI)
                .Subscribe(OnNext);

        }


        /// <inheritdoc/>
        protected override void LoadContent()
        {
            base.LoadContent();

            ReloadAssets();

            Frame.Background = new BorderBrush(Color.CornflowerBlue);

            NavigateFromTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 0f);
            NavigateToTransition = new AlphaTransition(Frame, Transition.Linear, TimeSpan.FromMilliseconds(200), 1f);

            this.extensionService.ExecuteExtender(this);

            NavigateToScreen(new MainScreen(this));

        }

        /// <inheritdoc/>
        public override void Update(GameTime gameTime)
        {
            foreach (var component in Components)
            {
                component.Update(gameTime);
            }

            base.Update(gameTime);
        }
        /// <inheritdoc/>
        public void Exit() => Game.Exit();

        /// <inheritdoc/>
        public void UnloadAssets()
        {
            Skin.Current.ButtonBrush = default!;
            Skin.Current.ButtonHoverBrush = default!;
            Skin.Current.ButtonPressedBrush = default!;
            Skin.Current.ProgressBarBrush = default!;
            Skin.Current.HorizontalScrollBackgroundBrush = default!;
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <inheritdoc/>
        public void Remove(BaseScreen screen) => screens.Remove(screen);

        /// <summary>
        /// Adds the ui component the all screen that can accept it and calls the execute extender on it
        /// </summary>
        /// <param name="uiComponent">The component to add to screens and extend</param>
        public void Add(UIComponent uiComponent)
        {
            foreach (var screen in screens)
            {
                screen.AddUiComponent(uiComponent);
            }

            extensionService.ExecuteExtender(uiComponent);
        }

        /// <summary>
        /// Removes the ui component from all the screens that accepted this component before
        /// </summary>
        /// <param name="uiComponent">The componen to remove from the component</param>
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
