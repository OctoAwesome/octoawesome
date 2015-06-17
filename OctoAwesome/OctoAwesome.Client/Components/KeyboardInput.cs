using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{ 
    /// <summary>
    /// Keyboard-Implementierung der Input-Schnittstelle.
    /// </summary>
    internal sealed class KeyboardInput : IInputSet
    {
        bool[] slotTriggers = new bool[10];

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

        public bool[] SlotTrigger { get { return slotTriggers; } }

        public bool SlotLeftTrigger { get; private set; }

        public bool SlotRightTrigger { get; private set; }

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
            slotTriggers[0] = keyboardState.IsKeyDown(Keys.D1);
            slotTriggers[1] = keyboardState.IsKeyDown(Keys.D2);
            slotTriggers[2] = keyboardState.IsKeyDown(Keys.D3);
            slotTriggers[3] = keyboardState.IsKeyDown(Keys.D4);
            slotTriggers[4] = keyboardState.IsKeyDown(Keys.D5);
            slotTriggers[5] = keyboardState.IsKeyDown(Keys.D6);
            slotTriggers[6] = keyboardState.IsKeyDown(Keys.D7);
            slotTriggers[7] = keyboardState.IsKeyDown(Keys.D8);
            slotTriggers[8] = keyboardState.IsKeyDown(Keys.D9);
            slotTriggers[9] = keyboardState.IsKeyDown(Keys.D0);
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
