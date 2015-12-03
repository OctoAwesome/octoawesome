namespace OctoAwesome.Server
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.listViewPlayers = new System.Windows.Forms.ListView();
            this.columnPlayername = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnPlanet = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.button_kickPlayer = new System.Windows.Forms.Button();
            this.button_banPlayer = new System.Windows.Forms.Button();
            this.button_teleportPlayer = new System.Windows.Forms.Button();
            this.button_messagePlayer = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button_teleportAll = new System.Windows.Forms.Button();
            this.button_kickAll = new System.Windows.Forms.Button();
            this.button_messageAll = new System.Windows.Forms.Button();
            this.button_stopServer = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.console_textBox = new System.Windows.Forms.RichTextBox();
            this.consoleEnter_textBox = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewPlayers
            // 
            this.listViewPlayers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnPlayername,
            this.columnPlanet});
            this.listViewPlayers.FullRowSelect = true;
            this.listViewPlayers.LabelWrap = false;
            this.listViewPlayers.Location = new System.Drawing.Point(6, 19);
            this.listViewPlayers.Name = "listViewPlayers";
            this.listViewPlayers.Size = new System.Drawing.Size(224, 286);
            this.listViewPlayers.TabIndex = 2;
            this.listViewPlayers.UseCompatibleStateImageBehavior = false;
            this.listViewPlayers.View = System.Windows.Forms.View.Details;
            // 
            // columnPlayername
            // 
            this.columnPlayername.Text = "Playername";
            this.columnPlayername.Width = 101;
            // 
            // columnPlanet
            // 
            this.columnPlanet.Text = "Planet";
            this.columnPlanet.Width = 98;
            // 
            // button_kickPlayer
            // 
            this.button_kickPlayer.Location = new System.Drawing.Point(236, 19);
            this.button_kickPlayer.Name = "button_kickPlayer";
            this.button_kickPlayer.Size = new System.Drawing.Size(84, 23);
            this.button_kickPlayer.TabIndex = 3;
            this.button_kickPlayer.Text = "Kick Player";
            this.button_kickPlayer.UseVisualStyleBackColor = true;
            // 
            // button_banPlayer
            // 
            this.button_banPlayer.Location = new System.Drawing.Point(236, 48);
            this.button_banPlayer.Name = "button_banPlayer";
            this.button_banPlayer.Size = new System.Drawing.Size(84, 23);
            this.button_banPlayer.TabIndex = 4;
            this.button_banPlayer.Text = "Ban Player";
            this.button_banPlayer.UseVisualStyleBackColor = true;
            // 
            // button_teleportPlayer
            // 
            this.button_teleportPlayer.Location = new System.Drawing.Point(236, 77);
            this.button_teleportPlayer.Name = "button_teleportPlayer";
            this.button_teleportPlayer.Size = new System.Drawing.Size(84, 23);
            this.button_teleportPlayer.TabIndex = 5;
            this.button_teleportPlayer.Text = "Teleport";
            this.button_teleportPlayer.UseVisualStyleBackColor = true;
            // 
            // button_messagePlayer
            // 
            this.button_messagePlayer.Location = new System.Drawing.Point(236, 106);
            this.button_messagePlayer.Name = "button_messagePlayer";
            this.button_messagePlayer.Size = new System.Drawing.Size(84, 23);
            this.button_messagePlayer.TabIndex = 6;
            this.button_messagePlayer.Text = "Message";
            this.button_messagePlayer.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.listViewPlayers);
            this.groupBox1.Controls.Add(this.button_messagePlayer);
            this.groupBox1.Controls.Add(this.button_teleportPlayer);
            this.groupBox1.Controls.Add(this.button_kickPlayer);
            this.groupBox1.Controls.Add(this.button_banPlayer);
            this.groupBox1.Location = new System.Drawing.Point(365, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(326, 311);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Players";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button_teleportAll);
            this.groupBox2.Controls.Add(this.button_kickAll);
            this.groupBox2.Controls.Add(this.button_messageAll);
            this.groupBox2.Controls.Add(this.button_stopServer);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(162, 142);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Server Controls";
            // 
            // button_teleportAll
            // 
            this.button_teleportAll.Location = new System.Drawing.Point(6, 107);
            this.button_teleportAll.Name = "button_teleportAll";
            this.button_teleportAll.Size = new System.Drawing.Size(150, 23);
            this.button_teleportAll.TabIndex = 3;
            this.button_teleportAll.Text = "Teleport All";
            this.button_teleportAll.UseVisualStyleBackColor = true;
            // 
            // button_kickAll
            // 
            this.button_kickAll.Location = new System.Drawing.Point(6, 78);
            this.button_kickAll.Name = "button_kickAll";
            this.button_kickAll.Size = new System.Drawing.Size(150, 23);
            this.button_kickAll.TabIndex = 2;
            this.button_kickAll.Text = "Kick All";
            this.button_kickAll.UseVisualStyleBackColor = true;
            // 
            // button_messageAll
            // 
            this.button_messageAll.Location = new System.Drawing.Point(6, 49);
            this.button_messageAll.Name = "button_messageAll";
            this.button_messageAll.Size = new System.Drawing.Size(150, 23);
            this.button_messageAll.TabIndex = 1;
            this.button_messageAll.Text = "Message All";
            this.button_messageAll.UseVisualStyleBackColor = true;
            // 
            // button_stopServer
            // 
            this.button_stopServer.Location = new System.Drawing.Point(6, 20);
            this.button_stopServer.Name = "button_stopServer";
            this.button_stopServer.Size = new System.Drawing.Size(150, 23);
            this.button_stopServer.TabIndex = 0;
            this.button_stopServer.Text = "Stop";
            this.button_stopServer.UseVisualStyleBackColor = true;
            this.button_stopServer.Click += new System.EventHandler(this.button_stopServer_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.consoleEnter_textBox);
            this.groupBox3.Controls.Add(this.console_textBox);
            this.groupBox3.Location = new System.Drawing.Point(12, 160);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(347, 163);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Output / Console";
            // 
            // groupBox4
            // 
            this.groupBox4.Location = new System.Drawing.Point(181, 12);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(178, 142);
            this.groupBox4.TabIndex = 10;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Status";
            // 
            // console_textBox
            // 
            this.console_textBox.Location = new System.Drawing.Point(7, 20);
            this.console_textBox.Name = "console_textBox";
            this.console_textBox.ReadOnly = true;
            this.console_textBox.Size = new System.Drawing.Size(334, 111);
            this.console_textBox.TabIndex = 0;
            this.console_textBox.Text = "";
            // 
            // consoleEnter_textBox
            // 
            this.consoleEnter_textBox.Location = new System.Drawing.Point(7, 137);
            this.consoleEnter_textBox.Name = "consoleEnter_textBox";
            this.consoleEnter_textBox.Size = new System.Drawing.Size(334, 20);
            this.consoleEnter_textBox.TabIndex = 1;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 335);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "OctoAwesome Server";
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ListView listViewPlayers;
        private System.Windows.Forms.ColumnHeader columnPlayername;
        private System.Windows.Forms.ColumnHeader columnPlanet;
        private System.Windows.Forms.Button button_kickPlayer;
        private System.Windows.Forms.Button button_banPlayer;
        private System.Windows.Forms.Button button_teleportPlayer;
        private System.Windows.Forms.Button button_messagePlayer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button button_messageAll;
        private System.Windows.Forms.Button button_stopServer;
        private System.Windows.Forms.Button button_kickAll;
        private System.Windows.Forms.Button button_teleportAll;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox consoleEnter_textBox;
        private System.Windows.Forms.RichTextBox console_textBox;
    }
}