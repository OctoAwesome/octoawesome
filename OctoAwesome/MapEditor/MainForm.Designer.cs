namespace MapEditor
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.programMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.newMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.smallMapMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mediumMapMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.closeMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.cellPositionLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.grasButton = new System.Windows.Forms.ToolStripButton();
            this.sandButton = new System.Windows.Forms.ToolStripButton();
            this.waterButton = new System.Windows.Forms.ToolStripButton();
            this.loadMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.saveMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.renderControl = new MapEditor.RenderControl();
            this.menuStrip.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.programMenu});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(849, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // programMenu
            // 
            this.programMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newMenu,
            this.loadMenu,
            this.saveMenu,
            this.toolStripMenuItem1,
            this.closeMenu});
            this.programMenu.Name = "programMenu";
            this.programMenu.Size = new System.Drawing.Size(65, 20);
            this.programMenu.Text = "Program";
            // 
            // newMenu
            // 
            this.newMenu.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.smallMapMenu,
            this.mediumMapMenu});
            this.newMenu.Name = "newMenu";
            this.newMenu.Size = new System.Drawing.Size(152, 22);
            this.newMenu.Text = "New";
            // 
            // smallMapMenu
            // 
            this.smallMapMenu.Name = "smallMapMenu";
            this.smallMapMenu.Size = new System.Drawing.Size(109, 22);
            this.smallMapMenu.Text = "20 x 20";
            this.smallMapMenu.Click += new System.EventHandler(this.smallMapMenu_Click);
            // 
            // mediumMapMenu
            // 
            this.mediumMapMenu.Name = "mediumMapMenu";
            this.mediumMapMenu.Size = new System.Drawing.Size(109, 22);
            this.mediumMapMenu.Text = "40 x 40";
            this.mediumMapMenu.Click += new System.EventHandler(this.mediumMapMenu_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(149, 6);
            // 
            // closeMenu
            // 
            this.closeMenu.Name = "closeMenu";
            this.closeMenu.Size = new System.Drawing.Size(152, 22);
            this.closeMenu.Text = "Close";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.grasButton,
            this.sandButton,
            this.waterButton});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(849, 25);
            this.toolStrip.TabIndex = 1;
            this.toolStrip.Text = "toolStrip1";
            // 
            // timer
            // 
            this.timer.Interval = 40;
            this.timer.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cellPositionLabel});
            this.statusStrip1.Location = new System.Drawing.Point(0, 423);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(849, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // cellPositionLabel
            // 
            this.cellPositionLabel.Name = "cellPositionLabel";
            this.cellPositionLabel.Size = new System.Drawing.Size(33, 17);
            this.cellPositionLabel.Text = "[cell]";
            // 
            // grasButton
            // 
            this.grasButton.Checked = true;
            this.grasButton.CheckState = System.Windows.Forms.CheckState.Checked;
            this.grasButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.grasButton.Image = ((System.Drawing.Image)(resources.GetObject("grasButton.Image")));
            this.grasButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.grasButton.Name = "grasButton";
            this.grasButton.Size = new System.Drawing.Size(34, 22);
            this.grasButton.Text = "Gras";
            this.grasButton.Click += new System.EventHandler(this.grasButton_Click);
            // 
            // sandButton
            // 
            this.sandButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.sandButton.Image = ((System.Drawing.Image)(resources.GetObject("sandButton.Image")));
            this.sandButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.sandButton.Name = "sandButton";
            this.sandButton.Size = new System.Drawing.Size(37, 22);
            this.sandButton.Text = "Sand";
            this.sandButton.Click += new System.EventHandler(this.sandButton_Click);
            // 
            // waterButton
            // 
            this.waterButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.waterButton.Image = ((System.Drawing.Image)(resources.GetObject("waterButton.Image")));
            this.waterButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.waterButton.Name = "waterButton";
            this.waterButton.Size = new System.Drawing.Size(42, 22);
            this.waterButton.Text = "Water";
            this.waterButton.Click += new System.EventHandler(this.waterButton_Click);
            // 
            // loadMenu
            // 
            this.loadMenu.Name = "loadMenu";
            this.loadMenu.Size = new System.Drawing.Size(152, 22);
            this.loadMenu.Text = "Load...";
            this.loadMenu.Click += new System.EventHandler(this.loadMenu_Click);
            // 
            // saveMenu
            // 
            this.saveMenu.Enabled = false;
            this.saveMenu.Name = "saveMenu";
            this.saveMenu.Size = new System.Drawing.Size(152, 22);
            this.saveMenu.Text = "Save...";
            this.saveMenu.Click += new System.EventHandler(this.saveMenu_Click);
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = "OctoMap|*.map";
            this.openFileDialog.Title = "Load OctoMap";
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.Filter = "OctoMap|*.map";
            this.saveFileDialog.Title = "Save OctoMap";
            // 
            // renderControl
            // 
            this.renderControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.renderControl.Location = new System.Drawing.Point(0, 49);
            this.renderControl.Name = "renderControl";
            this.renderControl.Size = new System.Drawing.Size(849, 374);
            this.renderControl.TabIndex = 2;
            this.renderControl.Paint += new System.Windows.Forms.PaintEventHandler(this.renderControl_Paint);
            this.renderControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.renderControl_MouseDown);
            this.renderControl.MouseEnter += new System.EventHandler(this.renderControl_MouseEnter);
            this.renderControl.MouseLeave += new System.EventHandler(this.renderControl_MouseLeave);
            this.renderControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.renderControl_MouseMove);
            this.renderControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.renderControl_MouseUp);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(849, 445);
            this.Controls.Add(this.renderControl);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "MainForm";
            this.ShowIcon = false;
            this.Text = "OctoAwesome Map Editor";
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripMenuItem programMenu;
        private System.Windows.Forms.ToolStripMenuItem newMenu;
        private System.Windows.Forms.ToolStripMenuItem smallMapMenu;
        private System.Windows.Forms.ToolStripMenuItem mediumMapMenu;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem closeMenu;
        private System.Windows.Forms.Timer timer;
        private RenderControl renderControl;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel cellPositionLabel;
        private System.Windows.Forms.ToolStripButton grasButton;
        private System.Windows.Forms.ToolStripButton sandButton;
        private System.Windows.Forms.ToolStripButton waterButton;
        private System.Windows.Forms.ToolStripMenuItem loadMenu;
        private System.Windows.Forms.ToolStripMenuItem saveMenu;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
    }
}

