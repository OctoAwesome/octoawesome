namespace OctoAwesome
{
    partial class DebugForm
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.lightEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.directionXtrackBar = new System.Windows.Forms.TrackBar();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.directionalColorPanel = new System.Windows.Forms.Panel();
            this.label4 = new System.Windows.Forms.Label();
            this.directionYtrackBar = new System.Windows.Forms.TrackBar();
            this.directionZtrackBar = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.directionXtrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionYtrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionZtrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // lightEnabledCheckBox
            // 
            this.lightEnabledCheckBox.AutoSize = true;
            this.lightEnabledCheckBox.Location = new System.Drawing.Point(39, 29);
            this.lightEnabledCheckBox.Name = "lightEnabledCheckBox";
            this.lightEnabledCheckBox.Size = new System.Drawing.Size(64, 17);
            this.lightEnabledCheckBox.TabIndex = 0;
            this.lightEnabledCheckBox.Text = "Licht an";
            this.lightEnabledCheckBox.UseVisualStyleBackColor = true;
            this.lightEnabledCheckBox.CheckedChanged += new System.EventHandler(this.lightEnabledCheckBox_CheckedChanged);
            // 
            // directionXtrackBar
            // 
            this.directionXtrackBar.Location = new System.Drawing.Point(75, 222);
            this.directionXtrackBar.Maximum = 100;
            this.directionXtrackBar.Minimum = -100;
            this.directionXtrackBar.Name = "directionXtrackBar";
            this.directionXtrackBar.Size = new System.Drawing.Size(104, 45);
            this.directionXtrackBar.TabIndex = 1;
            this.directionXtrackBar.Scroll += new System.EventHandler(this.directionXtrackBar_Scroll);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(503, 263);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(120, 20);
            this.numericUpDown1.TabIndex = 2;
            // 
            // panel1
            // 
            this.panel1.Location = new System.Drawing.Point(39, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(27, 26);
            this.panel1.TabIndex = 3;
            this.panel1.Click += new System.EventHandler(this.panel1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(72, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Diffuse";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(72, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(45, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Ambient";
            // 
            // panel2
            // 
            this.panel2.Location = new System.Drawing.Point(39, 84);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(27, 26);
            this.panel2.TabIndex = 5;
            this.panel2.Click += new System.EventHandler(this.panel2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(72, 116);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(48, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Emissive";
            // 
            // panel3
            // 
            this.panel3.Location = new System.Drawing.Point(39, 116);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(27, 26);
            this.panel3.TabIndex = 7;
            this.panel3.Click += new System.EventHandler(this.panel3_Click);
            // 
            // directionalColorPanel
            // 
            this.directionalColorPanel.Location = new System.Drawing.Point(39, 183);
            this.directionalColorPanel.Name = "directionalColorPanel";
            this.directionalColorPanel.Size = new System.Drawing.Size(27, 27);
            this.directionalColorPanel.TabIndex = 9;
            this.directionalColorPanel.Click += new System.EventHandler(this.directionalColorPanel_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(72, 183);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(84, 13);
            this.label4.TabIndex = 10;
            this.label4.Text = "Directional Color";
            // 
            // directionYtrackBar
            // 
            this.directionYtrackBar.Location = new System.Drawing.Point(75, 273);
            this.directionYtrackBar.Maximum = 100;
            this.directionYtrackBar.Minimum = -100;
            this.directionYtrackBar.Name = "directionYtrackBar";
            this.directionYtrackBar.Size = new System.Drawing.Size(104, 45);
            this.directionYtrackBar.TabIndex = 11;
            this.directionYtrackBar.Scroll += new System.EventHandler(this.directionYtrackBar_Scroll);
            // 
            // directionZtrackBar
            // 
            this.directionZtrackBar.Location = new System.Drawing.Point(75, 324);
            this.directionZtrackBar.Maximum = 100;
            this.directionZtrackBar.Minimum = -100;
            this.directionZtrackBar.Name = "directionZtrackBar";
            this.directionZtrackBar.Size = new System.Drawing.Size(104, 45);
            this.directionZtrackBar.TabIndex = 12;
            this.directionZtrackBar.Scroll += new System.EventHandler(this.directionZtrackBar_Scroll);
            // 
            // DebugForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(794, 427);
            this.Controls.Add(this.directionZtrackBar);
            this.Controls.Add(this.directionYtrackBar);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.directionalColorPanel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.directionXtrackBar);
            this.Controls.Add(this.lightEnabledCheckBox);
            this.Name = "DebugForm";
            this.Text = "DebugForm";
            ((System.ComponentModel.ISupportInitialize)(this.directionXtrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionYtrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.directionZtrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckBox lightEnabledCheckBox;
        private System.Windows.Forms.TrackBar directionXtrackBar;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel directionalColorPanel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TrackBar directionYtrackBar;
        private System.Windows.Forms.TrackBar directionZtrackBar;
    }
}