using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctoAwesome
{
    public partial class MainForm : Form
    {
        private Game game = new Game();
        private Stopwatch watch = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();

            watch.Start();
            renderControl.Game = game;
            game.PlaygroundSize = new Point(renderControl.ClientSize.Width, renderControl.ClientSize.Height);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (game != null)
            {
                switch (keyData)
                {
                    case Keys.Left:
                        game.Left = true;
                        break;
                    case Keys.Right:
                        game.Right = true;
                        break;
                    case Keys.Up:
                        game.Up = true;
                        break;
                    case Keys.Down:
                        game.Down = true;
                        break;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            game.Update(watch.Elapsed);
            watch.Restart();
            renderControl.Invalidate();
        }

        private void closeMenu_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnKeyUp(KeyEventArgs e)
        {
            if (game == null)
                return;

            switch (e.KeyCode)
            {
                case Keys.Left:
                    game.Left = false;
                    break;
                case Keys.Right:
                    game.Right = false;
                    break;
                case Keys.Up:
                    game.Up = false;
                    break;
                case Keys.Down:
                    game.Down = false;
                    break;
            }

            base.OnKeyUp(e);
        }
    }
}
