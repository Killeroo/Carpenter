﻿namespace PageDesigner.Forms
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
            GridFlowLayoutPanel = new FlowLayoutPanel();
            groupBox2 = new GroupBox();
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
            BaseUrlTextBox = new TextBox();
            label3 = new Label();
            label2 = new Label();
            label1 = new Label();
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
            toolStripSeparator1 = new ToolStripSeparator();
            exitToolStripMenuItem = new ToolStripMenuItem();
            editToolStripMenuItem = new ToolStripMenuItem();
            presetsToolStripMenuItem = new ToolStripMenuItem();
            resetFieldsToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            buildToolStripMenuItem = new ToolStripMenuItem();
            previewToolStripMenuItem = new ToolStripMenuItem();
            webpageToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator3 = new ToolStripSeparator();
            preferencesToolStripMenuItem = new ToolStripMenuItem();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // ImagePreviewFlowLayoutPanel
            // 
            ImagePreviewFlowLayoutPanel.AutoScroll = true;
            ImagePreviewFlowLayoutPanel.Location = new Point(7, 29);
            ImagePreviewFlowLayoutPanel.Margin = new Padding(3, 4, 3, 4);
            ImagePreviewFlowLayoutPanel.Name = "ImagePreviewFlowLayoutPanel";
            ImagePreviewFlowLayoutPanel.Size = new Size(567, 343);
            ImagePreviewFlowLayoutPanel.TabIndex = 0;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ImagePreviewFlowLayoutPanel);
            groupBox1.Location = new Point(717, 355);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(581, 380);
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
            // GridFlowLayoutPanel
            // 
            GridFlowLayoutPanel.AutoScroll = true;
            GridFlowLayoutPanel.BorderStyle = BorderStyle.FixedSingle;
            GridFlowLayoutPanel.Location = new Point(14, 57);
            GridFlowLayoutPanel.Margin = new Padding(3, 4, 3, 4);
            GridFlowLayoutPanel.Name = "GridFlowLayoutPanel";
            GridFlowLayoutPanel.Size = new Size(696, 718);
            GridFlowLayoutPanel.TabIndex = 6;
            // 
            // groupBox2
            // 
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
            groupBox2.Controls.Add(BaseUrlTextBox);
            groupBox2.Controls.Add(label3);
            groupBox2.Controls.Add(label2);
            groupBox2.Location = new Point(717, 36);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(581, 187);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Page Details";
            // 
            // CameraTextBox
            // 
            CameraTextBox.Location = new Point(376, 149);
            CameraTextBox.Margin = new Padding(3, 4, 3, 4);
            CameraTextBox.Name = "CameraTextBox";
            CameraTextBox.Size = new Size(199, 27);
            CameraTextBox.TabIndex = 15;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(304, 153);
            label9.Name = "label9";
            label9.Size = new Size(60, 20);
            label9.TabIndex = 14;
            label9.Text = "Camera";
            // 
            // AuthorTextBox
            // 
            AuthorTextBox.Location = new Point(376, 111);
            AuthorTextBox.Margin = new Padding(3, 4, 3, 4);
            AuthorTextBox.Name = "AuthorTextBox";
            AuthorTextBox.Size = new Size(199, 27);
            AuthorTextBox.TabIndex = 13;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(304, 115);
            label8.Name = "label8";
            label8.Size = new Size(54, 20);
            label8.TabIndex = 12;
            label8.Text = "Author";
            // 
            // YearTextBox
            // 
            YearTextBox.Location = new Point(375, 73);
            YearTextBox.Margin = new Padding(3, 4, 3, 4);
            YearTextBox.Name = "YearTextBox";
            YearTextBox.Size = new Size(199, 27);
            YearTextBox.TabIndex = 11;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(304, 76);
            label7.Name = "label7";
            label7.Size = new Size(37, 20);
            label7.TabIndex = 10;
            label7.Text = "Year";
            // 
            // MonthTextBox
            // 
            MonthTextBox.Location = new Point(375, 35);
            MonthTextBox.Margin = new Padding(3, 4, 3, 4);
            MonthTextBox.Name = "MonthTextBox";
            MonthTextBox.Size = new Size(199, 27);
            MonthTextBox.TabIndex = 9;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(304, 37);
            label6.Name = "label6";
            label6.Size = new Size(52, 20);
            label6.TabIndex = 8;
            label6.Text = "Month";
            // 
            // LocationTextBox
            // 
            LocationTextBox.Location = new Point(79, 149);
            LocationTextBox.Margin = new Padding(3, 4, 3, 4);
            LocationTextBox.Name = "LocationTextBox";
            LocationTextBox.Size = new Size(211, 27);
            LocationTextBox.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(9, 153);
            label5.Name = "label5";
            label5.Size = new Size(66, 20);
            label5.TabIndex = 6;
            label5.Text = "Location";
            // 
            // TitleTextBox
            // 
            TitleTextBox.Location = new Point(79, 111);
            TitleTextBox.Margin = new Padding(3, 4, 3, 4);
            TitleTextBox.Name = "TitleTextBox";
            TitleTextBox.Size = new Size(211, 27);
            TitleTextBox.TabIndex = 5;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(9, 115);
            label4.Name = "label4";
            label4.Size = new Size(38, 20);
            label4.TabIndex = 4;
            label4.Text = "Title";
            // 
            // PageUrlTextBox
            // 
            PageUrlTextBox.Location = new Point(79, 72);
            PageUrlTextBox.Margin = new Padding(3, 4, 3, 4);
            PageUrlTextBox.Name = "PageUrlTextBox";
            PageUrlTextBox.Size = new Size(211, 27);
            PageUrlTextBox.TabIndex = 3;
            // 
            // BaseUrlTextBox
            // 
            BaseUrlTextBox.Location = new Point(79, 33);
            BaseUrlTextBox.Margin = new Padding(3, 4, 3, 4);
            BaseUrlTextBox.Name = "BaseUrlTextBox";
            BaseUrlTextBox.Size = new Size(211, 27);
            BaseUrlTextBox.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(9, 76);
            label3.Name = "label3";
            label3.Size = new Size(71, 20);
            label3.TabIndex = 1;
            label3.Text = "Page URL";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(9, 37);
            label2.Name = "label2";
            label2.Size = new Size(70, 20);
            label2.TabIndex = 0;
            label2.Text = "Base URL";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 33);
            label1.Name = "label1";
            label1.Size = new Size(80, 20);
            label1.TabIndex = 9;
            label1.Text = "Photo Grid";
            // 
            // PreviewButton
            // 
            PreviewButton.Location = new Point(1120, 743);
            PreviewButton.Margin = new Padding(3, 4, 3, 4);
            PreviewButton.Name = "PreviewButton";
            PreviewButton.Size = new Size(86, 31);
            PreviewButton.TabIndex = 10;
            PreviewButton.Text = "Preview";
            PreviewButton.UseVisualStyleBackColor = true;
            PreviewButton.Click += PreviewButton_Click;
            // 
            // GenerateButton
            // 
            GenerateButton.Location = new Point(1211, 743);
            GenerateButton.Margin = new Padding(3, 4, 3, 4);
            GenerateButton.Name = "GenerateButton";
            GenerateButton.Size = new Size(86, 31);
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
            groupBox3.Location = new Point(717, 231);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(581, 117);
            groupBox3.TabIndex = 12;
            groupBox3.TabStop = false;
            groupBox3.Text = "Image Details";
            // 
            // DetailedImageTextBox
            // 
            DetailedImageTextBox.Location = new Point(79, 73);
            DetailedImageTextBox.Margin = new Padding(3, 4, 3, 4);
            DetailedImageTextBox.Name = "DetailedImageTextBox";
            DetailedImageTextBox.Size = new Size(211, 27);
            DetailedImageTextBox.TabIndex = 7;
            DetailedImageTextBox.TextChanged += DetailedImageTextBox_TextChanged;
            // 
            // PreviewImageTextBox
            // 
            PreviewImageTextBox.Location = new Point(79, 33);
            PreviewImageTextBox.Margin = new Padding(3, 4, 3, 4);
            PreviewImageTextBox.Name = "PreviewImageTextBox";
            PreviewImageTextBox.Size = new Size(211, 27);
            PreviewImageTextBox.TabIndex = 6;
            PreviewImageTextBox.TextChanged += PreviewImageTextBox_TextChanged;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(9, 76);
            label10.Name = "label10";
            label10.Size = new Size(66, 20);
            label10.TabIndex = 5;
            label10.Text = "Detailed";
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(9, 37);
            label11.Name = "label11";
            label11.Size = new Size(60, 20);
            label11.TabIndex = 4;
            label11.Text = "Preview";
            // 
            // SaveButton
            // 
            SaveButton.Location = new Point(1027, 743);
            SaveButton.Margin = new Padding(3, 4, 3, 4);
            SaveButton.Name = "SaveButton";
            SaveButton.Size = new Size(86, 31);
            SaveButton.TabIndex = 13;
            SaveButton.Text = "Save";
            SaveButton.UseVisualStyleBackColor = true;
            SaveButton.Click += SaveButton_Click;
            // 
            // menuStrip1
            // 
            menuStrip1.ImageScalingSize = new Size(20, 20);
            menuStrip1.Items.AddRange(new ToolStripItem[] { toolStripMenuItem1, fileToolStripMenuItem, editToolStripMenuItem, buildToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Padding = new Padding(7, 3, 0, 3);
            menuStrip1.Size = new Size(1307, 30);
            menuStrip1.TabIndex = 14;
            menuStrip1.Text = "menuStrip";
            // 
            // toolStripMenuItem1
            // 
            toolStripMenuItem1.Name = "toolStripMenuItem1";
            toolStripMenuItem1.Size = new Size(14, 24);
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, toolStripSeparator2, saveToolStripMenuItem, openToolStripMenuItem, toolStripSeparator1, exitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(46, 24);
            fileToolStripMenuItem.Text = "File";
            // 
            // newToolStripMenuItem
            // 
            newToolStripMenuItem.Name = "newToolStripMenuItem";
            newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
            newToolStripMenuItem.Size = new Size(224, 26);
            newToolStripMenuItem.Text = "New";
            newToolStripMenuItem.Click += newToolStripMenuItem_Click;
            // 
            // toolStripSeparator2
            // 
            toolStripSeparator2.Name = "toolStripSeparator2";
            toolStripSeparator2.Size = new Size(221, 6);
            // 
            // saveToolStripMenuItem
            // 
            saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
            saveToolStripMenuItem.Size = new Size(224, 26);
            saveToolStripMenuItem.Text = "Save";
            saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
            // 
            // openToolStripMenuItem
            // 
            openToolStripMenuItem.Name = "openToolStripMenuItem";
            openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
            openToolStripMenuItem.Size = new Size(224, 26);
            openToolStripMenuItem.Text = "Open";
            openToolStripMenuItem.Click += openToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(221, 6);
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new Size(224, 26);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
            // 
            // editToolStripMenuItem
            // 
            editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { presetsToolStripMenuItem, resetFieldsToolStripMenuItem, preferencesToolStripMenuItem });
            editToolStripMenuItem.Name = "editToolStripMenuItem";
            editToolStripMenuItem.Size = new Size(49, 24);
            editToolStripMenuItem.Text = "Edit";
            // 
            // presetsToolStripMenuItem
            // 
            presetsToolStripMenuItem.Name = "presetsToolStripMenuItem";
            presetsToolStripMenuItem.Size = new Size(224, 26);
            presetsToolStripMenuItem.Text = "Presets";
            // 
            // resetFieldsToolStripMenuItem
            // 
            resetFieldsToolStripMenuItem.Name = "resetFieldsToolStripMenuItem";
            resetFieldsToolStripMenuItem.Size = new Size(224, 26);
            resetFieldsToolStripMenuItem.Text = "Reset Fields";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(64, 24);
            aboutToolStripMenuItem.Text = "About";
            // 
            // buildToolStripMenuItem
            // 
            buildToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { webpageToolStripMenuItem, toolStripSeparator3, previewToolStripMenuItem });
            buildToolStripMenuItem.Name = "buildToolStripMenuItem";
            buildToolStripMenuItem.Size = new Size(57, 24);
            buildToolStripMenuItem.Text = "Build";
            // 
            // previewToolStripMenuItem
            // 
            previewToolStripMenuItem.Name = "previewToolStripMenuItem";
            previewToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.P;
            previewToolStripMenuItem.Size = new Size(208, 26);
            previewToolStripMenuItem.Text = "Preview";
            previewToolStripMenuItem.Click += previewToolStripMenuItem_Click;
            // 
            // webpageToolStripMenuItem
            // 
            webpageToolStripMenuItem.Name = "webpageToolStripMenuItem";
            webpageToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.G;
            webpageToolStripMenuItem.Size = new Size(208, 26);
            webpageToolStripMenuItem.Text = "Webpage";
            webpageToolStripMenuItem.Click += webpageToolStripMenuItem_Click;
            // 
            // toolStripSeparator3
            // 
            toolStripSeparator3.Name = "toolStripSeparator3";
            toolStripSeparator3.Size = new Size(205, 6);
            // 
            // preferencesToolStripMenuItem
            // 
            preferencesToolStripMenuItem.Name = "preferencesToolStripMenuItem";
            preferencesToolStripMenuItem.Size = new Size(224, 26);
            preferencesToolStripMenuItem.Text = "Preferences";
            // 
            // PageDesignerForm
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1307, 801);
            Controls.Add(SaveButton);
            Controls.Add(groupBox3);
            Controls.Add(GenerateButton);
            Controls.Add(PreviewButton);
            Controls.Add(label1);
            Controls.Add(groupBox2);
            Controls.Add(GridFlowLayoutPanel);
            Controls.Add(groupBox1);
            Controls.Add(menuStrip1);
            MainMenuStrip = menuStrip1;
            Margin = new Padding(3, 4, 3, 4);
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
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel ImagePreviewFlowLayoutPanel;
        private GroupBox groupBox1;
        private ImageList imageList1;
        private FlowLayoutPanel GridFlowLayoutPanel;
        private GroupBox groupBox2;
        private Label label2;
        private Label label1;
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
        private TextBox BaseUrlTextBox;
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
    }
}