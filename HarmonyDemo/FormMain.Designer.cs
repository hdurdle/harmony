namespace HarmonyDemo
{
    partial class FormMain
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
            this.textBoxHarmonyHubAddress = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelLogitechUserName = new System.Windows.Forms.Label();
            this.textBoxUserName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.treeViewConfig = new System.Windows.Forms.TreeView();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabelConnection = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBoxHarmonyHubAddress
            // 
            this.textBoxHarmonyHubAddress.Location = new System.Drawing.Point(70, 73);
            this.textBoxHarmonyHubAddress.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxHarmonyHubAddress.Name = "textBoxHarmonyHubAddress";
            this.textBoxHarmonyHubAddress.Size = new System.Drawing.Size(196, 31);
            this.textBoxHarmonyHubAddress.TabIndex = 0;
            this.textBoxHarmonyHubAddress.Text = "HarmonyHub";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(64, 42);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(234, 25);
            this.label1.TabIndex = 1;
            this.label1.Text = "Harmony Hub Address:";
            // 
            // labelLogitechUserName
            // 
            this.labelLogitechUserName.AutoSize = true;
            this.labelLogitechUserName.Location = new System.Drawing.Point(372, 42);
            this.labelLogitechUserName.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.labelLogitechUserName.Name = "labelLogitechUserName";
            this.labelLogitechUserName.Size = new System.Drawing.Size(207, 25);
            this.labelLogitechUserName.TabIndex = 3;
            this.labelLogitechUserName.Text = "Logitech user name:";
            // 
            // textBoxUserName
            // 
            this.textBoxUserName.Location = new System.Drawing.Point(378, 73);
            this.textBoxUserName.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxUserName.Name = "textBoxUserName";
            this.textBoxUserName.Size = new System.Drawing.Size(264, 31);
            this.textBoxUserName.TabIndex = 2;
            this.textBoxUserName.Text = "myname@coolmail.com";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(700, 42);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(198, 25);
            this.label2.TabIndex = 5;
            this.label2.Text = "Logitech password:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(706, 73);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.PasswordChar = '*';
            this.textBoxPassword.Size = new System.Drawing.Size(264, 31);
            this.textBoxPassword.TabIndex = 4;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(24, 181);
            this.buttonConnect.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(150, 44);
            this.buttonConnect.TabIndex = 6;
            this.buttonConnect.Text = "Connect";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // treeViewConfig
            // 
            this.treeViewConfig.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewConfig.Location = new System.Drawing.Point(186, 181);
            this.treeViewConfig.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.treeViewConfig.Name = "treeViewConfig";
            this.treeViewConfig.Size = new System.Drawing.Size(1146, 862);
            this.treeViewConfig.TabIndex = 7;
            this.treeViewConfig.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewConfig_NodeMouseDoubleClick);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelConnection});
            this.statusStrip.Location = new System.Drawing.Point(0, 1076);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(2, 0, 28, 0);
            this.statusStrip.Size = new System.Drawing.Size(1360, 37);
            this.statusStrip.TabIndex = 8;
            this.statusStrip.Text = "App Status";
            // 
            // toolStripStatusLabelConnection
            // 
            this.toolStripStatusLabelConnection.Name = "toolStripStatusLabelConnection";
            this.toolStripStatusLabelConnection.Size = new System.Drawing.Size(209, 32);
            this.toolStripStatusLabelConnection.Text = "Connection Status";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1360, 1113);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.treeViewConfig);
            this.Controls.Add(this.buttonConnect);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.labelLogitechUserName);
            this.Controls.Add(this.textBoxUserName);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxHarmonyHubAddress);
            this.Margin = new System.Windows.Forms.Padding(6, 6, 6, 6);
            this.Name = "FormMain";
            this.Text = "Harmony Demo";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMain_FormClosing);
            this.Load += new System.EventHandler(this.FormMain_Load);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHarmonyHubAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelLogitechUserName;
        private System.Windows.Forms.TextBox textBoxUserName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TreeView treeViewConfig;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelConnection;
    }
}

