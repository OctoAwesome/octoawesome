namespace OctoAwesome
{
    partial class InventoryForm
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.listViewPlayer = new System.Windows.Forms.ListView();
            this.listViewBox = new System.Windows.Forms.ListView();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.listViewPlayer);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.listViewBox);
            this.splitContainer.Size = new System.Drawing.Size(915, 574);
            this.splitContainer.SplitterDistance = 425;
            this.splitContainer.TabIndex = 0;
            // 
            // listViewPlayer
            // 
            this.listViewPlayer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewPlayer.Location = new System.Drawing.Point(0, 0);
            this.listViewPlayer.Name = "listViewPlayer";
            this.listViewPlayer.Size = new System.Drawing.Size(425, 574);
            this.listViewPlayer.TabIndex = 0;
            this.listViewPlayer.UseCompatibleStateImageBehavior = false;
            // 
            // listViewBox
            // 
            this.listViewBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewBox.Location = new System.Drawing.Point(0, 0);
            this.listViewBox.Name = "listViewBox";
            this.listViewBox.Size = new System.Drawing.Size(486, 574);
            this.listViewBox.TabIndex = 1;
            this.listViewBox.UseCompatibleStateImageBehavior = false;
            // 
            // InventoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 574);
            this.Controls.Add(this.splitContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Name = "InventoryForm";
            this.Text = "Inventory";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private System.Windows.Forms.ListView listViewPlayer;
        private System.Windows.Forms.ListView listViewBox;
    }
}