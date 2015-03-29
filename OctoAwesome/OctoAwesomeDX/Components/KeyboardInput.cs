using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Components
{ 
    /// <summary>
    /// Keyboard-Implementierung der Input-Schnittstelle.
    /// </summary>
    internal sealed class KeyboardInput : IInputSet
    {
        /// <summary>
        /// Anteil der Seitwärtsbewegung (-1...1)
        /// </summary>
        public float MoveX { get; private set; }

        /// <summary>
        /// Anteil der Vorwärtsbewegung (-1...1)
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
        /// Frame Update zur Ermittlung der Veränderungen.
        /// </summary>
        public void Update()
        {
            KeyboardState keyboardState = Keyboard.GetState();

            MoveX = 0f;
            MoveY = 0f;
            HeadX = 0f;
            HeadY = 0f;
            InteractTrigger = keyboardState.IsKeyDown(Keys.E);
            ApplyTrigger = keyboardState.IsKeyDown(Keys.Q);
            JumpTrigger = keyboardState.IsKeyDown(Keys.Space);
            MoveX -= (keyboardState.IsKeyDown(Keys.A) ? 1 : 0);
            MoveX += (keyboardState.IsKeyDown(Keys.D) ? 1 : 0);
            MoveY -= (keyboardState.IsKeyDown(Keys.S) ? 1 : 0);
            MoveY += (keyboardState.IsKeyDown(Keys.W) ? 1 : 0);
            HeadX -= (keyboardState.IsKeyDown(Keys.Left) ? 1 : 0);
            HeadX += (keyboardState.IsKeyDown(Keys.Right) ? 1 : 0);
            HeadY -= (keyboardState.IsKeyDown(Keys.Up) ? 1 : 0);
            HeadY += (keyboardState.IsKeyDown(Keys.Down) ? 1 : 0);
        }
    }
}
