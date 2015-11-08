using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OctoAwesome.Client.Components.Input
{
    /// <summary>
    /// Maus-Implementierung der Input-Schnittstelle.
    /// </summary>
    internal sealed class MouseInput : IInputSet
    {
        /// <summary>
        /// Geschwindigkeitsmultiplikator für die Kopfbewegung-
        /// </summary>
        private float mouseSpeed = 0.2f;

        private int lastWheelState = 0;

        private Game game;

        private bool init = false;

        /// <summary>
        /// Anteil der Seitwärtsbewegung (-1...1). Keine Verwendung in der Maus
        /// </summary>
        public float MoveX { get; private set; }

        /// <summary>
        /// Anteil der Vorwärtsbewegung (-1...1). Keine Verwendung in der Maus
        /// </summary>
        public float MoveY { get; private set; }

        /// <summary>
        /// Kopfbewegung Drehung (-1...1)
        /// </summary>
        public float HeadX { get; private set; }

        /// <summary>
        /// Kopfbewegung Neigung (-1...1)
        /// </summary>
        public float HeadY { get; private set; }

        /// <summary>
        /// Interaktionstrigger (löst eine Interaktion mit dem markierten Element aus)
        /// </summary>
        public Trigger<bool> InteractTrigger { get; private set; }

        /// <summary>
        /// Anwendungstrigger (Verwendet das aktuelle Werkzeug auf die markierte Stelle an)
        /// </summary>
        public Trigger<bool> ApplyTrigger { get; private set; }

        public Trigger<bool> InventoryTrigger { get; private set; }

        /// <summary>
        /// Sprung-Trigger (löst einen Sprung aus)
        /// </summary>
        public Trigger<bool> JumpTrigger { get; private set; }

        public Trigger<bool> ToggleFlyMode { get; private set; }

        public Trigger<bool>[] SlotTrigger { get { return null; } }

        public Trigger<bool> SlotLeftTrigger { get; private set; }

        public Trigger<bool> SlotRightTrigger { get; private set; }

        /// <summary>
        /// Initialisierung. Benötigt eine Game-Instanz zur Ermittlung der Fenstergröße
        /// </summary>
        /// <param name="game">Referenz auf das aktuelle Game</param>
        public MouseInput(Game game)
        {
            this.game = game;

            InteractTrigger = new Trigger<bool>();
            ApplyTrigger = new Trigger<bool>();
            InventoryTrigger = new Trigger<bool>();
            JumpTrigger = new Trigger<bool>();
            SlotLeftTrigger = new Trigger<bool>();
            SlotRightTrigger = new Trigger<bool>();
            ToggleFlyMode = new Trigger<bool>();
        }

        /// <summary>
        /// Frame Update zur Ermittung der Veränderungen.
        /// </summary>
        public void Update()
        {
            MouseState state = Mouse.GetState();

            //InteractTrigger.Value = state.RightButton == ButtonState.Pressed;
            //ApplyTrigger.Value = state.LeftButton == ButtonState.Pressed;

            //int centerX = game.GraphicsDevice.Viewport.Width / 2;
            //int centerY = game.GraphicsDevice.Viewport.Height / 2;
            //Mouse.SetPosition(centerX, centerY);

            //if (init)
            //{
            //    float deltaX = state.Position.X - centerX;
            //    float deltaY = state.Position.Y - centerY;

            //    HeadX = deltaX * mouseSpeed;
            //    HeadY = -deltaY * mouseSpeed;
            //}
            //init = true;

            SlotLeftTrigger.Value = state.ScrollWheelValue < lastWheelState;
            SlotRightTrigger.Value = state.ScrollWheelValue > lastWheelState;
            lastWheelState = state.ScrollWheelValue;
        }
    }
}
