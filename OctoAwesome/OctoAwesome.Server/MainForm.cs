using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Windows.Forms;

namespace OctoAwesome.Server
{
    public partial class MainForm : Form
    {
        private World world;

        public MainForm()
        {
            InitializeComponent();
            Runtime.Server.Instance.Open();
            timer1.Enabled = true;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            world = new World();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            listBox1.Items.AddRange(Runtime.Server.Instance.Clients.ToArray());
        }
    }
}
