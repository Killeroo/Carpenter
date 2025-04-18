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
            tableLayoutPanel3 = new TableLayoutPanel();
            label2 = new Label();
            DescriptionTextBox = new TextBox();
            tableLayoutPanel1 = new TableLayoutPanel();
            ThumbnailTextBox = new TextBox();
            TitleTextBox = new TextBox();
            label9 = new Label();
            label1 = new Label();
            label5 = new Label();
            LocationTextBox = new TextBox();
            label4 = new Label();
            CameraTextBox = new TextBox();
            label8 = new Label();
            AuthorTextBox = new TextBox();
            label7 = new Label();
            MonthTextBox = new TextBox();
            label6 = new Label();
            YearTextBox = new TextBox();
            PreviewButton = new Button();
            GenerateButton = new Button();
            groupBox3 = new GroupBox();
            tableLayoutPanel2 = new TableLayoutPanel();
            PreviewImageTextBox = new TextBox();
            DetailedImageTextBox = new TextBox();
            label11 = new Label();
            label10 = new Label();
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
            tableLayoutPanel3.SuspendLayout();
            tableLayoutPanel1.SuspendLayout();
            groupBox3.SuspendLayout();
            tableLayoutPanel2.SuspendLayout();
            menuStrip1.SuspendLayout();
            groupBox4.SuspendLayout();
            statusStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ImagePreviewFlowLayoutPanel
            // 
            ImagePreviewFlowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ImagePreviewFlowLayoutPanel.AutoScroll = true;
            ImagePreviewFlowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ImagePreviewFlowLayoutPanel.Location = new Point(5, 20);
            ImagePreviewFlowLayoutPanel.Margin = new Padding(3, 2, 3, 2);
            ImagePreviewFlowLayoutPanel.Name = "ImagePreviewFlowLayoutPanel";
            ImagePreviewFlowLayoutPanel.Size = new Size(434, 193);
            ImagePreviewFlowLayoutPanel.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(ImagePreviewFlowLayoutPanel);
            groupBox1.Location = new Point(549, 317);
            groupBox1.Margin = new Padding(3, 2, 3, 2);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 2, 3, 2);
            groupBox1.Size = new Size(444, 217);
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
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox2.Controls.Add(tableLayoutPanel3);
            groupBox2.Controls.Add(tableLayoutPanel1);
            groupBox2.Location = new Point(549, 26);
            groupBox2.Margin = new Padding(3, 2, 3, 2);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 2, 3, 2);
            groupBox2.Size = new Size(444, 213);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Page Details";
            // 
            // tableLayoutPanel3
            // 
            tableLayoutPanel3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel3.ColumnCount = 2;
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel3.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 80F));
            tableLayoutPanel3.Controls.Add(label2, 0, 0);
            tableLayoutPanel3.Controls.Add(DescriptionTextBox, 1, 0);
            tableLayoutPanel3.Location = new Point(8, 148);
            tableLayoutPanel3.Name = "tableLayoutPanel3";
            tableLayoutPanel3.RowCount = 1;
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel3.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel3.Size = new Size(430, 60);
            tableLayoutPanel3.TabIndex = 20;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label2.Location = new Point(3, 0);
            label2.Name = "label2";
            label2.Size = new Size(80, 60);
            label2.TabIndex = 18;
            label2.Text = "Description";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DescriptionTextBox
            // 
            DescriptionTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            DescriptionTextBox.Location = new Point(89, 2);
            DescriptionTextBox.Margin = new Padding(3, 2, 3, 2);
            DescriptionTextBox.Multiline = true;
            DescriptionTextBox.Name = "DescriptionTextBox";
            DescriptionTextBox.ScrollBars = ScrollBars.Vertical;
            DescriptionTextBox.Size = new Size(338, 56);
            DescriptionTextBox.TabIndex = 19;
            // 
            // tableLayoutPanel1
            // 
            tableLayoutPanel1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tableLayoutPanel1.ColumnCount = 4;
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel1.Controls.Add(ThumbnailTextBox, 1, 0);
            tableLayoutPanel1.Controls.Add(TitleTextBox, 1, 1);
            tableLayoutPanel1.Controls.Add(label9, 0, 3);
            tableLayoutPanel1.Controls.Add(label1, 0, 0);
            tableLayoutPanel1.Controls.Add(label5, 0, 2);
            tableLayoutPanel1.Controls.Add(LocationTextBox, 1, 2);
            tableLayoutPanel1.Controls.Add(label4, 0, 1);
            tableLayoutPanel1.Controls.Add(CameraTextBox, 1, 3);
            tableLayoutPanel1.Controls.Add(label8, 2, 2);
            tableLayoutPanel1.Controls.Add(AuthorTextBox, 3, 2);
            tableLayoutPanel1.Controls.Add(label7, 2, 1);
            tableLayoutPanel1.Controls.Add(MonthTextBox, 3, 0);
            tableLayoutPanel1.Controls.Add(label6, 2, 0);
            tableLayoutPanel1.Controls.Add(YearTextBox, 3, 1);
            tableLayoutPanel1.Location = new Point(8, 21);
            tableLayoutPanel1.Name = "tableLayoutPanel1";
            tableLayoutPanel1.RowCount = 4;
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 30F));
            tableLayoutPanel1.Size = new Size(430, 121);
            tableLayoutPanel1.TabIndex = 17;
            // 
            // ThumbnailTextBox
            // 
            ThumbnailTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            ThumbnailTextBox.Location = new Point(89, 2);
            ThumbnailTextBox.Margin = new Padding(3, 2, 3, 2);
            ThumbnailTextBox.Name = "ThumbnailTextBox";
            ThumbnailTextBox.Size = new Size(123, 23);
            ThumbnailTextBox.TabIndex = 17;
            // 
            // TitleTextBox
            // 
            TitleTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TitleTextBox.Location = new Point(89, 32);
            TitleTextBox.Margin = new Padding(3, 2, 3, 2);
            TitleTextBox.Name = "TitleTextBox";
            TitleTextBox.Size = new Size(123, 23);
            TitleTextBox.TabIndex = 5;
            // 
            // label9
            // 
            label9.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label9.Location = new Point(3, 90);
            label9.Name = "label9";
            label9.Size = new Size(80, 31);
            label9.TabIndex = 14;
            label9.Text = "Camera";
            label9.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label1.Location = new Point(3, 0);
            label1.Name = "label1";
            label1.Size = new Size(80, 30);
            label1.TabIndex = 16;
            label1.Text = "Thumbnail";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label5
            // 
            label5.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label5.Location = new Point(3, 60);
            label5.Name = "label5";
            label5.Size = new Size(80, 30);
            label5.TabIndex = 6;
            label5.Text = "Location";
            label5.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // LocationTextBox
            // 
            LocationTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LocationTextBox.Location = new Point(89, 62);
            LocationTextBox.Margin = new Padding(3, 2, 3, 2);
            LocationTextBox.Name = "LocationTextBox";
            LocationTextBox.Size = new Size(123, 23);
            LocationTextBox.TabIndex = 7;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label4.Location = new Point(3, 30);
            label4.Name = "label4";
            label4.Size = new Size(80, 30);
            label4.TabIndex = 4;
            label4.Text = "Title";
            label4.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // CameraTextBox
            // 
            CameraTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            CameraTextBox.Location = new Point(89, 92);
            CameraTextBox.Margin = new Padding(3, 2, 3, 2);
            CameraTextBox.Name = "CameraTextBox";
            CameraTextBox.Size = new Size(123, 23);
            CameraTextBox.TabIndex = 15;
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label8.Location = new Point(218, 60);
            label8.Name = "label8";
            label8.Size = new Size(80, 30);
            label8.TabIndex = 12;
            label8.Text = "Author";
            label8.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // AuthorTextBox
            // 
            AuthorTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            AuthorTextBox.Location = new Point(304, 62);
            AuthorTextBox.Margin = new Padding(3, 2, 3, 2);
            AuthorTextBox.Name = "AuthorTextBox";
            AuthorTextBox.Size = new Size(123, 23);
            AuthorTextBox.TabIndex = 13;
            // 
            // label7
            // 
            label7.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label7.Location = new Point(218, 30);
            label7.Name = "label7";
            label7.Size = new Size(80, 30);
            label7.TabIndex = 10;
            label7.Text = "Year";
            label7.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // MonthTextBox
            // 
            MonthTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            MonthTextBox.Location = new Point(304, 2);
            MonthTextBox.Margin = new Padding(3, 2, 3, 2);
            MonthTextBox.Name = "MonthTextBox";
            MonthTextBox.Size = new Size(123, 23);
            MonthTextBox.TabIndex = 9;
            // 
            // label6
            // 
            label6.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label6.Location = new Point(218, 0);
            label6.Name = "label6";
            label6.Size = new Size(80, 30);
            label6.TabIndex = 8;
            label6.Text = "Month";
            label6.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // YearTextBox
            // 
            YearTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            YearTextBox.Location = new Point(304, 32);
            YearTextBox.Margin = new Padding(3, 2, 3, 2);
            YearTextBox.Name = "YearTextBox";
            YearTextBox.Size = new Size(123, 23);
            YearTextBox.TabIndex = 11;
            // 
            // PreviewButton
            // 
            PreviewButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            PreviewButton.Location = new Point(858, 538);
            PreviewButton.Margin = new Padding(3, 2, 3, 2);
            PreviewButton.Name = "PreviewButton";
            PreviewButton.Size = new Size(66, 25);
            PreviewButton.TabIndex = 10;
            PreviewButton.Text = "Preview";
            PreviewButton.UseVisualStyleBackColor = true;
            PreviewButton.Click += PreviewButton_Click;
            // 
            // GenerateButton
            // 
            GenerateButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            GenerateButton.Location = new Point(927, 538);
            GenerateButton.Margin = new Padding(3, 2, 3, 2);
            GenerateButton.Name = "GenerateButton";
            GenerateButton.Size = new Size(66, 25);
            GenerateButton.TabIndex = 11;
            GenerateButton.Text = "Generate";
            GenerateButton.UseVisualStyleBackColor = true;
            GenerateButton.Click += GenerateButton_Click;
            // 
            // groupBox3
            // 
            groupBox3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox3.Controls.Add(tableLayoutPanel2);
            groupBox3.Location = new Point(549, 243);
            groupBox3.Margin = new Padding(3, 2, 3, 2);
            groupBox3.Name = "groupBox3";
            groupBox3.Padding = new Padding(3, 2, 3, 2);
            groupBox3.Size = new Size(444, 70);
            groupBox3.TabIndex = 12;
            groupBox3.TabStop = false;
            groupBox3.Text = "Image";
            // 
            // tableLayoutPanel2
            // 
            tableLayoutPanel2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            tableLayoutPanel2.ColumnCount = 4;
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 20F));
            tableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tableLayoutPanel2.Controls.Add(PreviewImageTextBox, 1, 0);
            tableLayoutPanel2.Controls.Add(DetailedImageTextBox, 3, 0);
            tableLayoutPanel2.Controls.Add(label11, 0, 0);
            tableLayoutPanel2.Controls.Add(label10, 2, 0);
            tableLayoutPanel2.Location = new Point(8, 21);
            tableLayoutPanel2.Name = "tableLayoutPanel2";
            tableLayoutPanel2.RowCount = 1;
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tableLayoutPanel2.Size = new Size(430, 38);
            tableLayoutPanel2.TabIndex = 8;
            // 
            // PreviewImageTextBox
            // 
            PreviewImageTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            PreviewImageTextBox.Location = new Point(89, 2);
            PreviewImageTextBox.Margin = new Padding(3, 2, 3, 2);
            PreviewImageTextBox.Name = "PreviewImageTextBox";
            PreviewImageTextBox.Size = new Size(123, 23);
            PreviewImageTextBox.TabIndex = 6;
            PreviewImageTextBox.TextChanged += PreviewImageTextBox_TextChanged;
            // 
            // DetailedImageTextBox
            // 
            DetailedImageTextBox.Location = new Point(304, 2);
            DetailedImageTextBox.Margin = new Padding(3, 2, 3, 2);
            DetailedImageTextBox.Name = "DetailedImageTextBox";
            DetailedImageTextBox.Size = new Size(123, 23);
            DetailedImageTextBox.TabIndex = 7;
            DetailedImageTextBox.TextChanged += DetailedImageTextBox_TextChanged;
            // 
            // label11
            // 
            label11.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label11.Location = new Point(3, 0);
            label11.Name = "label11";
            label11.Size = new Size(80, 38);
            label11.TabIndex = 4;
            label11.Text = "Filename";
            label11.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label10
            // 
            label10.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            label10.Location = new Point(218, 0);
            label10.Name = "label10";
            label10.Size = new Size(80, 38);
            label10.TabIndex = 5;
            label10.Text = "Alt name";
            label10.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // SaveButton
            // 
            SaveButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            SaveButton.Location = new Point(786, 538);
            SaveButton.Margin = new Padding(3, 2, 3, 2);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(66, 25);
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
            menuStrip1.Padding = new Padding(5, 2, 0, 2);
            menuStrip1.Size = new Size(1001, 24);
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
            undoToolStripMenuItem.Size = new Size(174, 22);
            undoToolStripMenuItem.Text = "Undo";
            undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
            // 
            // redoToolStripMenuItem
            // 
            redoToolStripMenuItem.Name = "redoToolStripMenuItem";
            redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
            redoToolStripMenuItem.Size = new Size(174, 22);
            redoToolStripMenuItem.Text = "Redo";
            redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
            // 
            // toolStripSeparator4
            // 
            toolStripSeparator4.Name = "toolStripSeparator4";
            toolStripSeparator4.Size = new Size(171, 6);
            // 
            // openInExplorerToolStripMenuItem
            // 
            openInExplorerToolStripMenuItem.Name = "openInExplorerToolStripMenuItem";
            openInExplorerToolStripMenuItem.Size = new Size(174, 22);
            openInExplorerToolStripMenuItem.Text = "Open in Explorer...";
            openInExplorerToolStripMenuItem.Click += openInExplorerToolStripMenuItem_Click;
            // 
            // openInNotepadToolStripMenuItem
            // 
            openInNotepadToolStripMenuItem.Name = "openInNotepadToolStripMenuItem";
            openInNotepadToolStripMenuItem.Size = new Size(174, 22);
            openInNotepadToolStripMenuItem.Text = "Open in Notepad...";
            openInNotepadToolStripMenuItem.Click += openInNotepadToolStripMenuItem_Click;
            // 
            // toolStripSeparator9
            // 
            toolStripSeparator9.Name = "toolStripSeparator9";
            toolStripSeparator9.Size = new Size(171, 6);
            // 
            // resetFieldsToolStripMenuItem
            // 
            resetFieldsToolStripMenuItem.Name = "resetFieldsToolStripMenuItem";
            resetFieldsToolStripMenuItem.Size = new Size(174, 22);
            resetFieldsToolStripMenuItem.Text = "Reset Fields";
            // 
            // toolStripSeparator5
            // 
            toolStripSeparator5.Name = "toolStripSeparator5";
            toolStripSeparator5.Size = new Size(171, 6);
            // 
            // presetsToolStripMenuItem
            // 
            presetsToolStripMenuItem.Name = "presetsToolStripMenuItem";
            presetsToolStripMenuItem.Size = new Size(174, 22);
            presetsToolStripMenuItem.Text = "Presets";
            // 
            // preferencesToolStripMenuItem
            // 
            preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            preferencesToolStripMenuItem.Size = new Size(174, 22);
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
            addDetailedPrefixToolStripMenuItem.Enabled = false;
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
            groupBox4.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
            groupBox4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            groupBox4.Controls.Add(GridFlowLayoutPanel);
            groupBox4.Location = new Point(10, 26);
            groupBox4.Margin = new Padding(3, 2, 3, 2);
            groupBox4.Name = "groupBox4";
            groupBox4.Padding = new Padding(3, 2, 3, 2);
            groupBox4.Size = new Size(533, 537);
            groupBox4.TabIndex = 15;
            groupBox4.TabStop = false;
            groupBox4.Text = "Photo Grid";
            // 
            // GridFlowLayoutPanel
            // 
            GridFlowLayoutPanel.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GridFlowLayoutPanel.AutoScroll = true;
            GridFlowLayoutPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            GridFlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            GridFlowLayoutPanel.Location = new Point(7, 20);
            GridFlowLayoutPanel.Margin = new Padding(3, 2, 3, 2);
            GridFlowLayoutPanel.Name = "GridFlowLayoutPanel";
            GridFlowLayoutPanel.Size = new Size(521, 513);
            GridFlowLayoutPanel.TabIndex = 6;
            // 
            // statusStrip1
            // 
            statusStrip1.ImageScalingSize = new Size(20, 20);
            statusStrip1.Items.AddRange(new ToolStripItem[] { statusToolStripStatusLabel, toolStripProgressBar1 });
            statusStrip1.Location = new Point(0, 571);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Padding = new Padding(1, 0, 10, 0);
            statusStrip1.Size = new Size(1001, 22);
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
            toolStripProgressBar1.Size = new Size(77, 16);
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
            ClientSize = new Size(1001, 593);
            Controls.Add(statusStrip1);
            Controls.Add(groupBox3);
            Controls.Add(groupBox4);
            Controls.Add(SaveButton);
            Controls.Add(GenerateButton);
            Controls.Add(PreviewButton);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            MinimumSize = new Size(1017, 572);
            Name = "PageDesignerForm";
            Text = "Carpenter";
            Load += PageDesignerForm_Load;
            Resize += PageDesignerForm_Resize;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            tableLayoutPanel3.ResumeLayout(false);
            tableLayoutPanel3.PerformLayout();
            tableLayoutPanel1.ResumeLayout(false);
            tableLayoutPanel1.PerformLayout();
            groupBox3.ResumeLayout(false);
            tableLayoutPanel2.ResumeLayout(false);
            tableLayoutPanel2.PerformLayout();
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox CameraTextBox;
        private System.Windows.Forms.Label label9;
        private TextBox AuthorTextBox;
        private Label label8;
        private TextBox YearTextBox;
        private Label label7;
        private TextBox MonthTextBox;
        private Label label6;
        private System.Windows.Forms.TextBox LocationTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox TitleTextBox;
        private System.Windows.Forms.Label label4;
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
        private System.Windows.Forms.TextBox ThumbnailTextBox;
        private System.Windows.Forms.Label label1;
        private ToolStripMenuItem openInExplorerToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator8;
        private ToolStripMenuItem editSourceToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem openInNotepadToolStripMenuItem;
        private TextBox DescriptionTextBox;
        private Label label2;
        private TableLayoutPanel tableLayoutPanel1;
        private TableLayoutPanel tableLayoutPanel2;
        private TableLayoutPanel tableLayoutPanel3;
    }
}