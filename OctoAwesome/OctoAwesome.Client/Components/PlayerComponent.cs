using System;
using engenious;
using engenious.Helper;
using OctoAwesome.Entities;
namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent, IEntityController
    {
        #region IEntityController Interface

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

        public bool[] SlotInput { get; private set; } = new bool[10];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }
        #endregion

        public Vector3 HeadOffset { get; private set; }

        private new OctoGame Game;

        private IResourceManager resourceManager;


        public Entity CurrentEntity { get; private set; }

        //TODO: wieder hinzufügen, oder anders lösen
        //public InventoryComponent Inventory { get; private set; }
        //public ToolBarComponent Toolbar { get; private set; }
        
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

            //if (CurrentEntity != null)
            //{
            //    // Map other Components                
            //    //CurrentEntityHead = CurrentEntity.Components.GetComponent<HeadComponent>();
            //    //if (CurrentEntityHead == null) CurrentEntityHead = new HeadComponent();

            //    // TODO: toolbar und inventory wieder hinzufügen, oder anders lösen
            //    //Inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            //    //if (Inventory == null) Inventory = new InventoryComponent();
            //    //Toolbar = CurrentEntity.Components.GetComponent<ToolBarComponent>();
            //    //if (Toolbar == null) Toolbar = new ToolBarComponent();
            //}
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
            #region Toolbar
            //TODO: wieder hinzufügen, oder anders lösen
            //if (Toolbar.Tools != null && Toolbar.Tools.Length > 0)
            //{
            //    if (Toolbar.ActiveTool == null) Toolbar.ActiveTool = Toolbar.Tools[0];
            //    for (int i = 0; i < Math.Min(Toolbar.Tools.Length, SlotInput.Length); i++)
            //    {
            //        if (SlotInput[i])
            //            Toolbar.ActiveTool = Toolbar.Tools[i];
            //        SlotInput[i] = false;
            //    }
            //}

            //Index des aktiven Werkzeugs ermitteln
            //int activeTool = -1;
            //List<int> toolIndices = new List<int>();
            //if (Toolbar.Tools != null)
            //{
            //    for (int i = 0; i < Toolbar.Tools.Length; i++)
            //    {
            //        if (Toolbar.Tools[i] != null)
            //            toolIndices.Add(i);

            //        if (Toolbar.Tools[i] == Toolbar.ActiveTool)
            //            activeTool = toolIndices.Count - 1;
            //    }
            //}

            //if (SlotLeftInput)
            //{
            //    if (activeTool > -1)
            //        activeTool--;
            //    else if (toolIndices.Count > 0)
            //        activeTool = toolIndices[toolIndices.Count - 1];
            //}
            //SlotLeftInput = false;

            //if (SlotRightInput)
            //{
            //    if (activeTool > -1)
            //        activeTool++;
            //    else if (toolIndices.Count > 0)
            //        activeTool = toolIndices[0];
            //}
            //SlotRightInput = false;

            //if (activeTool > -1)
            //{
            //    activeTool = (activeTool + toolIndices.Count) % toolIndices.Count;
            //    Toolbar.ActiveTool = Toolbar.Tools[toolIndices[activeTool]];
            //}
            #endregion
        }

        /// <summary>
        /// DEBUG METHODE: NICHT FÜR VERWENDUNG IM SPIEL!
        /// </summary>
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
