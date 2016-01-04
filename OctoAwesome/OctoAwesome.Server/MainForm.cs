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
        public MainForm()
        {
            InitializeComponent();

            ServerConsole.ConsoleTextbox = console_textBox;

            Runtime.Server.Instance.OnJoin += Instance_OnJoin;
            Runtime.Server.Instance.OnLeave += Instance_OnLeave;

            ServerConsole.Log("Starting...");

            Runtime.Server.Instance.Open();

            ServerConsole.Log("Started!");
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            Runtime.Server.Instance.Close();

            base.OnFormClosed(e);
        }

        private void Instance_OnLeave(Client client)
        {
            //ListViewItem bestehend aus Playername & Planet
            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => { listViewPlayers.Items.RemoveByKey(client.ConnectionId.ToString()); }));
            }
            else
            {
                listViewPlayers.Items.RemoveByKey(client.ConnectionId.ToString());
            }

            ServerConsole.Log($"Player {client.Playername} left the game");
        }

        private void Instance_OnJoin(Client client)
        {
            //ListViewItem bestehend aus Playername & Planet
            ListViewItem playerItem = new ListViewItem();
            playerItem.Tag = client.ConnectionId;
            playerItem.Name = client.ConnectionId.ToString();
            playerItem.Text = client.Playername;
            playerItem.SubItems.Add("Default");

            if (InvokeRequired)
            {
                Invoke(new MethodInvoker(() => { listViewPlayers.Items.Add(playerItem); }));
            }
            else
            {
                listViewPlayers.Items.Add(playerItem);
            }

            ServerConsole.Log($"Player {client.Playername} joined the game");
        }

        private void button_stopServer_Click(object sender, EventArgs e)
        {
            if (Runtime.Server.Instance.IsRunning)
            {
                Runtime.Server.Instance.Close();
                ((Button) sender).Text = "Start";
            }
            else
            {
                Runtime.Server.Instance.Open();
                ((Button) sender).Text = "Stop";
            }
        }

        private void button_kickPlayer_Click(object sender, EventArgs e)
        {
            if (listViewPlayers.SelectedItems.Count == 0)
                return;

            try
            {
                Runtime.Server.Instance.Kick((Guid) listViewPlayers.SelectedItems[0].Tag);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kick Error");
            }
        }

        private void button_kickAll_Click(object sender, EventArgs e)
        {
            var guids = Runtime.Server.Instance.Clients.Select(c => c.Id);
            foreach (var guid in guids)
                Runtime.Server.Instance.Kick(guid);
        }
    }
}