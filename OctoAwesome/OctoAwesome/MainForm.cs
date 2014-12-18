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
        private RenderControl renderControl;
        private InventoryForm inventory;

        public MainForm()
        {
            InitializeComponent();
            inventory = new InventoryForm();

            game = new Game(input);

            renderControl = new RenderControl(game);
            renderControl.Dock = DockStyle.Fill;
            this.Controls.Add(renderControl);

            watch.Start();
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

            if (game.Player.InteractionPartner != null)
            {
                if (!inventory.Visible) 
                {
                    inventory.Show();
                    inventory.Init(game.Player, game.Player.InteractionPartner);
                }
            }
            else
            {
                if (inventory.Visible)
                {
                    inventory.Hide();
                }
            }
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
