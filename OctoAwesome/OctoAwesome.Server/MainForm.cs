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

            string server = "localhost";
            int port = 8888;
            string name = "Octo";

            string address = string.Format("net.tcp://{0}:{1}/{2}", server, port, name);

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.None);

            ServiceHost host = new ServiceHost(typeof(Client), new Uri(address));
            host.AddServiceEndpoint(typeof(IClient), binding, address);
            host.Open();
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            world = new World();
        }
    }
}
