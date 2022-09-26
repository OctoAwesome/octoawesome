using System;
using System.Diagnostics;
using System.Linq;

using engenious;

using OctoAwesome.EntityComponents;
using OctoAwesome.Extension;
using OctoAwesome.SumTypes;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent
    {

        private readonly IResourceManager resourceManager;

        #region External Input

        public Vector2 HeadInput { get; set; }

        public Vector2 MoveInput { get; set; }

        public bool InteractInput { get; set; }

        public bool ApplyInput { get; set; }

        public bool JumpInput { get; set; }

        public bool FlymodeInput { get; set; }

        public bool[] SlotInput { get; } = new bool[10];

        public bool SlotLeftInput { get; set; }

        public bool SlotRightInput { get; set; }

        #endregion


        private HeadComponent? currentEntityHead;
        private ControllableComponent? currentController;
        private InventoryComponent? inventory;
        private ToolBarComponent? toolbar;
        private PositionComponent? position;
        private Entity? currentEntity;

        public Entity CurrentEntity
            => NullabilityHelper.NotNullAssert(currentEntity, $"{nameof(CurrentEntity)} was not initialized!");

        public HeadComponent CurrentEntityHead
            => NullabilityHelper.NotNullAssert(currentEntityHead, $"{nameof(CurrentEntityHead)} was not initialized!");

        public ControllableComponent CurrentController
            => NullabilityHelper.NotNullAssert(currentController, $"{nameof(CurrentController)} was not initialized!");

        public InventoryComponent Inventory
            => NullabilityHelper.NotNullAssert(inventory, $"{nameof(Inventory)} was not initialized!");

        public ToolBarComponent Toolbar
            => NullabilityHelper.NotNullAssert(toolbar, $"{nameof(Toolbar)} was not initialized!");

        public PositionComponent Position
            => NullabilityHelper.NotNullAssert(position, $"{nameof(Position)} was not initialized!");

        // public ActorHost ActorHost { get; private set; }
        public Selection? Selection { get; set; }
        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public PlayerComponent(OctoGame game, IResourceManager resourceManager)
            : base(game)
        {
            this.resourceManager = resourceManager;
            Enabled = false;
        }

        public void Unload()
        {
            Enabled = false;
            currentEntity = null;
            currentEntityHead = null;
            Selection = null;
            SelectedBox = null;
            SelectedPoint = null;
        }

        public void Load(Entity entity)
        {
            // Map other Components

            currentEntity = entity;

            var controlComp = entity.Components.GetComponent<ControllableComponent>();

            Debug.Assert(controlComp != null, nameof(controlComp) + " != null");
            currentController = controlComp;

            currentEntityHead = entity.Components.GetComponent<HeadComponent>();
            if (currentEntityHead is null)
                currentEntityHead = new() { Offset = new(0, 0, 3.2f) };

            inventory = entity.Components.GetComponent<InventoryComponent>();
            if (inventory is null)
                inventory = new();

            toolbar = entity.Components.GetComponent<ToolBarComponent>();
            if (toolbar is null)
                toolbar = new();

            position = entity.Components.GetComponent<PositionComponent>();
            if (position is null)
                position = new() { Position = new Coordinate(0, new Index3(0, 0, 0), new Vector3(0, 0, 0)) };
            
            
            Enabled = true;
        }

        public override void Update(GameTime gameTime)
        {
            CurrentEntityHead.Angle += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.X;
            CurrentEntityHead.Tilt += (float)gameTime.ElapsedGameTime.TotalSeconds * HeadInput.Y;
            CurrentEntityHead.Tilt = Math.Min(1.5f, Math.Max(-1.5f, CurrentEntityHead.Tilt));
            HeadInput = Vector2.Zero;

            CurrentController.MoveInput = MoveInput;
            MoveInput = Vector2.Zero;

            CurrentController.JumpInput = JumpInput;
            JumpInput = false;

            if (InteractInput && SelectedBox.HasValue )
                CurrentController.Selection = Selection;
            else
                CurrentController.Selection = null;

            if (InteractInput && SelectedBox.HasValue)
                CurrentController.InteractBlock = SelectedBox.Value;
            else
                CurrentController.InteractBlock = null;

            if (ApplyInput && SelectedBox.HasValue)
            {
                CurrentController.ApplyBlock = SelectedBox.Value;
                CurrentController.ApplySide = SelectedSide;
            }

            ApplyInput = false;
            //if (FlymodeInput)
            //    ActorHost.Player.FlyMode = !ActorHost.Player.FlyMode;
            //FlymodeInput = false;

            if (Toolbar.Tools.Length > 0)
            {
                for (int i = 0; i < Math.Min(Toolbar.Tools.Length, SlotInput.Length); i++)
                {
                    if (SlotInput[i])
                        Toolbar.ActiveIndex = i;
                    SlotInput[i] = false;
                }
            }

            // Determine index of the active tool
            if (SlotLeftInput)
            {
                Toolbar.ActiveIndex--;
            }
            SlotLeftInput = false;

            if (SlotRightInput)
            {
                Toolbar.ActiveIndex++;
            }
            SlotRightInput = false;


        }

        /// <summary>
        /// DEBUG METHOD: NOT FOR USAGE IN GAME!
        /// </summary>
        internal void AllBlocksDebug()
        {
            var inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            if (inventory == null)
                return;

            var blockDefinitions = resourceManager.DefinitionManager.BlockDefinitions;
            foreach (var blockDefinition in blockDefinitions)
                inventory.Add(blockDefinition, blockDefinition.VolumePerUnit);

        }

        internal void AllFoodsDebug()
        {
            var inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            if (inventory == null)
                return;

            var itemDefinitions = resourceManager.DefinitionManager.ItemDefinitions;
            var foodMaterial = resourceManager.DefinitionManager.FoodDefinitions.FirstOrDefault();
            if (foodMaterial is null)
                return;
            foreach (var itemDefinition in itemDefinitions)
            {
                var fooditem = itemDefinition.Create(foodMaterial);
                if (fooditem is not null)
                    inventory.Add(fooditem, fooditem.VolumePerUnit);
            }

        }

        internal void AllItemsDebug()
        {
            var inventory = CurrentEntity.Components.GetComponent<InventoryComponent>();
            if (inventory == null)
                return;

            var itemDefinitions = resourceManager.DefinitionManager.ItemDefinitions;
            var wood = resourceManager.DefinitionManager.MaterialDefinitions.FirstOrDefault(d => d.DisplayName == "Wood");
            var stone = resourceManager.DefinitionManager.MaterialDefinitions.FirstOrDefault(d => d.DisplayName == "Stone");
            foreach (var itemDefinition in itemDefinitions)
            {
                if (wood is not null && itemDefinition.Create(wood) is { } woodItem)
                    inventory.Add(woodItem, woodItem.VolumePerUnit);
                
                if (stone is not null && itemDefinition.Create(stone) is { } stoneItem)
                    inventory.Add(stoneItem, stoneItem.VolumePerUnit);

            }

        }

    }
}
