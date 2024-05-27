namespace PageDesigner.Forms
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
            this.OpenFolderButton = new System.Windows.Forms.Button();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.PathTextBox = new System.Windows.Forms.TextBox();
            this.StatusStrip = new System.Windows.Forms.StatusStrip();
            this.ToolStripProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.StateToolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.GenerateAllButton = new System.Windows.Forms.Button();
            this.NewFolderButton = new System.Windows.Forms.Button();
            this.GenerateSiteBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.FolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            this.StatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // OpenFolderButton
            // 
            this.OpenFolderButton.Location = new System.Drawing.Point(585, 11);
            this.OpenFolderButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.OpenFolderButton.Name = "OpenFolderButton";
            this.OpenFolderButton.Size = new System.Drawing.Size(32, 31);
            this.OpenFolderButton.TabIndex = 0;
            this.OpenFolderButton.Text = "...";
            this.OpenFolderButton.UseVisualStyleBackColor = true;
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoScroll = true;
            this.TableLayoutPanel.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Single;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.Location = new System.Drawing.Point(14, 49);
            this.TableLayoutPanel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.TableLayoutPanel.Size = new System.Drawing.Size(605, 444);
            this.TableLayoutPanel.TabIndex = 1;
            // 
            // PathTextBox
            // 
            this.PathTextBox.Location = new System.Drawing.Point(14, 11);
            this.PathTextBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.PathTextBox.Name = "PathTextBox";
            this.PathTextBox.Size = new System.Drawing.Size(564, 27);
            this.PathTextBox.TabIndex = 2;
            this.PathTextBox.Text = "C:\\Users\\Shadowfax\\My Drive\\Website\\photos";
            this.PathTextBox.TextChanged += new System.EventHandler(this.PathTextBox_TextChanged);
            // 
            // StatusStrip
            // 
            this.StatusStrip.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.StatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripProgressBar,
            this.StateToolStripStatusLabel});
            this.StatusStrip.Location = new System.Drawing.Point(0, 538);
            this.StatusStrip.Name = "StatusStrip";
            this.StatusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.StatusStrip.Size = new System.Drawing.Size(632, 27);
            this.StatusStrip.TabIndex = 3;
            this.StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripProgressBar
            // 
            this.ToolStripProgressBar.Name = "ToolStripProgressBar";
            this.ToolStripProgressBar.Size = new System.Drawing.Size(114, 19);
            // 
            // StateToolStripStatusLabel
            // 
            this.StateToolStripStatusLabel.Name = "StateToolStripStatusLabel";
            this.StateToolStripStatusLabel.Size = new System.Drawing.Size(50, 21);
            this.StateToolStripStatusLabel.Text = "Ready";
            // 
            // GenerateAllButton
            // 
            this.GenerateAllButton.Location = new System.Drawing.Point(527, 501);
            this.GenerateAllButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.GenerateAllButton.Name = "GenerateAllButton";
            this.GenerateAllButton.Size = new System.Drawing.Size(91, 31);
            this.GenerateAllButton.TabIndex = 4;
            this.GenerateAllButton.Text = "Generate all";
            this.GenerateAllButton.UseVisualStyleBackColor = true;
            this.GenerateAllButton.Click += new System.EventHandler(this.GenerateAllButton_Click);
            // 
            // NewFolderButton
            // 
            this.NewFolderButton.Location = new System.Drawing.Point(434, 501);
            this.NewFolderButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.NewFolderButton.Name = "NewFolderButton";
            this.NewFolderButton.Size = new System.Drawing.Size(86, 31);
            this.NewFolderButton.TabIndex = 5;
            this.NewFolderButton.Text = "New Folder";
            this.NewFolderButton.UseVisualStyleBackColor = true;
            this.NewFolderButton.Click += new System.EventHandler(this.NewFolderButton_Click);
            // 
            // GenerateSiteBackgroundWorker
            // 
            this.GenerateSiteBackgroundWorker.WorkerReportsProgress = true;
            this.GenerateSiteBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.GenerateSiteBackgroundWorker_DoWork);
            this.GenerateSiteBackgroundWorker.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.GenerateSiteBackgroundWorker_ProgressChanged);
            this.GenerateSiteBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.GenerateSiteBackgroundWorker_RunWorkerCompleted);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(632, 565);
            this.Controls.Add(this.NewFolderButton);
            this.Controls.Add(this.GenerateAllButton);
            this.Controls.Add(this.StatusStrip);
            this.Controls.Add(this.PathTextBox);
            this.Controls.Add(this.TableLayoutPanel);
            this.Controls.Add(this.OpenFolderButton);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "Carpenter";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.StatusStrip.ResumeLayout(false);
            this.StatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Button OpenFolderButton;
        private TableLayoutPanel TableLayoutPanel;
        private TextBox PathTextBox;
        private StatusStrip StatusStrip;
        private ToolStripStatusLabel StateToolStripStatusLabel;
        private ToolStripProgressBar ToolStripProgressBar;
        private Button GenerateAllButton;
        private Button NewFolderButton;
        private System.ComponentModel.BackgroundWorker GenerateSiteBackgroundWorker;
        private FolderBrowserDialog FolderBrowser;
    }
}