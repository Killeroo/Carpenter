namespace SiteViewer.Forms
{
    partial class GeneratedPageHeadersForm
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
            SingleTextBox = new TextBox();
            ClipboardButton = new Button();
            CloseButton = new Button();
            SplitColumnCheckBox = new CheckBox();
            splitContainer1 = new SplitContainer();
            LeftTextBox = new TextBox();
            RightTextBox = new TextBox();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // SingleTextBox
            // 
            SingleTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            SingleTextBox.Location = new Point(17, 59);
            SingleTextBox.Margin = new Padding(4, 5, 4, 5);
            SingleTextBox.Multiline = true;
            SingleTextBox.Name = "SingleTextBox";
            SingleTextBox.ReadOnly = true;
            SingleTextBox.ScrollBars = ScrollBars.Vertical;
            SingleTextBox.Size = new Size(684, 682);
            SingleTextBox.TabIndex = 0;
            // 
            // ClipboardButton
            // 
            ClipboardButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ClipboardButton.Location = new Point(373, 753);
            ClipboardButton.Margin = new Padding(4, 5, 4, 5);
            ClipboardButton.Name = "ClipboardButton";
            ClipboardButton.Size = new Size(196, 38);
            ClipboardButton.TabIndex = 1;
            ClipboardButton.Text = "Copy to Clipboard";
            ClipboardButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CloseButton.Location = new Point(577, 753);
            CloseButton.Margin = new Padding(4, 5, 4, 5);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(126, 38);
            CloseButton.TabIndex = 2;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // SplitColumnCheckBox
            // 
            SplitColumnCheckBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            SplitColumnCheckBox.AutoSize = true;
            SplitColumnCheckBox.Location = new Point(556, 22);
            SplitColumnCheckBox.Name = "SplitColumnCheckBox";
            SplitColumnCheckBox.Size = new Size(145, 29);
            SplitColumnCheckBox.TabIndex = 5;
            SplitColumnCheckBox.Text = "Split columns";
            SplitColumnCheckBox.UseVisualStyleBackColor = true;
            SplitColumnCheckBox.CheckedChanged += SplitColumnCheckBox_CheckedChanged;
            // 
            // splitContainer1
            // 
            splitContainer1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            splitContainer1.Location = new Point(77, 95);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(LeftTextBox);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(RightTextBox);
            splitContainer1.Size = new Size(562, 504);
            splitContainer1.SplitterDistance = 257;
            splitContainer1.TabIndex = 6;
            // 
            // LeftTextBox
            // 
            LeftTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            LeftTextBox.Location = new Point(4, 5);
            LeftTextBox.Margin = new Padding(4, 5, 4, 5);
            LeftTextBox.Multiline = true;
            LeftTextBox.Name = "LeftTextBox";
            LeftTextBox.ReadOnly = true;
            LeftTextBox.ScrollBars = ScrollBars.Vertical;
            LeftTextBox.Size = new Size(238, 494);
            LeftTextBox.TabIndex = 4;
            // 
            // RightTextBox
            // 
            RightTextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            RightTextBox.Location = new Point(4, 5);
            RightTextBox.Margin = new Padding(4, 5, 4, 5);
            RightTextBox.Multiline = true;
            RightTextBox.Name = "RightTextBox";
            RightTextBox.ReadOnly = true;
            RightTextBox.ScrollBars = ScrollBars.Vertical;
            RightTextBox.Size = new Size(293, 494);
            RightTextBox.TabIndex = 5;
            // 
            // GeneratedTextForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(720, 812);
            Controls.Add(splitContainer1);
            Controls.Add(SplitColumnCheckBox);
            Controls.Add(CloseButton);
            Controls.Add(ClipboardButton);
            Controls.Add(SingleTextBox);
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GeneratedTextForm";
            ShowIcon = false;
            Text = "Generated Headers";
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox SingleTextBox;
        private Button ClipboardButton;
        private Button CloseButton;
        private CheckBox SplitColumnCheckBox;
        private SplitContainer splitContainer1;
        private TextBox LeftTextBox;
        private TextBox RightTextBox;
    }
}