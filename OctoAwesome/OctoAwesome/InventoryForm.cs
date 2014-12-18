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

            // TODO: INIT
        }
    }
}
