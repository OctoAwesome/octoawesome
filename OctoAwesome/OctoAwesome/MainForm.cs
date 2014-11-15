using OctoAwesome.Components;
using OctoAwesome.Model;
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
        private Input input = new Input();
        private Game game;
        private Stopwatch watch = new Stopwatch();

        public MainForm()
        {
            InitializeComponent();

            game = new Game(input);

            watch.Start();
            renderControl.Game = game;
            game.PlaygroundSize = new Point(renderControl.ClientSize.Width, renderControl.ClientSize.Height);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            input.KeyDown(keyData);
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
            input.KeyUp(e.KeyCode);
            base.OnKeyUp(e);
        }
    }
}
