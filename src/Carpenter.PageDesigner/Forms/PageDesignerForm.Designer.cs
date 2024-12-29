namespace PageDesigner.Forms
{
    partial class PageDesignerForm
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
            ImagePreviewFlowLayoutPanel = new FlowLayoutPanel();
            groupBox1 = new GroupBox();
            imageList1 = new ImageList(components);
            groupBox2 = new GroupBox();
            ThumbnailTextBox = new TextBox();
            label1 = new Label();
            CameraTextBox = new TextBox();
            label9 = new Label();
            AuthorTextBox = new TextBox();
            label8 = new Label();
            YearTextBox = new TextBox();
            label7 = new Label();
            MonthTextBox = new TextBox();
            label6 = new Label();
            LocationTextBox = new TextBox();
            label5 = new Label();
            TitleTextBox = new TextBox();
            label4 = new Label();
            PageUrlTextBox = new TextBox();
            label3 = new Label();
            PreviewButton = new Button();
            GenerateButton = new Button();
            groupBox3 = new GroupBox();
            DetailedImageTextBox = new TextBox();
            PreviewImageTextBox = new TextBox();
            label10 = new Label();
            label11 = new Label();
            SaveButton = new Button();
            menuStrip1 = new MenuStrip();
            toolStripMenuItem1 = new ToolStripMenuItem();
            fileToolStripMenuItem = new ToolStripMenuItem();
            newToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator2 = new ToolStripSeparator();
            saveToolStripMenuItem = new ToolStripMenuItem();
            openToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator8 = new ToolStripSeparator();
            importToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            undoToolStripMenuItem = new ToolStripMenuItem();
            redoToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator4 = new ToolStripSeparator();
            openInExplorerToolStripMenuItem = new ToolStripMenuItem();
            openInNotepadToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator9 = new ToolStripSeparator();
            resetFieldsToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator5 = new ToolStripSeparator();
            presetsToolStripMenuItem = new ToolStripMenuItem();
            preferencesToolStripMenuItem = new ToolStripMenuItem();
            toolsToolStripMenuItem = new ToolStripMenuItem();
            livePreviewToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator7 = new ToolStripSeparator();
            addDetailedPrefixToolStripMenuItem = new ToolStripMenuItem();
            buildToolStripMenuItem = new ToolStripMenuItem();
            webpageToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            previewToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            groupBox4 = new GroupBox();
            GridFlowLayoutPanel = new FlowLayoutPanel();
            statusStrip1 = new StatusStrip();
            statusToolStripStatusLabel = new ToolStripStatusLabel();
            toolStripProgressBar1 = new ToolStripProgressBar();
            LivePreviewGenerateTimer = new System.Windows.Forms.Timer(components);
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            menuStrip1.SuspendLayout();
            groupBox4.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ImagePreviewFlowLayoutPanel
            // 
            ImagePreviewFlowLayoutPanel.AutoScroll = true;
            ImagePreviewFlowLayoutPanel.Location = new Point(6, 17);
            ImagePreviewFlowLayoutPanel.Name = "ImagePreviewFlowLayoutPanel";
            ImagePreviewFlowLayoutPanel.Size = new Size(496, 262);
            ImagePreviewFlowLayoutPanel.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ImagePreviewFlowLayoutPanel);
            groupBox1.Location = new Point(627, 266);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(508, 285);
            groupBox1.TabIndex = 5;
            groupBox1.TabStop = false;
            groupBox1.Text = "Available Images";
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth8Bit;
            imageList1.ImageSize = new Size(16, 16);
            imageList1.TransparentColor = Color.Transparent;
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(ThumbnailTextBox);
            groupBox2.Controls.Add(label1);
            groupBox2.Controls.Add(CameraTextBox);
            groupBox2.Controls.Add(label9);
            groupBox2.Controls.Add(AuthorTextBox);
            groupBox2.Controls.Add(label8);
            groupBox2.Controls.Add(YearTextBox);
            groupBox2.Controls.Add(label7);
            groupBox2.Controls.Add(MonthTextBox);
            groupBox2.Controls.Add(label6);
            groupBox2.Controls.Add(LocationTextBox);
            groupBox2.Controls.Add(label5);
            groupBox2.Controls.Add(TitleTextBox);
            groupBox2.Controls.Add(label4);
            groupBox2.Controls.Add(PageUrlTextBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(627, 27);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(508, 140);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Page Details";
            // 
            // ThumbnailTextBox
            // 
            ThumbnailTextBox.Location = new Point(77, 55);
            ThumbnailTextBox.Name = "ThumbnailTextBox";
            ThumbnailTextBox.Size = new Size(185, 23);
            ThumbnailTextBox.TabIndex = 17;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(8, 58);
            label1.Name = "label1";
            label1.Size = new Size(64, 15);
            label1.TabIndex = 16;
            label1.Text = "Thumbnail";
            // 
            // CameraTextBox
            // 
            CameraTextBox.Location = new Point(329, 112);
            CameraTextBox.Name = "CameraTextBox";
            CameraTextBox.Size = new Size(175, 23);
            CameraTextBox.TabIndex = 15;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(274, 115);
            label9.Name = "label9";
            label9.Size = new Size(48, 15);
            label9.TabIndex = 14;
            label9.Text = "Camera";
            // 
            // AuthorTextBox
            // 
            AuthorTextBox.Location = new Point(329, 83);
            AuthorTextBox.Name = "AuthorTextBox";
            AuthorTextBox.Size = new Size(175, 23);
            AuthorTextBox.TabIndex = 13;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(274, 86);
            label8.Name = "label8";
            label8.Size = new Size(44, 15);
            label8.TabIndex = 12;
            label8.Text = "Author";
            // 
            // YearTextBox
            // 
            YearTextBox.Location = new Point(328, 55);
            YearTextBox.Name = "YearTextBox";
            YearTextBox.Size = new Size(175, 23);
            YearTextBox.TabIndex = 11;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(274, 57);
            label7.Name = "label7";
            label7.Size = new Size(29, 15);
            label7.TabIndex = 10;
            label7.Text = "Year";
            // 
            // MonthTextBox
            // 
            MonthTextBox.Location = new Point(328, 26);
            MonthTextBox.Name = "MonthTextBox";
            MonthTextBox.Size = new Size(175, 23);
            MonthTextBox.TabIndex = 9;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(274, 28);
            label6.Name = "label6";
            label6.Size = new Size(43, 15);
            label6.TabIndex = 8;
            label6.Text = "Month";
            // 
            // LocationTextBox
            // 
            LocationTextBox.Location = new Point(77, 112);
            LocationTextBox.Name = "LocationTextBox";
            LocationTextBox.Size = new Size(185, 23);
            LocationTextBox.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(8, 115);
            label5.Name = "label5";
            label5.Size = new Size(53, 15);
            label5.TabIndex = 6;
            label5.Text = "Location";
            // 
            // TitleTextBox
            // 
            TitleTextBox.Location = new Point(77, 83);
            TitleTextBox.Name = "TitleTextBox";
            TitleTextBox.Size = new Size(185, 23);
            TitleTextBox.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(8, 86);
            label4.Name = "label4";
            label4.Size = new Size(29, 15);
            label4.TabIndex = 4;
            label4.Text = "Title";
            // 
            // PageUrlTextBox
            // 
            PageUrlTextBox.Location = new Point(77, 26);
            PageUrlTextBox.Name = "PageUrlTextBox";
            PageUrlTextBox.Size = new Size(185, 23);
            PageUrlTextBox.TabIndex = 3;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 29);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 1;
            label3.Text = "Page URL";
            // 
            // PreviewButton
            // 
            PreviewButton.Location = new Point(980, 566);
            PreviewButton.Name = "PreviewButton";
            PreviewButton.Size = new Size(75, 23);
            PreviewButton.TabIndex = 10;
            PreviewButton.Text = "Preview";
            PreviewButton.UseVisualStyleBackColor = true;
            PreviewButton.Click += PreviewButton_Click;
            // 
            // GenerateButton
            // 
            GenerateButton.Location = new Point(1060, 566);
            GenerateButton.Name = "GenerateButton";
            GenerateButton.Size = new Size(75, 23);
            GenerateButton.TabIndex = 11;
            GenerateButton.Text = "Generate";
            GenerateButton.UseVisualStyleBackColor = true;
            GenerateButton.Click += GenerateButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(DetailedImageTextBox);
            groupBox3.Controls.Add(PreviewImageTextBox);
            groupBox3.Controls.Add(label10);
            groupBox3.Controls.Add(label11);
            groupBox3.Location = new Point(627, 173);
            groupBox3.Margin = new Padding(3, 2, 3, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 2, 3, 2);
            groupBox3.Size = new Size(508, 88);
            groupBox3.TabIndex = 12;
            groupBox3.TabStop = false;
            groupBox3.Text = "Image Details";
            // 
            // DetailedImageTextBox
            // 
            DetailedImageTextBox.Location = new Point(69, 55);
            DetailedImageTextBox.Name = "DetailedImageTextBox";
            DetailedImageTextBox.Size = new Size(185, 23);
            DetailedImageTextBox.TabIndex = 7;
            DetailedImageTextBox.TextChanged += DetailedImageTextBox_TextChanged;
            // 
            // PreviewImageTextBox
            // 
            PreviewImageTextBox.Location = new Point(69, 25);
            PreviewImageTextBox.Name = "PreviewImageTextBox";
            PreviewImageTextBox.Size = new Size(185, 23);
            PreviewImageTextBox.TabIndex = 6;
            PreviewImageTextBox.TextChanged += PreviewImageTextBox_TextChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(8, 57);
            label10.Name = "label10";
            label10.Size = new Size(50, 15);
            label10.TabIndex = 5;
            label10.Text = "Detailed";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(8, 28);
            label11.Name = "label11";
            label11.Size = new Size(48, 15);
            label11.TabIndex = 4;
            label11.Text = "Preview";
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(899, 566);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(75, 23);
            SaveButton.TabIndex = 13;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, fileToolStripMenuItem, editToolStripMenuItem, toolsToolStripMenuItem, buildToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(1144, 24);
            menuStrip1.TabIndex = 14;
            menuStrip1.Text = "menuStrip";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(12, 20);
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, toolStripSeparator2, saveToolStripMenuItem, openToolStripMenuItem, toolStripSeparator8, importToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(160, 22);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(157, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(160, 22);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(160, 22);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripSeparator8
            // 
            toolStripSeparator8.Name = "toolStripSeparator8";
            toolStripSeparator8.Size = new Size(157, 6);
            // 
            // importToolStripMenuItem
            // 
            importToolStripMenuItem.Name = "importToolStripMenuItem";
            importToolStripMenuItem.Size = new Size(160, 22);
            importToolStripMenuItem.Text = "Import Images...";
            importToolStripMenuItem.Click += importToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(157, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(160, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { undoToolStripMenuItem, redoToolStripMenuItem, toolStripSeparator4, openInExplorerToolStripMenuItem, openInNotepadToolStripMenuItem, toolStripSeparator9, resetFieldsToolStripMenuItem, toolStripSeparator5, presetsToolStripMenuItem, preferencesToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(39, 20);
            editToolStripMenuItem.Text = "Edit";
            // 
            // undoToolStripMenuItem
            // 
            undoToolStripMenuItem.Name = "undoToolStripMenuItem";
            undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
            undoToolStripMenuItem.Size = new Size(180, 22);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(180, 22);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(177, 6);
            // 
            // openInExplorerToolStripMenuItem
            // 
            openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            openInExplorerToolStripMenuItem.Size = new Size(180, 22);
            openInExplorerToolStripMenuItem.Text = "Open in Explorer...";
            openInExplorerToolStripMenuItem.Click += openInExplorerToolStripMenuItem_Click;
            // 
            // openInNotepadToolStripMenuItem
            // 
            openInNotepadToolStripMenuItem.Name = "openInNotepadToolStripMenuItem";
            openInNotepadToolStripMenuItem.Size = new Size(180, 22);
            openInNotepadToolStripMenuItem.Text = "Open in Notepad...";
            openInNotepadToolStripMenuItem.Click += openInNotepadToolStripMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(177, 6);
            // 
            // resetFieldsToolStripMenuItem
            // 
            resetFieldsToolStripMenuItem.Name = "resetFieldsToolStripMenuItem";
            resetFieldsToolStripMenuItem.Size = new Size(180, 22);
            resetFieldsToolStripMenuItem.Text = "Reset Fields";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(177, 6);
            // 
            // presetsToolStripMenuItem
            // 
            presetsToolStripMenuItem.Name = "presetsToolStripMenuItem";
            presetsToolStripMenuItem.Size = new Size(180, 22);
            presetsToolStripMenuItem.Text = "Presets";
            // 
            // preferencesToolStripMenuItem
            // 
            preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            preferencesToolStripMenuItem.Size = new Size(180, 22);
            preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // toolsToolStripMenuItem
            // 
            toolsToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { livePreviewToolStripMenuItem, toolStripSeparator7, addDetailedPrefixToolStripMenuItem });
            toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            toolsToolStripMenuItem.Size = new Size(46, 20);
            toolsToolStripMenuItem.Text = "Tools";
            // 
            // livePreviewToolStripMenuItem
            // 
            livePreviewToolStripMenuItem.Name = "livePreviewToolStripMenuItem";
            livePreviewToolStripMenuItem.Size = new Size(184, 22);
            livePreviewToolStripMenuItem.Text = "Live Preview";
            livePreviewToolStripMenuItem.Click += livePreviewToolStripMenuItem_Click;
            // 
            // toolStripSeparator7
            // 
            toolStripSeparator7.Name = "toolStripSeparator7";
            toolStripSeparator7.Size = new Size(181, 6);
            // 
            // addDetailedPrefixToolStripMenuItem
            // 
            addDetailedPrefixToolStripMenuItem.Checked = true;
            addDetailedPrefixToolStripMenuItem.CheckState = CheckState.Checked;
            addDetailedPrefixToolStripMenuItem.Name = "addDetailedPrefixToolStripMenuItem";
            addDetailedPrefixToolStripMenuItem.Size = new Size(184, 22);
            addDetailedPrefixToolStripMenuItem.Text = "Add prefix to images";
            addDetailedPrefixToolStripMenuItem.Click += adddetailedPrefixToolStripMenuItem_Click;
            // 
            // buildToolStripMenuItem
            // 
            buildToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { webpageToolStripMenuItem, toolStripSeparator3, previewToolStripMenuItem });
            buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            buildToolStripMenuItem.Size = new Size(66, 20);
            buildToolStripMenuItem.Text = "Generate";
            // 
            // webpageToolStripMenuItem
            // 
            webpageToolStripMenuItem.Name = "webpageToolStripMenuItem";
            webpageToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G;
            webpageToolStripMenuItem.Size = new Size(166, 22);
            webpageToolStripMenuItem.Text = "Webpage";
            webpageToolStripMenuItem.Click += webpageToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(163, 6);
            // 
            // previewToolStripMenuItem
            // 
            previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            previewToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            previewToolStripMenuItem.Size = new Size(166, 22);
            previewToolStripMenuItem.Text = "Preview";
            previewToolStripMenuItem.Click += previewToolStripMenuItem_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(GridFlowLayoutPanel);
            groupBox4.Location = new Point(12, 27);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(609, 524);
            groupBox4.TabIndex = 15;
            groupBox4.TabStop = false;
            groupBox4.Text = "Photo Grid";
            // 
            // GridFlowLayoutPanel
            // 
            GridFlowLayoutPanel.AutoScroll = true;
            GridFlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            GridFlowLayoutPanel.Location = new Point(8, 16);
            GridFlowLayoutPanel.Name = "GridFlowLayoutPanel";
            GridFlowLayoutPanel.Size = new Size(595, 502);
            GridFlowLayoutPanel.TabIndex = 6;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusToolStripStatusLabel, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 579);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 12, 0);
            statusStrip1.Size = new Size(1144, 22);
            statusStrip1.TabIndex = 16;
            statusStrip1.Text = "statusStrip";
            // 
            // statusToolStripStatusLabel
            // 
            statusToolStripStatusLabel.Name = "statusToolStripStatusLabel";
            statusToolStripStatusLabel.Size = new Size(39, 17);
            statusToolStripStatusLabel.Text = "Ready";
            // 
            // toolStripProgressBar1
            // 
            toolStripProgressBar1.Name = "toolStripProgressBar1";
            toolStripProgressBar1.Size = new Size(88, 16);
            // 
            // LivePreviewGenerateTimer
            // 
            LivePreviewGenerateTimer.Interval = 1000;
            LivePreviewGenerateTimer.Tick += LivePreviewGenerateTimer_Tick;
            // 
            // PageDesignerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1144, 601);
            Controls.Add(statusStrip1);
            Controls.Add(groupBox4);
            Controls.Add(SaveButton);
            Controls.Add(groupBox3);
            Controls.Add(GenerateButton);
            Controls.Add(PreviewButton);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Name = "PageDesignerForm";
            Text = "Carpenter";
            Load += PageDesignerForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            groupBox4.ResumeLayout(false);
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel ImagePreviewFlowLayoutPanel;
        private GroupBox groupBox1;
        private ImageList imageList1;
        private GroupBox groupBox2;
        private TextBox CameraTextBox;
        private Label label9;
        private TextBox AuthorTextBox;
        private Label label8;
        private TextBox YearTextBox;
        private Label label7;
        private TextBox MonthTextBox;
        private Label label6;
        private TextBox LocationTextBox;
        private Label label5;
        private TextBox TitleTextBox;
        private Label label4;
        private TextBox PageUrlTextBox;
        private Label label3;
        private Button PreviewButton;
        private Button GenerateButton;
        private GroupBox groupBox3;
        private TextBox DetailedImageTextBox;
        private TextBox PreviewImageTextBox;
        private Label label10;
        private Label label11;
        private Button SaveButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem toolStripMenuItem1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem editToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem saveToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem presetsToolStripMenuItem;
        private ToolStripMenuItem resetFieldsToolStripMenuItem;
        private ToolStripMenuItem newToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripMenuItem buildToolStripMenuItem;
        private ToolStripMenuItem previewToolStripMenuItem;
        private ToolStripMenuItem webpageToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator3;
        private ToolStripMenuItem preferencesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator4;
        private ToolStripMenuItem undoToolStripMenuItem;
        private ToolStripMenuItem redoToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator5;
        private GroupBox groupBox4;
        private FlowLayoutPanel GridFlowLayoutPanel;
        private ToolStripSeparator toolStripSeparator6;
        private ToolStripMenuItem importToolStripMenuItem;
        private ToolStripMenuItem toolsToolStripMenuItem;
        private ToolStripMenuItem addDetailedPrefixToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel statusToolStripStatusLabel;
        private ToolStripProgressBar toolStripProgressBar1;
        private ToolStripMenuItem livePreviewToolStripMenuItem;
        private System.Windows.Forms.Timer LivePreviewGenerateTimer;
        private ToolStripSeparator toolStripSeparator7;
        private TextBox ThumbnailTextBox;
        private Label label1;
        private ToolStripMenuItem openInExplorerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem editSourceToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem openInNotepadToolStripMenuItem;
    }
}