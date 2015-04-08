using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
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
        public bool InteractTrigger { get; private set; }

        /// <summary>
        /// Anwendungstrigger (Verwendet das aktuelle Werkzeug auf die markierte Stelle an)
        /// </summary>
        public bool ApplyTrigger { get; private set; }

        /// <summary>
        /// Sprung-Trigger (löst einen Sprung aus)
        /// </summary>
        public bool JumpTrigger { get; private set; }

        /// <summary>
        /// Initialisierung. Benötigt eine Game-Instanz zur Ermittlung der Fenstergröße
        /// </summary>
        /// <param name="game">Referenz auf das aktuelle Game</param>
        public MouseInput(Game game)
        {
            this.game = game;
        }

        /// <summary>
        /// Frame Update zur Ermittung der Veränderungen.
        /// </summary>
        public void Update()
        {
            MouseState state = Mouse.GetState();

            InteractTrigger = state.RightButton == ButtonState.Pressed;
            ApplyTrigger = state.LeftButton == ButtonState.Pressed;

            int centerX = game.GraphicsDevice.Viewport.Width / 2;
            int centerY = game.GraphicsDevice.Viewport.Height / 2;
            Mouse.SetPosition(centerX, centerY);

            if (init)
            {
                float deltaX = state.Position.X - centerX;
                float deltaY = state.Position.Y - centerY;

                HeadX = deltaX * mouseSpeed;
                HeadY = -deltaY * mouseSpeed;
            }
            init = true;            
        }
    }
}
