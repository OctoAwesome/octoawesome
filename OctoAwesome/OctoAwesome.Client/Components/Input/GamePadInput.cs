using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Input
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
        public Trigger<bool> InteractTrigger { get; private set; }

        /// <summary>
        /// Anwendungstrigger (Verwendet das aktuelle Werkzeug auf die markierte Stelle an)
        /// </summary>
        public Trigger<bool> ApplyTrigger { get; private set; }

        /// <summary>
        /// Sprung-Trigger (löst einen Sprung aus)
        /// </summary>
        public Trigger<bool> JumpTrigger { get; private set; }

        public Trigger<bool>[] SlotTrigger { get { return null; } }

        public Trigger<bool> SlotLeftTrigger { get; private set; }

        public Trigger<bool> SlotRightTrigger { get; private set; }

        public GamePadInput()
        {
            InteractTrigger = new Trigger<bool>();
            ApplyTrigger = new Trigger<bool>();
            JumpTrigger = new Trigger<bool>();
            SlotLeftTrigger = new Trigger<bool>();
            SlotRightTrigger = new Trigger<bool>();
        }

        /// <summary>
        /// Frame-Update zur Ermittlung der Veränderungen.
        /// </summary>
        public void Update()
        {
            try
            {
                GamePadState gamePadState = GamePad.GetState(PlayerIndex.One);

                InteractTrigger.Value = gamePadState.Buttons.X == ButtonState.Pressed;
                ApplyTrigger.Value = gamePadState.Buttons.A == ButtonState.Pressed;
                JumpTrigger.Value = gamePadState.Buttons.Y == ButtonState.Pressed;
                SlotLeftTrigger.Value = gamePadState.Buttons.LeftShoulder == ButtonState.Pressed;
                SlotRightTrigger.Value = gamePadState.Buttons.RightShoulder == ButtonState.Pressed;
                MoveX = gamePadState.ThumbSticks.Left.X;
                MoveY = gamePadState.ThumbSticks.Left.Y;
                HeadX = gamePadState.ThumbSticks.Right.X;
                HeadY = gamePadState.ThumbSticks.Right.Y;
            }
            catch (Exception) { }
        }

        
    }
}
