using System;
using System.Collections.Generic;
using engenious;
using engenious.Graphics;
using engenious.Helper;
using MonoGameUi;
using OctoAwesome.Entities;
namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent, IEntityController, IUserInterfaceManager
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
        
        public Vector3 Direction { get; set; }

        public Index3? SelectedBlock { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }
                
        #endregion

        #region External Inputs
        public Vector2 HeadInput { get; set; }

        public Vector2 MoveInput { get; set; }

        public bool FlymodeInput { get; set; }
        #endregion

        public Vector3 HeadOffset { get; private set; }
        public Entity CurrentEntity { get; private set; }

        public IEnumerable<Func<Control>> GameScreenExtension { get { return gamescreenextension.Values; } }

        public IEnumerable<Func<Control>> InventoryScreenExtension { get { return inventoryscreenextension.Values; } }

        private new OctoGame Game;

        private IResourceManager resourceManager;

        private Dictionary<Type, Func<Control>> gamescreenextension;
        private Dictionary<Type, Func<Control>> inventoryscreenextension;

        public PlayerComponent(OctoGame game, IResourceManager resourceManager) : base(game)
        {
            this.resourceManager = resourceManager;
            Game = game;
            gamescreenextension = new Dictionary<Type, Func<Control>>();
            inventoryscreenextension = new Dictionary<Type, Func<Control>>();
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
                {
                    if (comp is IUserInterfaceExtension extension)
                        extension.Register(this);
                }
            }
            else
            {
                gamescreenextension.Clear();
                inventoryscreenextension.Clear();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled || CurrentEntity == null)
                return;

            Yaw += (float) gameTime.ElapsedGameTime.TotalSeconds * HeadInput.X;
            Tilt = Math.Min(1.5f, Math.Max(-1.5f, Tilt + (float) gameTime.ElapsedGameTime.TotalSeconds * HeadInput.Y));

            // calculation of motion direction
            float lookX = (float) Math.Cos(Yaw);
            float lookY = -(float) Math.Sin(Yaw);
            Direction = new Vector3(lookX, lookY, 0) * MoveInput.Y;

            float stafeX = (float) Math.Cos(Yaw + MathHelper.PiOver2);
            float stafeY = -(float) Math.Sin(Yaw + MathHelper.PiOver2);
            Direction += new Vector3(stafeX, stafeY, 0) * MoveInput.X;

            MoveInput = Vector2.Zero;
            HeadInput = Vector2.Zero;

            //TODO: was ist damit
            //if (FlymodeInput)
            //    ActorHost.Player.FlyMode = !ActorHost.Player.FlyMode;
            //FlymodeInput = false;
        }

        /// <summary>
        /// DEBUG METHODE: NICHT FÜR VERWENDUNG IM SPIEL!
        /// </summary>
        [Obsolete("Is Empty right now... TODO: implementieren")]
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
            catch
            {
                return null;
            }
        }
        #endregion
    }
}
