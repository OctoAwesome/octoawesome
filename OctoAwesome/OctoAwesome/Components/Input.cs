using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctoAwesome.Components
{
    internal sealed class Input
    {
        public bool Left { get; private set; }

        public bool Right { get; private set; }

        public bool Up { get; private set; }

        public bool Down { get; private set; }

        public Input()
        {
        }

        public void KeyDown(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    Left = true;
                    break;
                case Keys.Right:
                    Right = true;
                    break;
                case Keys.Up:
                    Up = true;
                    break;
                case Keys.Down:
                    Down = true;
                    break;
            }
        }

        public void KeyUp(Keys key)
        {
            switch (key)
            {
                case Keys.Left:
                    Left = false;
                    break;
                case Keys.Right:
                    Right = false;
                    break;
                case Keys.Up:
                    Up = false;
                    break;
                case Keys.Down:
                    Down = false;
                    break;
            }

        }
    }
}
