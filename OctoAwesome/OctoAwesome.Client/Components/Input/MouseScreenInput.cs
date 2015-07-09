using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Input
{
    internal class MouseScreenInput : IScreenInputSet
    {
        private Index2 mousePointer;

        public Index2 PointerPosition
        {
            get { return mousePointer; }
            set
            {
                Mouse.SetPosition(value.X, value.Y);
            }
        }

        public void Update()
        {
            MouseState state = Mouse.GetState();
            mousePointer = new Index2(state.X, state.Y);
        }

        public event OnKeyChange OnKeyDown;

        public event OnKeyChange OnKeyUp;
    }
}
