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

        public bool CamLeft { get; private set; }

        public bool CamRight { get; private set; }

        public bool CamUp { get; private set; }

        public bool CamDown { get; private set; }

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
                case Keys.A:
                    CamLeft = true;
                    break;
                case Keys.D:
                    CamRight = true;
                    break;
                case Keys.W:
                    CamUp = true;
                    break;
                case Keys.S:
                    CamDown = true;
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
                case Keys.A:
                    CamLeft = false;
                    break;
                case Keys.D:
                    CamRight = false;
                    break;
                case Keys.W:
                    CamUp = false;
                    break;
                case Keys.S:
                    CamDown = false;
                    break;
            }

        }
    }
}
