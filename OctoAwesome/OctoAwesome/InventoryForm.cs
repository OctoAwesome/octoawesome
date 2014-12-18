using OctoAwesome.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OctoAwesome
{
    public partial class InventoryForm : Form
    {
        private IHaveInventory left;
        private IHaveInventory right;

        public InventoryForm()
        {
            InitializeComponent();
        }

        public void Init(IHaveInventory left, IHaveInventory right)
        {
            this.left = left;
            this.right = right;

            listViewLeft.Items.Clear();
            foreach (var inventoryItem in left.InventoryItems)
            {
                ListViewItem item = listViewLeft.Items.Add(inventoryItem.Name);
                item.Tag = inventoryItem;
            }

            listViewRight.Items.Clear();
            foreach (var inventoryItem in right.InventoryItems)
            {
                ListViewItem item = listViewRight.Items.Add(inventoryItem.Name);
                item.Tag = inventoryItem;
            }

        }

        private void InventoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            // this.Hide();
        }

        private void listViewLeft_DoubleClick(object sender, EventArgs e)
        {
            if (listViewLeft.SelectedItems.Count > 0)
            {
                ListViewItem item = listViewLeft.SelectedItems[0];
                InventoryItem inventoryItem = item.Tag as InventoryItem;

                left.InventoryItems.Remove(inventoryItem);
                right.InventoryItems.Add(inventoryItem);

                listViewLeft.Items.Remove(item);

                ListViewItem item2 = listViewRight.Items.Add(inventoryItem.Name);
                item2.Tag = inventoryItem;
            }
        }

        private void listViewRight_DoubleClick(object sender, EventArgs e)
        {
            if (listViewRight.SelectedItems.Count > 0)
            {
                ListViewItem item = listViewRight.SelectedItems[0];
                InventoryItem inventoryItem = item.Tag as InventoryItem;

                right.InventoryItems.Remove(inventoryItem);
                left.InventoryItems.Add(inventoryItem);

                listViewRight.Items.Remove(item);

                ListViewItem item2 = listViewLeft.Items.Add(inventoryItem.Name);
                item2.Tag = inventoryItem;
            }
        }
    }
}
