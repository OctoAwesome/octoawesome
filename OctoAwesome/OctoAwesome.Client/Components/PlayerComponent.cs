using Microsoft.Xna.Framework;
using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    internal sealed class PlayerComponent : GameComponent
    {
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

        public bool SprintInput { get; set; }

        public bool CrouchInput { get; set; }

        #endregion

        public IPlayerController PlayerController { get; private set; }

        public Index3? SelectedBox { get; set; }

        public Vector2? SelectedPoint { get; set; }

        public OrientationFlags SelectedSide { get; set; }

        public OrientationFlags SelectedEdge { get; set; }

        public OrientationFlags SelectedCorner { get; set; }

        public List<InventorySlot> Tools { get; set; }

        public InventorySlot ActiveTool { get; set; }

        public PlayerComponent(Game game, IPlayerController playerController)
            : base(game)
        {
            PlayerController = playerController;
        }

        public override void Initialize()
        {
            base.Initialize();
            Tools = new List<InventorySlot>();
        }

        public override void Update(GameTime gameTime)
        {
            Tools.Clear();
            Tools.AddRange(PlayerController.Inventory);

            PlayerController.Head = HeadInput;
            HeadInput = Vector2.Zero;

            PlayerController.Move = MoveInput;
            MoveInput = Vector2.Zero;

            if (JumpInput)
                PlayerController.Jump();
            JumpInput = false;

            if (InteractInput && SelectedBox.HasValue)
                PlayerController.Interact(SelectedBox.Value);
            InteractInput = false;

            if (ApplyInput && SelectedBox.HasValue)
                PlayerController.Apply(SelectedBox.Value, ActiveTool, SelectedSide);
            ApplyInput = false;

            if (FlymodeInput)
                PlayerController.FlyMode = !PlayerController.FlyMode;
            FlymodeInput = false;

            PlayerController.Sprint = SprintInput;
            PlayerController.Crouch = CrouchInput;

            if (Tools != null && Tools.Count > 0)
            {
                if (ActiveTool == null) ActiveTool = Tools[0];
                for (int i = 0; i < Math.Min(Tools.Count, SlotInput.Length); i++)
                {
                    if (SlotInput[i])
                        ActiveTool = Tools[i];
                    SlotInput[i] = false;
                }
            }

            // Index des aktiven Werkzeugs ermitteln
            int activeTool = -1;
            if (Tools != null && ActiveTool != null)
            {
                for (int i = 0; i < Tools.Count; i++)
                {
                    if (Tools[i] == ActiveTool)
                    {
                        activeTool = i;
                        break;
                    }
                }
            }

            if (activeTool > -1)
            {
                if (SlotLeftInput)
                    activeTool--;
                SlotLeftInput = false;

                if (SlotRightInput)
                    activeTool++;
                SlotRightInput = false;

                activeTool = (activeTool + Tools.Count) % Tools.Count;
                ActiveTool = Tools[activeTool];
            }
        }
    }
}