namespace SiteViewer.Forms
{
    partial class SiteViewerForm
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
            OpenFolderButton = new Button();
            TableLayoutPanel = new TableLayoutPanel();
            PathTextBox = new TextBox();
            StatusStrip = new StatusStrip();
            ToolStripProgressBar = new ToolStripProgressBar();
            StateToolStripStatusLabel = new ToolStripStatusLabel();
            GenerateAllButton = new Button();
            NewFolderButton = new Button();
            GenerateSiteBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            FolderBrowser = new FolderBrowserDialog();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newFPageToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            openInExplorerToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            generateToolStripMenuItem = new ToolStripMenuItem();
            siteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            GenerateHeadersToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            removeUnusedImagesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            generateHeaders1ColumnToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            StatusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // OpenFolderButton
            // 
            OpenFolderButton.Location = new Point(731, 45);
            OpenFolderButton.Margin = new Padding(4, 5, 4, 5);
            OpenFolderButton.Name = "OpenFolderButton";
            OpenFolderButton.Size = new Size(40, 38);
            OpenFolderButton.TabIndex = 0;
            OpenFolderButton.Text = "...";
            OpenFolderButton.UseVisualStyleBackColor = true;
            // 
            // TableLayoutPanel
            // 
            TableLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TableLayoutPanel.AutoScroll = true;
            TableLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            TableLayoutPanel.ColumnCount = 1;
            TableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel.Location = new Point(17, 93);
            TableLayoutPanel.Margin = new Padding(4, 5, 4, 5);
            TableLayoutPanel.Name = "TableLayoutPanel";
            TableLayoutPanel.RowCount = 1;
            TableLayoutPanel.RowStyles.Add(new RowStyle());
            TableLayoutPanel.Size = new Size(756, 587);
            TableLayoutPanel.TabIndex = 1;
            // 
            // PathTextBox
            // 
            PathTextBox.Location = new Point(17, 45);
            PathTextBox.Margin = new Padding(4, 5, 4, 5);
            PathTextBox.Name = "PathTextBox";
            PathTextBox.Size = new Size(704, 31);
            PathTextBox.TabIndex = 2;
            PathTextBox.Text = "C:\\Users\\Shadowfax\\My Drive\\Website\\photos";
            PathTextBox.TextChanged += PathTextBox_TextChanged;
            // 
            // StatusStrip
            // 
            StatusStrip.ImageScalingSize = new Size(20, 20);
            StatusStrip.Items.AddRange(new ToolStripItem[] { ToolStripProgressBar, StateToolStripStatusLabel });
            StatusStrip.Location = new Point(0, 738);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Padding = new Padding(1, 0, 20, 0);
            StatusStrip.Size = new Size(790, 32);
            StatusStrip.TabIndex = 3;
            StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripProgressBar
            // 
            ToolStripProgressBar.Name = "ToolStripProgressBar";
            ToolStripProgressBar.Size = new Size(143, 24);
            // 
            // StateToolStripStatusLabel
            // 
            StateToolStripStatusLabel.Name = "StateToolStripStatusLabel";
            StateToolStripStatusLabel.Size = new Size(60, 25);
            StateToolStripStatusLabel.Text = "Ready";
            // 
            // GenerateAllButton
            // 
            GenerateAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            GenerateAllButton.Location = new Point(659, 690);
            GenerateAllButton.Margin = new Padding(4, 5, 4, 5);
            GenerateAllButton.Name = "GenerateAllButton";
            GenerateAllButton.Size = new Size(114, 38);
            GenerateAllButton.TabIndex = 4;
            GenerateAllButton.Text = "Generate all";
            GenerateAllButton.UseVisualStyleBackColor = true;
            GenerateAllButton.Click += GenerateAllButton_Click;
            // 
            // NewFolderButton
            // 
            NewFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            NewFolderButton.Location = new Point(543, 690);
            NewFolderButton.Margin = new Padding(4, 5, 4, 5);
            NewFolderButton.Name = "NewFolderButton";
            NewFolderButton.Size = new Size(107, 38);
            NewFolderButton.TabIndex = 5;
            NewFolderButton.Text = "New Folder";
            NewFolderButton.UseVisualStyleBackColor = true;
            NewFolderButton.Click += NewFolderButton_Click;
            // 
            // GenerateSiteBackgroundWorker
            // 
            GenerateSiteBackgroundWorker.WorkerReportsProgress = true;
            GenerateSiteBackgroundWorker.DoWork += GenerateSiteBackgroundWorker_DoWork;
            GenerateSiteBackgroundWorker.ProgressChanged += GenerateSiteBackgroundWorker_ProgressChanged;
            GenerateSiteBackgroundWorker.RunWorkerCompleted += GenerateSiteBackgroundWorker_RunWorkerCompleted;
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, generateToolStripMenuItem, toolsToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Padding = new Padding(9, 3, 0, 3);
            menuStrip.Size = new Size(790, 35);
            menuStrip.TabIndex = 7;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newFPageToolStripMenuItem, toolStripSeparator2, openInExplorerToolStripMenuItem, toolStripSeparator3, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(54, 29);
            fileToolStripMenuItem.Text = "File";
            // 
            // newFPageToolStripMenuItem
            // 
            newFPageToolStripMenuItem.Name = "newFPageToolStripMenuItem";
            newFPageToolStripMenuItem.Size = new Size(270, 34);
            newFPageToolStripMenuItem.Text = "New Page...";
            newFPageToolStripMenuItem.Click += newPageToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(267, 6);
            // 
            // openInExplorerToolStripMenuItem
            // 
            openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            openInExplorerToolStripMenuItem.Size = new Size(270, 34);
            openInExplorerToolStripMenuItem.Text = "Open in Explorer";
            openInExplorerToolStripMenuItem.Click += openInExplorerToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(267, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(270, 34);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // generateToolStripMenuItem
            // 
            generateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { siteToolStripMenuItem, toolStripSeparator4, GenerateHeadersToolStripMenuItem });
            generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            generateToolStripMenuItem.Size = new Size(98, 29);
            generateToolStripMenuItem.Text = "Generate";
            // 
            // siteToolStripMenuItem
            // 
            siteToolStripMenuItem.Name = "siteToolStripMenuItem";
            siteToolStripMenuItem.Size = new Size(270, 34);
            siteToolStripMenuItem.Text = "Site";
            siteToolStripMenuItem.Click += siteToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(267, 6);
            // 
            // GenerateHeadersToolStripMenuItem
            // 
            GenerateHeadersToolStripMenuItem.Name = "GenerateHeadersToolStripMenuItem";
            GenerateHeadersToolStripMenuItem.Size = new Size(270, 34);
            GenerateHeadersToolStripMenuItem.Text = "Headers";
            GenerateHeadersToolStripMenuItem.Click += GenerateHeadersToolStripMenuItem_Click;
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { removeUnusedImagesToolStripMenuItem, toolStripSeparator1, generateHeaders1ColumnToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(69, 29);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // removeUnusedImagesToolStripMenuItem
            // 
            removeUnusedImagesToolStripMenuItem.Name = "removeUnusedImagesToolStripMenuItem";
            removeUnusedImagesToolStripMenuItem.Size = new Size(306, 34);
            removeUnusedImagesToolStripMenuItem.Text = "Remove Unused Images";
            removeUnusedImagesToolStripMenuItem.Click += removeUnusedImagesToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(303, 6);
            // 
            // generateHeaders1ColumnToolStripMenuItem
            // 
            generateHeaders1ColumnToolStripMenuItem.Name = "generateHeaders1ColumnToolStripMenuItem";
            generateHeaders1ColumnToolStripMenuItem.Size = new Size(306, 34);
            generateHeaders1ColumnToolStripMenuItem.Text = "Generate Page Headers";
            generateHeaders1ColumnToolStripMenuItem.Click += generateHeadersToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(78, 29);
            aboutToolStripMenuItem.Text = "About";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(790, 770);
            Controls.Add(NewFolderButton);
            Controls.Add(GenerateAllButton);
            Controls.Add(StatusStrip);
            Controls.Add(menuStrip);
            Controls.Add(PathTextBox);
            Controls.Add(TableLayoutPanel);
            Controls.Add(OpenFolderButton);
            MainMenuStrip = menuStrip;
            Margin = new Padding(4, 5, 4, 5);
            Name = "MainForm";
            Text = "Carpenter";
            Load += MainForm_Load;
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
        private MenuStrip menuStrip;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem removeUnusedImagesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem generateHeaders1ColumnToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newFPageToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem openInExplorerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem generateToolStripMenuItem;
        private ToolStripMenuItem siteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem GenerateHeadersToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
    }
}