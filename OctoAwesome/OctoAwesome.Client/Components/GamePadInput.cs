using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components
{
    /// <summary>
    /// Gamepad Implementierung der Input-Schnittstelle.
    /// </summary>
    internal class GamePadInput : IInputSet
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
        /// Frame-Update zur Ermittlung der Veränderungen.
        /// </summary>
        public void Update()
        {
            try
            {
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

                InteractTrigger = gamePadState.Buttons.X == ButtonState.Pressed;
                ApplyTrigger = gamePadState.Buttons.A == ButtonState.Pressed;
                JumpTrigger = gamePadState.Buttons.Y == ButtonState.Pressed;
                SlotLeftTrigger = gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                SlotRightTrigger = gamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                MoveX = gamePadState.ThumbSticks.Left.X;
                MoveY = gamePadState.ThumbSticks.Left.Y;
                HeadX = gamePadState.ThumbSticks.Right.X;
                HeadY = gamePadState.ThumbSticks.Right.Y;
            }
            catch (Exception) { }
        }

        public bool[] SlotTrigger { get { return null; } }

        public bool SlotLeftTrigger { get; private set; }

        public bool SlotRightTrigger { get; private set; }
    }
}
