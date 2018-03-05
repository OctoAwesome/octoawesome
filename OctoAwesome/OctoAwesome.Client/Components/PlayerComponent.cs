﻿using System;
using engenious;
using engenious.Helper;
using OctoAwesome.Common;
using OctoAwesome.Entities;
namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent, IEntityController
    {
        #region IEntityController Interface

        public bool[] SlotInput { get; private set; } = new bool[10];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }

        public bool InteractInput { get; set; }

        public bool ApplyInput { get; set; }

        public bool JumpInput { get; set; }

        public float Tilt { get; set; }

        public float Yaw { get; set; }

        public float Roll { get; set; }

        public Vector2 HeadInput { get; set; }

        public Vector2 MoveInput { get; set; }

        public Vector3 Direction { get; set; }

        public Index3? SelectedBlock { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        // suggestion: kamere freezen -> sodass sie nicht mehr bewegt werden kann.
        public bool CanFreeze => false;

        public bool Freezed { get; set; }

        #endregion

        #region External Inputs        

        public bool FlymodeInput { get; set; }

        #endregion

        public Vector3 HeadOffset { get; private set; }

        public Entity CurrentEntity { get; private set; }
        
        private new OctoGame Game;

        private IResourceManager resourceManager;


        public PlayerComponent(OctoGame game, IResourceManager resourceManager) : base(game)
        {
            this.resourceManager = resourceManager;
            Game = game;
        }

        public void SetEntity(Entity entity)
        {
            CurrentEntity = entity;
            if (CurrentEntity != null && CurrentEntity is IControllable current)
                current.Reset();
            if (entity is IControllable controllable)
                controllable.Register(this);
            if (CurrentEntity is Entities.IDrawable draw)
                HeadOffset = new Vector3(0, 0, draw.Height - 0.3f);
            else HeadOffset = new Vector3(0, 0, 3.2f);

            if (CurrentEntity != null)
            {
                foreach(Entities.EntityComponent comp in entity.Components)
                    if (comp is IUserInterfaceExtension extension)
                        extension.Register(Game.Screen);
            }
            else
            {
                Game.Screen.CleanExtensions();
            }
        }        

        //public override void Update(GameTime gameTime)
        //{
        //    if (!Enabled || CurrentEntity == null)
        //        return;
            
        //    //TODO: was ist damit
        //    //if (FlymodeInput)
        //    //    ActorHost.Player.FlyMode = !ActorHost.Player.FlyMode;
        //    //FlymodeInput = false;
        //}

        /// <summary>
        /// DEBUG METHODE: NICHT FÜR VERWENDUNG IM SPIEL!
        /// </summary>
        [Obsolete("Is Empty right now... TODO: implementieren, but how")]
        internal void AllBlocksDebug()
        {
            //var inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            //if (inventory == null)
            //    return;

            //var blockDefinitions = resourceManager.DefinitionManager.GetBlockDefinitions();
            //foreach (var blockDefinition in blockDefinitions)
            //    inventory.AddUnit(blockDefinition);

            //var itemDefinitions = resourceManager.DefinitionManager.GetItemDefinitions();
            //foreach (var itemDefinition in itemDefinitions)
            //    inventory.AddUnit(itemDefinition);
        }
    }
}
