namespace SiteViewer.Forms
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
            headers1ColumnToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            removeUnusedImagesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            generateHeaders1ColumnToolStripMenuItem = new ToolStripMenuItem();
            generateHeaders2ColumnsToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            StatusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // OpenFolderButton
            // 
            OpenFolderButton.Location = new Point(512, 27);
            OpenFolderButton.Name = "OpenFolderButton";
            OpenFolderButton.Size = new Size(28, 23);
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
            TableLayoutPanel.Location = new Point(12, 56);
            TableLayoutPanel.Name = "TableLayoutPanel";
            TableLayoutPanel.RowCount = 1;
            TableLayoutPanel.RowStyles.Add(new RowStyle());
            TableLayoutPanel.Size = new Size(529, 352);
            TableLayoutPanel.TabIndex = 1;
            // 
            // PathTextBox
            // 
            PathTextBox.Location = new Point(12, 27);
            PathTextBox.Name = "PathTextBox";
            PathTextBox.Size = new Size(494, 23);
            PathTextBox.TabIndex = 2;
            PathTextBox.Text = "C:\\Users\\Shadowfax\\My Drive\\Website\\photos";
            PathTextBox.TextChanged += PathTextBox_TextChanged;
            // 
            // StatusStrip
            // 
            StatusStrip.ImageScalingSize = new Size(20, 20);
            StatusStrip.Items.AddRange(new ToolStripItem[] { ToolStripProgressBar, StateToolStripStatusLabel });
            StatusStrip.Location = new Point(0, 440);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(553, 22);
            StatusStrip.TabIndex = 3;
            StatusStrip.Text = "statusStrip1";
            // 
            // ToolStripProgressBar
            // 
            ToolStripProgressBar.Name = "ToolStripProgressBar";
            ToolStripProgressBar.Size = new Size(100, 16);
            // 
            // StateToolStripStatusLabel
            // 
            StateToolStripStatusLabel.Name = "StateToolStripStatusLabel";
            StateToolStripStatusLabel.Size = new Size(39, 17);
            StateToolStripStatusLabel.Text = "Ready";
            // 
            // GenerateAllButton
            // 
            GenerateAllButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            GenerateAllButton.Location = new Point(461, 414);
            GenerateAllButton.Name = "GenerateAllButton";
            GenerateAllButton.Size = new Size(80, 23);
            GenerateAllButton.TabIndex = 4;
            GenerateAllButton.Text = "Generate all";
            GenerateAllButton.UseVisualStyleBackColor = true;
            GenerateAllButton.Click += GenerateAllButton_Click;
            // 
            // NewFolderButton
            // 
            NewFolderButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            NewFolderButton.Location = new Point(380, 414);
            NewFolderButton.Name = "NewFolderButton";
            NewFolderButton.Size = new Size(75, 23);
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
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, generateToolStripMenuItem, toolsToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(553, 24);
            menuStrip.TabIndex = 7;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newFPageToolStripMenuItem, toolStripSeparator2, openInExplorerToolStripMenuItem, toolStripSeparator3, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newFPageToolStripMenuItem
            // 
            newFPageToolStripMenuItem.Name = "newFPageToolStripMenuItem";
            newFPageToolStripMenuItem.Size = new Size(180, 22);
            newFPageToolStripMenuItem.Text = "New Page...";
            newFPageToolStripMenuItem.Click += newPageToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(177, 6);
            // 
            // openInExplorerToolStripMenuItem
            // 
            openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            openInExplorerToolStripMenuItem.Size = new Size(180, 22);
            openInExplorerToolStripMenuItem.Text = "Open in Explorer";
            openInExplorerToolStripMenuItem.Click += openInExplorerToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(177, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(180, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // generateToolStripMenuItem
            // 
            generateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { siteToolStripMenuItem, toolStripSeparator4, headers1ColumnToolStripMenuItem });
            generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            generateToolStripMenuItem.Size = new Size(66, 20);
            generateToolStripMenuItem.Text = "Generate";
            // 
            // siteToolStripMenuItem
            // 
            siteToolStripMenuItem.Name = "siteToolStripMenuItem";
            siteToolStripMenuItem.Size = new Size(180, 22);
            siteToolStripMenuItem.Text = "Site";
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(177, 6);
            // 
            // headers1ColumnToolStripMenuItem
            // 
            headers1ColumnToolStripMenuItem.Name = "headers1ColumnToolStripMenuItem";
            headers1ColumnToolStripMenuItem.Size = new Size(180, 22);
            headers1ColumnToolStripMenuItem.Text = "Headers (1 Column)";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { removeUnusedImagesToolStripMenuItem, toolStripSeparator1, generateHeaders1ColumnToolStripMenuItem, generateHeaders2ColumnsToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // removeUnusedImagesToolStripMenuItem
            // 
            removeUnusedImagesToolStripMenuItem.Name = "removeUnusedImagesToolStripMenuItem";
            removeUnusedImagesToolStripMenuItem.Size = new Size(264, 22);
            removeUnusedImagesToolStripMenuItem.Text = "Remove Unused Images";
            removeUnusedImagesToolStripMenuItem.Click += removeUnusedImagesToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(261, 6);
            // 
            // generateHeaders1ColumnToolStripMenuItem
            // 
            generateHeaders1ColumnToolStripMenuItem.Name = "generateHeaders1ColumnToolStripMenuItem";
            generateHeaders1ColumnToolStripMenuItem.Size = new Size(264, 22);
            generateHeaders1ColumnToolStripMenuItem.Text = "Generate Page Headers (1 Column)";
            generateHeaders1ColumnToolStripMenuItem.Click += generateHeaders1ColumnToolStripMenuItem_Click;
            // 
            // generateHeaders2ColumnsToolStripMenuItem
            // 
            generateHeaders2ColumnsToolStripMenuItem.Name = "generateHeaders2ColumnsToolStripMenuItem";
            generateHeaders2ColumnsToolStripMenuItem.Size = new Size(264, 22);
            generateHeaders2ColumnsToolStripMenuItem.Text = "Generate Page Headers (2 Columns)";
            generateHeaders2ColumnsToolStripMenuItem.Click += generateHeaders2ColumnsToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(553, 462);
            Controls.Add(NewFolderButton);
            Controls.Add(GenerateAllButton);
            Controls.Add(StatusStrip);
            Controls.Add(menuStrip);
            Controls.Add(PathTextBox);
            Controls.Add(TableLayoutPanel);
            Controls.Add(OpenFolderButton);
            MainMenuStrip = menuStrip;
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
        private ToolStripMenuItem generateHeaders2ColumnsToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem newFPageToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem openInExplorerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem generateToolStripMenuItem;
        private ToolStripMenuItem siteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem headers1ColumnToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
    }
}