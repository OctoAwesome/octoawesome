using OctoAwesome.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            world = new World();
        }
    }
}
