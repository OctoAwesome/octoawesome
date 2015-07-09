using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Input
{
    internal class KeyboardScreenInput : IScreenInputSet
    {
        private List<Keys> pressedKeys = new List<Keys>();

        public Index2 PointerPosition
        {
            get { return Index2.Zero; }
            set { }
        }

        public void Update()
        {
            var state = Keyboard.GetState();
            Keys[] keys = state.GetPressedKeys();


            foreach (var key in keys)
            {
                if (!pressedKeys.Contains(key))
                {
                    if (OnKeyDown != null)
                        OnKeyDown(key);
                    pressedKeys.Add(key);
                }
            }

            List<Keys> releasedKeys = new List<Keys>();
            foreach (var key in pressedKeys)
            {
                if (!keys.Contains(key))
                {
                    if (OnKeyUp != null)
                        OnKeyUp(key);
                    releasedKeys.Add(key);
                }
            }

            foreach (var key in releasedKeys)
            {
                pressedKeys.Remove(key);
            }
        }

        public event OnKeyChange OnKeyDown;

        public event OnKeyChange OnKeyUp;
    }
}
