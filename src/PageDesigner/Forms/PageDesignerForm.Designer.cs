namespace PageDesigner
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
            textBox1 = new TextBox();
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
            button2 = new Button();
            label1 = new Label();
            button3 = new Button();
            button4 = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // ImagePreviewFlowLayoutPanel
            // 
            ImagePreviewFlowLayoutPanel.AutoScroll = true;
            ImagePreviewFlowLayoutPanel.Location = new Point(6, 22);
            ImagePreviewFlowLayoutPanel.Name = "ImagePreviewFlowLayoutPanel";
            ImagePreviewFlowLayoutPanel.Size = new Size(496, 260);
            ImagePreviewFlowLayoutPanel.TabIndex = 0;
            ImagePreviewFlowLayoutPanel.Click += ImagePreviewFlowLayoutPanel_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(633, 544);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(147, 23);
            textBox1.TabIndex = 3;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ImagePreviewFlowLayoutPanel);
            groupBox1.Location = new Point(627, 226);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(508, 288);
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
            GridFlowLayoutPanel.Location = new Point(12, 28);
            GridFlowLayoutPanel.Name = "GridFlowLayoutPanel";
            GridFlowLayoutPanel.Size = new Size(609, 539);
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
            groupBox2.Location = new Point(627, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(508, 208);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Page Details";
            // 
            // CameraTextBox
            // 
            CameraTextBox.Location = new Point(327, 54);
            CameraTextBox.Name = "CameraTextBox";
            CameraTextBox.Size = new Size(175, 23);
            CameraTextBox.TabIndex = 15;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(266, 57);
            label9.Name = "label9";
            label9.Size = new Size(48, 15);
            label9.TabIndex = 14;
            label9.Text = "Camera";
            // 
            // AuthorTextBox
            // 
            AuthorTextBox.Location = new Point(327, 25);
            AuthorTextBox.Name = "AuthorTextBox";
            AuthorTextBox.Size = new Size(175, 23);
            AuthorTextBox.TabIndex = 13;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(266, 28);
            label8.Name = "label8";
            label8.Size = new Size(44, 15);
            label8.TabIndex = 12;
            label8.Text = "Author";
            // 
            // YearTextBox
            // 
            YearTextBox.Location = new Point(69, 170);
            YearTextBox.Name = "YearTextBox";
            YearTextBox.Size = new Size(185, 23);
            YearTextBox.TabIndex = 11;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(8, 173);
            label7.Name = "label7";
            label7.Size = new Size(29, 15);
            label7.TabIndex = 10;
            label7.Text = "Year";
            // 
            // MonthTextBox
            // 
            MonthTextBox.Location = new Point(69, 141);
            MonthTextBox.Name = "MonthTextBox";
            MonthTextBox.Size = new Size(185, 23);
            MonthTextBox.TabIndex = 9;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(8, 144);
            label6.Name = "label6";
            label6.Size = new Size(43, 15);
            label6.TabIndex = 8;
            label6.Text = "Month";
            // 
            // LocationTextBox
            // 
            LocationTextBox.Location = new Point(69, 112);
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
            TitleTextBox.Location = new Point(69, 83);
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
            PageUrlTextBox.Location = new Point(69, 54);
            PageUrlTextBox.Name = "PageUrlTextBox";
            PageUrlTextBox.Size = new Size(185, 23);
            PageUrlTextBox.TabIndex = 3;
            // 
            // BaseUrlTextBox
            // 
            BaseUrlTextBox.Location = new Point(69, 25);
            BaseUrlTextBox.Name = "BaseUrlTextBox";
            BaseUrlTextBox.Size = new Size(185, 23);
            BaseUrlTextBox.TabIndex = 2;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(8, 57);
            label3.Name = "label3";
            label3.Size = new Size(57, 15);
            label3.TabIndex = 1;
            label3.Text = "Page URL";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(8, 28);
            label2.Name = "label2";
            label2.Size = new Size(55, 15);
            label2.TabIndex = 0;
            label2.Text = "Base URL";
            // 
            // button2
            // 
            button2.Location = new Point(806, 544);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 8;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 10);
            label1.Name = "label1";
            label1.Size = new Size(65, 15);
            label1.TabIndex = 9;
            label1.Text = "Grid layout";
            // 
            // button3
            // 
            button3.Location = new Point(887, 544);
            button3.Name = "button3";
            button3.Size = new Size(75, 23);
            button3.TabIndex = 10;
            button3.Text = "Preview";
            button3.UseVisualStyleBackColor = true;
            button3.Click += button3_Click;
            // 
            // button4
            // 
            button4.Location = new Point(968, 544);
            button4.Name = "button4";
            button4.Size = new Size(75, 23);
            button4.TabIndex = 11;
            button4.Text = "Generate";
            button4.UseVisualStyleBackColor = true;
            // 
            // PageDesignerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1144, 585);
            Controls.Add(button4);
            Controls.Add(button3);
            Controls.Add(label1);
            Controls.Add(button2);
            Controls.Add(groupBox2);
            Controls.Add(GridFlowLayoutPanel);
            Controls.Add(groupBox1);
            Controls.Add(textBox1);
            Name = "PageDesignerForm";
            Text = "Carpenter";
            Load += PageDesignerForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel ImagePreviewFlowLayoutPanel;
        private TextBox textBox1;
        private GroupBox groupBox1;
        private ImageList imageList1;
        private FlowLayoutPanel GridFlowLayoutPanel;
        private GroupBox groupBox2;
        private Button button2;
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
        private Button button3;
        private Button button4;
    }
}