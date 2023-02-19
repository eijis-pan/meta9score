namespace meta9score
{
    partial class BilliardsModuleEventLogger
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
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.planeText = new System.Windows.Forms.TextBox();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.AllVrcLog = new System.Windows.Forms.TabPage();
            this.JumpToTail = new System.Windows.Forms.Button();
            this.ClearLog = new System.Windows.Forms.Button();
            this.BilliardsLog = new System.Windows.Forms.TabPage();
            this.JumpToTail2 = new System.Windows.Forms.Button();
            this.ClearLog2 = new System.Windows.Forms.Button();
            this.BilliardLogPanel = new System.Windows.Forms.Panel();
            this.richText = new System.Windows.Forms.RichTextBox();
            this.backgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.tabControl.SuspendLayout();
            this.AllVrcLog.SuspendLayout();
            this.BilliardsLog.SuspendLayout();
            this.BilliardLogPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.SynchronizingObject = this;
            // 
            // planeText
            // 
            this.planeText.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.planeText.Font = new System.Drawing.Font("ＭＳ ゴシック", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.planeText.Location = new System.Drawing.Point(0, 0);
            this.planeText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.planeText.Multiline = true;
            this.planeText.Name = "planeText";
            this.planeText.ReadOnly = true;
            this.planeText.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.planeText.Size = new System.Drawing.Size(843, 306);
            this.planeText.TabIndex = 0;
            this.planeText.WordWrap = false;
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.AllVrcLog);
            this.tabControl.Controls.Add(this.BilliardsLog);
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(851, 397);
            this.tabControl.TabIndex = 1;
            this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
            // 
            // AllVrcLog
            // 
            this.AllVrcLog.Controls.Add(this.JumpToTail);
            this.AllVrcLog.Controls.Add(this.ClearLog);
            this.AllVrcLog.Controls.Add(this.planeText);
            this.AllVrcLog.Location = new System.Drawing.Point(4, 34);
            this.AllVrcLog.Name = "AllVrcLog";
            this.AllVrcLog.Size = new System.Drawing.Size(843, 359);
            this.AllVrcLog.TabIndex = 0;
            this.AllVrcLog.Text = "AllVrcLog";
            this.AllVrcLog.UseVisualStyleBackColor = true;
            // 
            // JumpToTail
            // 
            this.JumpToTail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.JumpToTail.Location = new System.Drawing.Point(727, 316);
            this.JumpToTail.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.JumpToTail.Name = "JumpToTail";
            this.JumpToTail.Size = new System.Drawing.Size(107, 38);
            this.JumpToTail.TabIndex = 3;
            this.JumpToTail.Text = "JumpToTail";
            this.JumpToTail.UseVisualStyleBackColor = true;
            this.JumpToTail.Click += new System.EventHandler(this.JumpToTail_Click);
            // 
            // ClearLog
            // 
            this.ClearLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ClearLog.Location = new System.Drawing.Point(9, 316);
            this.ClearLog.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ClearLog.Name = "ClearLog";
            this.ClearLog.Size = new System.Drawing.Size(107, 38);
            this.ClearLog.TabIndex = 1;
            this.ClearLog.Text = "ClearLog";
            this.ClearLog.UseVisualStyleBackColor = true;
            this.ClearLog.Click += new System.EventHandler(this.ClearLog_Click);
            // 
            // BilliardsLog
            // 
            this.BilliardsLog.Controls.Add(this.JumpToTail2);
            this.BilliardsLog.Controls.Add(this.ClearLog2);
            this.BilliardsLog.Controls.Add(this.BilliardLogPanel);
            this.BilliardsLog.Location = new System.Drawing.Point(4, 34);
            this.BilliardsLog.Name = "BilliardsLog";
            this.BilliardsLog.Size = new System.Drawing.Size(843, 359);
            this.BilliardsLog.TabIndex = 2;
            this.BilliardsLog.Text = "BilliardsLog";
            this.BilliardsLog.UseVisualStyleBackColor = true;
            // 
            // JumpToTail2
            // 
            this.JumpToTail2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.JumpToTail2.Location = new System.Drawing.Point(727, 316);
            this.JumpToTail2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.JumpToTail2.Name = "JumpToTail2";
            this.JumpToTail2.Size = new System.Drawing.Size(107, 38);
            this.JumpToTail2.TabIndex = 11;
            this.JumpToTail2.Text = "JumpToTail";
            this.JumpToTail2.UseVisualStyleBackColor = true;
            this.JumpToTail2.Click += new System.EventHandler(this.JumpToTail2_Click);
            // 
            // ClearLog2
            // 
            this.ClearLog2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.ClearLog2.Location = new System.Drawing.Point(9, 316);
            this.ClearLog2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.ClearLog2.Name = "ClearLog2";
            this.ClearLog2.Size = new System.Drawing.Size(107, 38);
            this.ClearLog2.TabIndex = 10;
            this.ClearLog2.Text = "ClearLog";
            this.ClearLog2.UseVisualStyleBackColor = true;
            this.ClearLog2.Click += new System.EventHandler(this.ClearLog2_Click);
            // 
            // BilliardLogPanel
            // 
            this.BilliardLogPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.BilliardLogPanel.Controls.Add(this.richText);
            this.BilliardLogPanel.Location = new System.Drawing.Point(0, 0);
            this.BilliardLogPanel.Name = "BilliardLogPanel";
            this.BilliardLogPanel.Size = new System.Drawing.Size(843, 308);
            this.BilliardLogPanel.TabIndex = 8;
            // 
            // richText
            // 
            this.richText.BackColor = System.Drawing.SystemColors.ControlDark;
            this.richText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richText.Font = new System.Drawing.Font("ＭＳ ゴシック", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.richText.HideSelection = false;
            this.richText.Location = new System.Drawing.Point(0, 0);
            this.richText.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.richText.Name = "richText";
            this.richText.ReadOnly = true;
            this.richText.Size = new System.Drawing.Size(843, 308);
            this.richText.TabIndex = 10;
            this.richText.Text = "";
            this.richText.WordWrap = false;
            // 
            // backgroundWorker
            // 
            this.backgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.backgroundWorker_DoWork);
            this.backgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.backgroundWorker_ProgressChanged);
            this.backgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.backgroundWorker_RunWorkerCompleted);
            // 
            // statusStrip
            // 
            this.statusStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.statusStrip.Location = new System.Drawing.Point(0, 402);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 20, 0);
            this.statusStrip.Size = new System.Drawing.Size(851, 22);
            this.statusStrip.TabIndex = 3;
            this.statusStrip.Text = "statusStrip1";
            // 
            // BilliardsModuleEventLogger
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 25F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(851, 424);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "BilliardsModuleEventLogger";
            this.ShowInTaskbar = false;
            this.Text = "BilliardsModuleEventLogger";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BilliardsModuleEventLogger_FormClosing);
            this.Load += new System.EventHandler(this.BilliardsModuleEventLogger_Load);
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.AllVrcLog.ResumeLayout(false);
            this.AllVrcLog.PerformLayout();
            this.BilliardsLog.ResumeLayout(false);
            this.BilliardLogPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private FileSystemWatcher fileSystemWatcher;
        private TabControl tabControl;
        private TabPage AllVrcLog;
        private Button ClearLog;
        private TextBox planeText;
        private System.ComponentModel.BackgroundWorker backgroundWorker;
        private Button JumpToTail;
        private StatusStrip statusStrip;
        private TabPage BilliardsLog;
        private Panel BilliardLogPanel;
        private RichTextBox richText;
        private Button JumpToTail2;
        private Button ClearLog2;
    }
}