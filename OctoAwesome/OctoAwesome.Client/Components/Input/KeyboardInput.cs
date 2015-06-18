using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Input
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
        public Trigger<bool> InteractTrigger { get; private set; }

        /// <summary>
        /// Anwendungstrigger (Verwendet das aktuelle Werkzeug auf die markierte Stelle an)
        /// </summary>
        public Trigger<bool> ApplyTrigger { get; private set; }

        /// <summary>
        /// Sprung-Trigger (löst einen Sprung aus)
        /// </summary>
        public Trigger<bool> JumpTrigger { get; private set; }

        public Trigger<bool>[] SlotTrigger { get; private set; }

        public Trigger<bool> SlotLeftTrigger { get; private set; }

        public Trigger<bool> SlotRightTrigger { get; private set; }

        public KeyboardInput()
        {
            InteractTrigger = new Trigger<bool>();
            ApplyTrigger = new Trigger<bool>();
            JumpTrigger = new Trigger<bool>();
            SlotLeftTrigger = new Trigger<bool>();
            SlotRightTrigger = new Trigger<bool>();

            SlotTrigger = new Trigger<bool>[InputComponent.SlotTriggerLength];
            for (int i = 0; i < SlotTrigger.Length; i++)
            {
                SlotTrigger[i] = new Trigger<bool>();
            }
        }

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
            InteractTrigger.Value = keyboardState.IsKeyDown(Keys.E);
            ApplyTrigger.Value = keyboardState.IsKeyDown(Keys.Q);
            JumpTrigger.Value = keyboardState.IsKeyDown(Keys.Space);
            SlotTrigger[0].Value = keyboardState.IsKeyDown(Keys.NumPad1);
            SlotTrigger[1].Value = keyboardState.IsKeyDown(Keys.D2);
            SlotTrigger[2].Value = keyboardState.IsKeyDown(Keys.D3);
            SlotTrigger[3].Value = keyboardState.IsKeyDown(Keys.D4);
            SlotTrigger[4].Value = keyboardState.IsKeyDown(Keys.D5);
            SlotTrigger[5].Value = keyboardState.IsKeyDown(Keys.D6);
            SlotTrigger[6].Value = keyboardState.IsKeyDown(Keys.D7);
            SlotTrigger[7].Value = keyboardState.IsKeyDown(Keys.D8);
            SlotTrigger[8].Value = keyboardState.IsKeyDown(Keys.D9);
            SlotTrigger[9].Value = keyboardState.IsKeyDown(Keys.D0);
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
