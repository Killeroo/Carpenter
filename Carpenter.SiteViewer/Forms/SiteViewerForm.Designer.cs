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
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SiteViewerForm));
            OpenFolderButton = new Button();
            PathTextBox = new TextBox();
            StatusStrip = new StatusStrip();
            ToolStripProgressBar = new ToolStripProgressBar();
            StateToolStripStatusLabel = new ToolStripStatusLabel();
            GenerateAllButton = new Button();
            GenerateSiteBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            FolderBrowser = new FolderBrowserDialog();
            menuStrip = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            openInExplorerToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            generateToolStripMenuItem = new ToolStripMenuItem();
            siteToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            removeUnusedImagesToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            PublishButton = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            PublishSiteBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            ValidateButton = new Button();
            FileTreeView = new TreeView();
            FileTreeImageList = new ImageList(components);
            contextMenuStrip1 = new ContextMenuStrip(components);
            toolStripMenuItem1 = new ToolStripMenuItem();
            toolStripMenuItem2 = new ToolStripMenuItem();
            toolStripMenuItem3 = new ToolStripMenuItem();
            StatusStrip.SuspendLayout();
            menuStrip.SuspendLayout();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // OpenFolderButton
            // 
            OpenFolderButton.Location = new Point(512, 27);
            OpenFolderButton.Name = "OpenFolderButton";
            OpenFolderButton.Size = new Size(28, 22);
            OpenFolderButton.TabIndex = 0;
            OpenFolderButton.Text = "...";
            OpenFolderButton.UseVisualStyleBackColor = true;
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
            GenerateAllButton.Size = new Size(80, 22);
            GenerateAllButton.TabIndex = 4;
            GenerateAllButton.Text = "Generate all";
            GenerateAllButton.UseVisualStyleBackColor = true;
            GenerateAllButton.Click += GenerateAllButton_Click;
            // 
            // GenerateSiteBackgroundWorker
            // 
            GenerateSiteBackgroundWorker.WorkerReportsProgress = true;
            GenerateSiteBackgroundWorker.DoWork += GenerateSiteBackgroundWorker_DoWork;
            GenerateSiteBackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            GenerateSiteBackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            // 
            // menuStrip
            // 
            menuStrip.ImageScalingSize = new Size(24, 24);
            menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, generateToolStripMenuItem, toolsToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip.Location = new Point(0, 0);
            menuStrip.Name = "menuStrip";
            menuStrip.Size = new Size(553, 24);
            menuStrip.TabIndex = 7;
            menuStrip.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { toolStripSeparator2, openInExplorerToolStripMenuItem, toolStripSeparator3, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
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
            generateToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { siteToolStripMenuItem, toolStripSeparator4 });
            generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            generateToolStripMenuItem.Size = new Size(66, 20);
            generateToolStripMenuItem.Text = "Generate";
            // 
            // siteToolStripMenuItem
            // 
            siteToolStripMenuItem.Name = "siteToolStripMenuItem";
            siteToolStripMenuItem.Size = new Size(180, 22);
            siteToolStripMenuItem.Text = "Site";
            siteToolStripMenuItem.Click += siteToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(177, 6);
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { removeUnusedImagesToolStripMenuItem, toolStripSeparator1 });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // removeUnusedImagesToolStripMenuItem
            // 
            removeUnusedImagesToolStripMenuItem.Name = "removeUnusedImagesToolStripMenuItem";
            removeUnusedImagesToolStripMenuItem.Size = new Size(201, 22);
            removeUnusedImagesToolStripMenuItem.Text = "Remove Unused Images";
            removeUnusedImagesToolStripMenuItem.Click += removeUnusedImagesToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(198, 6);
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // PublishButton
            // 
            PublishButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            PublishButton.Location = new Point(12, 414);
            PublishButton.Name = "PublishButton";
            PublishButton.Size = new Size(80, 22);
            PublishButton.TabIndex = 8;
            PublishButton.Text = "Publish";
            PublishButton.UseVisualStyleBackColor = true;
            PublishButton.Click += PublishButton_Click;
            // 
            // backgroundWorker1
            // 
            backgroundWorker1.WorkerReportsProgress = true;
            // 
            // PublishSiteBackgroundWorker
            // 
            PublishSiteBackgroundWorker.WorkerReportsProgress = true;
            PublishSiteBackgroundWorker.DoWork += PublishSiteBackgroundWorker_DoWork;
            PublishSiteBackgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;
            PublishSiteBackgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;
            // 
            // ValidateButton
            // 
            ValidateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ValidateButton.Location = new Point(375, 414);
            ValidateButton.Name = "ValidateButton";
            ValidateButton.Size = new Size(80, 22);
            ValidateButton.TabIndex = 9;
            ValidateButton.Text = "Validate";
            ValidateButton.UseVisualStyleBackColor = true;
            ValidateButton.Click += ValidateButton_Click;
            // 
            // FileTreeView
            // 
            FileTreeView.ImageIndex = 0;
            FileTreeView.ImageList = FileTreeImageList;
            FileTreeView.Location = new Point(12, 56);
            FileTreeView.Name = "FileTreeView";
            FileTreeView.SelectedImageIndex = 0;
            FileTreeView.Size = new Size(528, 352);
            FileTreeView.TabIndex = 10;
            FileTreeView.NodeMouseClick += FileTreeView_NodeMouseClick;
            FileTreeView.NodeMouseDoubleClick += FileTreeView_NodeMouseDoubleClick;
            // 
            // FileTreeImageList
            // 
            FileTreeImageList.ColorDepth = ColorDepth.Depth8Bit;
            FileTreeImageList.ImageStream = (ImageListStreamer)resources.GetObject("FileTreeImageList.ImageStream");
            FileTreeImageList.TransparentColor = Color.Transparent;
            FileTreeImageList.Images.SetKeyName(0, "image_1.png");
            FileTreeImageList.Images.SetKeyName(1, "image_2.png");
            FileTreeImageList.Images.SetKeyName(2, "image_3.png");
            FileTreeImageList.Images.SetKeyName(3, "image_4.png");
            FileTreeImageList.Images.SetKeyName(4, "image_5.png");
            FileTreeImageList.Images.SetKeyName(5, "image_6.png");
            FileTreeImageList.Images.SetKeyName(6, "image_7.png");
            FileTreeImageList.Images.SetKeyName(7, "image_8.png");
            FileTreeImageList.Images.SetKeyName(8, "image_9.png");
            FileTreeImageList.Images.SetKeyName(9, "image_10.png");
            FileTreeImageList.Images.SetKeyName(10, "image_11.png");
            FileTreeImageList.Images.SetKeyName(11, "image_12.png");
            FileTreeImageList.Images.SetKeyName(12, "image_13.png");
            FileTreeImageList.Images.SetKeyName(13, "image_14.png");
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, toolStripMenuItem2, toolStripMenuItem3 });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(181, 70);
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(180, 22);
            toolStripMenuItem1.Text = "toolStripMenuItem1";
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new Size(180, 22);
            toolStripMenuItem2.Text = "toolStripMenuItem2";
            // 
            // toolStripMenuItem3
            // 
            toolStripMenuItem3.Name = "toolStripMenuItem3";
            toolStripMenuItem3.Size = new Size(180, 22);
            toolStripMenuItem3.Text = "toolStripMenuItem3";
            // 
            // SiteViewerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(553, 462);
            Controls.Add(FileTreeView);
            Controls.Add(ValidateButton);
            Controls.Add(PublishButton);
            Controls.Add(GenerateAllButton);
            Controls.Add(StatusStrip);
            Controls.Add(menuStrip);
            Controls.Add(PathTextBox);
            Controls.Add(OpenFolderButton);
            MainMenuStrip = menuStrip;
            Name = "SiteViewerForm";
            Text = "Carpenter";
            Load += MainForm_Load;
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            menuStrip.ResumeLayout(false);
            menuStrip.PerformLayout();
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button OpenFolderButton;
        private TextBox PathTextBox;
        private StatusStrip StatusStrip;
        private ToolStripStatusLabel StateToolStripStatusLabel;
        private ToolStripProgressBar ToolStripProgressBar;
        private Button GenerateAllButton;
        private System.ComponentModel.BackgroundWorker GenerateSiteBackgroundWorker;
        private FolderBrowserDialog FolderBrowser;
        private MenuStrip menuStrip;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem removeUnusedImagesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem generateHeaders1ColumnToolStripMenuItem;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem openInExplorerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem generateToolStripMenuItem;
        private ToolStripMenuItem siteToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem GenerateHeadersToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private Button PublishButton;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private System.ComponentModel.BackgroundWorker PublishSiteBackgroundWorker;
        private Button ValidateButton;
        private TreeView FileTreeView;
        private ImageList FileTreeImageList;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem toolStripMenuItem2;
        private ToolStripMenuItem toolStripMenuItem3;
    }
}