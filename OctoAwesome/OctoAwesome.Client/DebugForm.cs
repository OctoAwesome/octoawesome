using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace OctoAwesome
{
    public partial class DebugForm : Form
    {
        public DebugForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
        }

        private void lightEnabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DebugData.Instance.LightEnabled = lightEnabledCheckBox.Checked;
        }

        private void panel1_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DebugData.Instance.DiffuseColor = new Microsoft.Xna.Framework.Vector3((float)colorDialog1.Color.R / 255, (float)colorDialog1.Color.G / 255, (float)colorDialog1.Color.B / 255);
                panel1.BackColor = colorDialog1.Color;
            }
        }

        private void panel2_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DebugData.Instance.AmbientColor = new Microsoft.Xna.Framework.Vector3((float)colorDialog1.Color.R / 255, (float)colorDialog1.Color.G / 255, (float)colorDialog1.Color.B / 255);
                panel2.BackColor = colorDialog1.Color;
            }
        }

        private void panel3_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DebugData.Instance.EmissiveColor = new Microsoft.Xna.Framework.Vector3((float)colorDialog1.Color.R / 255, (float)colorDialog1.Color.G / 255, (float)colorDialog1.Color.B / 255);
                panel3.BackColor = colorDialog1.Color;
            }
        }

        private void directionalColorPanel_Click(object sender, EventArgs e)
        {
            if (colorDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                DebugData.Instance.DirectionalColor = new Microsoft.Xna.Framework.Vector3((float)colorDialog1.Color.R / 255, (float)colorDialog1.Color.G / 255, (float)colorDialog1.Color.B / 255);
                directionalColorPanel.BackColor = colorDialog1.Color;
            }
        }

        private void directionXtrackBar_Scroll(object sender, EventArgs e)
        {
            DebugData.Instance.DirectionalDirection = new Microsoft.Xna.Framework.Vector3((float)directionXtrackBar.Value / 100, DebugData.Instance.DirectionalDirection.Y, DebugData.Instance.DirectionalDirection.Z);
        }

        private void directionYtrackBar_Scroll(object sender, EventArgs e)
        {
            DebugData.Instance.DirectionalDirection = new Microsoft.Xna.Framework.Vector3(DebugData.Instance.DirectionalDirection.X, (float)directionYtrackBar.Value / 100, DebugData.Instance.DirectionalDirection.Z);
        }

        private void directionZtrackBar_Scroll(object sender, EventArgs e)
        {
            DebugData.Instance.DirectionalDirection = new Microsoft.Xna.Framework.Vector3(DebugData.Instance.DirectionalDirection.X, DebugData.Instance.DirectionalDirection.Y, (float)directionZtrackBar.Value / 100);
        }
    }
}
