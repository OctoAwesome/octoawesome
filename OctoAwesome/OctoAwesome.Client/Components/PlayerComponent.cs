using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using engenious;
using OctoAwesome.EntityComponents;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent
    {
        private new OctoGame Game;

        private IResourceManager resourceManager;

        #region External Input

        public Vector2 HeadInput { get; set; }

        public Vector2 MoveInput { get; set; }

        public bool InteractInput { get; set; }

        public bool ApplyInput { get; set; }

        public bool JumpInput { get; set; }

        public bool FlymodeInput { get; set; }

        public bool[] SlotInput { get; private set; } = new bool[10];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }

        #endregion

        public Entity CurrentEntity { get; private set; }

        public HeadComponent CurrentEntityHead { get; private set; }

        public ControllableComponent CurrentController { get; private set; }

        public InventoryComponent Inventory { get; private set; }

        public ToolBarComponent Toolbar { get; private set; }

        public PositionComponent Position { get; private set; }

        // public ActorHost ActorHost { get; private set; }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public PlayerComponent(OctoGame game, IResourceManager resourceManager)
            : base(game)
        {
            this.resourceManager = resourceManager;
            Game = game;
        }

        public void SetEntity(Entity entity)
        {
            CurrentEntity = entity;



            if (CurrentEntity == null)
            {
                CurrentEntityHead = null;
            }
            else
            {
                // Map other Components

                CurrentController = entity.Components.GetComponent<ControllableComponent>();

                CurrentEntityHead = CurrentEntity.Components.GetComponent<HeadComponent>();
                if (CurrentEntityHead == null) CurrentEntityHead = new HeadComponent();

                Inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
                if (Inventory == null) Inventory = new InventoryComponent();

                Toolbar = CurrentEntity.Components.GetComponent<ToolBarComponent>();
                if (Toolbar == null) Toolbar = new ToolBarComponent();

                Position = CurrentEntity.Components.GetComponent<PositionComponent>();
                if (Position == null) Position = new PositionComponent() { Position = new Coordinate(0, new Index3(0, 0, 0), new Vector3(0, 0, 0)) };
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (!Enabled)
                return;

            if (CurrentEntity == null)
                return;

            CurrentEntityHead.Angle += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.X;
            CurrentEntityHead.Tilt += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.Y;
            CurrentEntityHead.Tilt = Math.Min(1.5f, Math.Max(-1.5f, CurrentEntityHead.Tilt));
            HeadInput = Vector2.Zero;

            CurrentController.MoveInput = MoveInput;
            MoveInput = Vector2.Zero;

            CurrentController.JumpInput = JumpInput;
            JumpInput = false;

            if (InteractInput && SelectedBox.HasValue)
                CurrentController.InteractBlock = SelectedBox.Value;
            InteractInput = false;

            if (ApplyInput && SelectedBox.HasValue)
            {
                CurrentController.ApplyBlock = SelectedBox.Value;
                CurrentController.ApplySide = SelectedSide;
            }

            ApplyInput = false;

            //if (FlymodeInput)
            //    ActorHost.Player.FlyMode = !ActorHost.Player.FlyMode;
            //FlymodeInput = false;

            if (Toolbar.Tools != null && Toolbar.Tools.Length > 0)
            {
                if (Toolbar.ActiveTool == null) Toolbar.ActiveTool = Toolbar.Tools[0];
                for (int i = 0; i < Math.Min(Toolbar.Tools.Length, SlotInput.Length); i++)
                {
                    if (SlotInput[i])
                        Toolbar.ActiveTool = Toolbar.Tools[i];
                    SlotInput[i] = false;
                }
            }

            //Index des aktiven Werkzeugs ermitteln
            int activeTool = -1;
            List<int> toolIndices = new List<int>();
            if (Toolbar.Tools != null)
            {
                for (int i = 0; i < Toolbar.Tools.Length; i++)
                {
                    if (Toolbar.Tools[i] != null)
                        toolIndices.Add(i);

                    if (Toolbar.Tools[i] == Toolbar.ActiveTool)
                        activeTool = toolIndices.Count - 1;
                }
            }

            if (SlotLeftInput)
            {
                if (activeTool > -1)
                    activeTool--;
                else if (toolIndices.Count > 0)
                    activeTool = toolIndices[toolIndices.Count - 1];
            }
            SlotLeftInput = false;

            if (SlotRightInput)
            {
                if (activeTool > -1)
                    activeTool++;
                else if (toolIndices.Count > 0)
                    activeTool = toolIndices[0];
            }
            SlotRightInput = false;

            if (activeTool > -1)
            {
                activeTool = (activeTool + toolIndices.Count) % toolIndices.Count;
                Toolbar.ActiveTool = Toolbar.Tools[toolIndices[activeTool]];
            }
        }

        /// <summary>
        /// DEBUG METHODE: NICHT FÜR VERWENDUNG IM SPIEL!
        /// </summary>
        internal void AllBlocksDebug()
        {
            var inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            if (inventory == null)
                return;

            var blockDefinitions = resourceManager.DefinitionManager.GetBlockDefinitions();
            foreach (var blockDefinition in blockDefinitions)
            {

                var slot = inventory.Inventory.Where(s => s.Definition == blockDefinition && s.Amount < blockDefinition.StackLimit).FirstOrDefault();

                // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
                if (slot == null || slot.Amount >= blockDefinition.StackLimit)
                {
                    slot = new InventorySlot()
                    {
                        Definition = blockDefinition,
                        Amount = 0
                    };
                    inventory.Inventory.Add(slot);
                }
                slot.Amount++;
            }

            var itemDefinitions = resourceManager.DefinitionManager.GetItemDefinitions();
            foreach (var blockDefinition in itemDefinitions)
            {

                var slot = inventory.Inventory.Where(s => s.Definition == blockDefinition && s.Amount < blockDefinition.StackLimit).FirstOrDefault();

                // Wenn noch kein Slot da ist oder der vorhandene voll, dann neuen Slot
                if (slot == null || slot.Amount >= blockDefinition.StackLimit)
                {
                    slot = new InventorySlot()
                    {
                        Definition = blockDefinition,
                        Amount = 0
                    };
                    inventory.Inventory.Add(slot);
                }
                slot.Amount++;
            }

        }
    }
}
