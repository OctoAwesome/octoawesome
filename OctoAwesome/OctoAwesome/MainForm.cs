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
    }
}
