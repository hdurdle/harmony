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
			this.buttonConnect = new System.Windows.Forms.Button();
			this.treeViewConfig = new System.Windows.Forms.TreeView();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStripStatusLabelConnection = new System.Windows.Forms.ToolStripStatusLabel();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// textBoxHarmonyHubAddress
			// 
			this.textBoxHarmonyHubAddress.Location = new System.Drawing.Point(47, 47);
			this.textBoxHarmonyHubAddress.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.textBoxHarmonyHubAddress.Name = "textBoxHarmonyHubAddress";
			this.textBoxHarmonyHubAddress.Size = new System.Drawing.Size(132, 22);
			this.textBoxHarmonyHubAddress.TabIndex = 0;
			this.textBoxHarmonyHubAddress.Text = "HarmonyHub";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(43, 27);
			this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(155, 17);
			this.label1.TabIndex = 1;
			this.label1.Text = "Harmony Hub Address:";
			// 
			// buttonConnect
			// 
			this.buttonConnect.Location = new System.Drawing.Point(16, 116);
			this.buttonConnect.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.buttonConnect.Name = "buttonConnect";
			this.buttonConnect.Size = new System.Drawing.Size(100, 28);
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
			this.treeViewConfig.Location = new System.Drawing.Point(124, 116);
			this.treeViewConfig.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.treeViewConfig.Name = "treeViewConfig";
			this.treeViewConfig.Size = new System.Drawing.Size(765, 553);
			this.treeViewConfig.TabIndex = 7;
			this.treeViewConfig.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeViewConfig_NodeMouseDoubleClick);
			// 
			// statusStrip
			// 
			this.statusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabelConnection});
			this.statusStrip.Location = new System.Drawing.Point(0, 688);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 19, 0);
			this.statusStrip.Size = new System.Drawing.Size(907, 25);
			this.statusStrip.TabIndex = 8;
			this.statusStrip.Text = "App Status";
			// 
			// toolStripStatusLabelConnection
			// 
			this.toolStripStatusLabelConnection.Name = "toolStripStatusLabelConnection";
			this.toolStripStatusLabelConnection.Size = new System.Drawing.Size(128, 20);
			this.toolStripStatusLabelConnection.Text = "Connection Status";
			// 
			// FormMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(907, 713);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.treeViewConfig);
			this.Controls.Add(this.buttonConnect);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxHarmonyHubAddress);
			this.Margin = new System.Windows.Forms.Padding(4, 4, 4, 4);
			this.Name = "FormMain";
			this.Text = "Harmony Demo";
			this.Load += new System.EventHandler(this.FormMain_Load);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxHarmonyHubAddress;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.TreeView treeViewConfig;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabelConnection;
    }
}

