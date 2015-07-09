using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OctoAwesome.Client.Components.Input
{
    internal class MouseScreenInput : IScreenInputSet
    {
        public Index2 PointerPosition { get; private set; }

        public void Update()
        {
            MouseState state = Mouse.GetState();
            PointerPosition = new Index2(state.X, state.Y);
        }

        public event OnKeyChange OnKeyDown;

        public event OnKeyChange OnKeyUp;
    }
}
