using System;
using engenious;
using OctoAwesome.Entities;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent, IController
    {
        private new OctoGame Game;

        private IResourceManager resourceManager;

        public Vector3 HeadOffset { get; set; }

        #region IController Interface

        public float HeadTilt { get; set; }

        public float HeadYaw { get; set; }

        public InputTrigger<bool> InteractInput { get; }

        public InputTrigger<bool> ApplyInput { get; }

        public InputTrigger<bool> JumpInput { get; }

        public Vector2 HeadValue { get; set; }

        public Vector2 MoveValue { get; set; }

        public Index3? InteractBlock { get; set; }

        public Index3? ApplyBlock { get; set; }

        public OrientationFlags? ApplySide { get; set; }

        #endregion

        #region External Inputs
        public bool FlymodeInput { get; set; }

        public bool[] SlotInput { get; private set; } = new bool[10];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }
        #endregion

        public Entity CurrentEntity { get; private set; }

        //TODO: wieder hinzufügen, oder anders lösen
        //public InventoryComponent Inventory { get; private set; }
        //public ToolBarComponent Toolbar { get; private set; }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public PlayerComponent(OctoGame game, IResourceManager resourceManager) : base(game)
        {
            this.resourceManager = resourceManager;
            ApplyInput = new InputTrigger<bool>();
            InteractInput = new InputTrigger<bool>();
            JumpInput = new InputTrigger<bool>();
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
                HeadOffset = draw.Body;
            else HeadOffset = new Vector3(0, 0, 3.2f);

            //CurrentEntityHead.Angle += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.X;
            //CurrentEntityHead.Tilt += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.Y;
            //CurrentEntityHead.Tilt = Math.Min(1.5f, Math.Max(-1.5f, CurrentEntityHead.Tilt));

            if (CurrentEntity != null)
            {
                // Map other Components                
                //CurrentEntityHead = CurrentEntity.Components.GetComponent<HeadComponent>();
                //if (CurrentEntityHead == null) CurrentEntityHead = new HeadComponent();

                // TODO: toolbar und inventory wieder hinzufügen, oder anders lösen
                //Inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
                //if (Inventory == null) Inventory = new InventoryComponent();
                //Toolbar = CurrentEntity.Components.GetComponent<ToolBarComponent>();
                //if (Toolbar == null) Toolbar = new ToolBarComponent();
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled || CurrentEntity == null)
                return;

            HeadYaw += (float) gameTime.ElapsedGameTime.TotalSeconds * HeadValue.X;
            HeadTilt = Math.Min(1.5f, Math.Max(-1.5f, HeadTilt + (float) gameTime.ElapsedGameTime.TotalSeconds * HeadValue.X));

            if (InteractInput.Value && SelectedBox.HasValue)
            {
                InteractBlock = SelectedBox.Value;
            }

            if (ApplyInput.Value && SelectedBox.HasValue)
            {
                ApplyBlock = SelectedBox.Value;
                ApplySide = SelectedSide;
            }

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
