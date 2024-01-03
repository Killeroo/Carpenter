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
            button1 = new Button();
            textBox1 = new TextBox();
            PathTextBox = new TextBox();
            groupBox1 = new GroupBox();
            imageList1 = new ImageList(components);
            GridFlowLayoutPanel = new FlowLayoutPanel();
            groupBox2 = new GroupBox();
            button2 = new Button();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // ImagePreviewFlowLayoutPanel
            // 
            ImagePreviewFlowLayoutPanel.AutoScroll = true;
            ImagePreviewFlowLayoutPanel.Location = new Point(6, 22);
            ImagePreviewFlowLayoutPanel.Name = "ImagePreviewFlowLayoutPanel";
            ImagePreviewFlowLayoutPanel.Size = new Size(548, 186);
            ImagePreviewFlowLayoutPanel.TabIndex = 0;
            ImagePreviewFlowLayoutPanel.Click += ImagePreviewFlowLayoutPanel_Click;
            // 
            // button1
            // 
            button1.Location = new Point(655, 12);
            button1.Name = "button1";
            button1.Size = new Size(75, 23);
            button1.TabIndex = 2;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(554, 55);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(219, 23);
            textBox1.TabIndex = 3;
            // 
            // PathTextBox
            // 
            PathTextBox.Location = new Point(413, 12);
            PathTextBox.Name = "PathTextBox";
            PathTextBox.Size = new Size(219, 23);
            PathTextBox.TabIndex = 4;
            PathTextBox.Text = "C:\\Path\\Test";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ImagePreviewFlowLayoutPanel);
            groupBox1.Location = new Point(448, 231);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(560, 214);
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
            GridFlowLayoutPanel.Location = new Point(12, 12);
            GridFlowLayoutPanel.Name = "GridFlowLayoutPanel";
            GridFlowLayoutPanel.Size = new Size(380, 427);
            GridFlowLayoutPanel.TabIndex = 6;
            // 
            // groupBox2
            // 
            groupBox2.Location = new Point(448, 84);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(422, 141);
            groupBox2.TabIndex = 7;
            groupBox2.TabStop = false;
            groupBox2.Text = "Page Details";
            // 
            // button2
            // 
            button2.Location = new Point(791, 28);
            button2.Name = "button2";
            button2.Size = new Size(75, 23);
            button2.TabIndex = 8;
            button2.Text = "button2";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // PageDesignerForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1020, 457);
            Controls.Add(button2);
            Controls.Add(groupBox2);
            Controls.Add(GridFlowLayoutPanel);
            Controls.Add(groupBox1);
            Controls.Add(button1);
            Controls.Add(PathTextBox);
            Controls.Add(textBox1);
            Name = "PageDesignerForm";
            Text = "TestForm";
            groupBox1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel ImagePreviewFlowLayoutPanel;
        private Button button1;
        private TextBox textBox1;
        private TextBox PathTextBox;
        private GroupBox groupBox1;
        private ImageList imageList1;
        private FlowLayoutPanel GridFlowLayoutPanel;
        private GroupBox groupBox2;
        private Button button2;
    }
}