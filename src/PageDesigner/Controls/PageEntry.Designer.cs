namespace PageDesigner.Controls
{
    partial class PageEntry
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panel1 = new Panel();
            DirectoryLabel = new Label();
            PreviewButton = new Button();
            EditButton = new Button();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(DirectoryLabel);
            panel1.Controls.Add(PreviewButton);
            panel1.Controls.Add(EditButton);
            panel1.Location = new Point(3, 3);
            panel1.Name = "panel1";
            panel1.Size = new Size(502, 24);
            panel1.TabIndex = 0;
            // 
            // DirectoryLabel
            // 
            DirectoryLabel.AutoSize = true;
            DirectoryLabel.Location = new Point(3, 5);
            DirectoryLabel.Name = "DirectoryLabel";
            DirectoryLabel.Size = new Size(52, 15);
            DirectoryLabel.TabIndex = 2;
            DirectoryLabel.Text = "Example";
            // 
            // PreviewButton
            // 
            PreviewButton.Anchor = AnchorStyles.Right;
            PreviewButton.Location = new Point(424, 1);
            PreviewButton.Name = "PreviewButton";
            PreviewButton.Size = new Size(75, 23);
            PreviewButton.TabIndex = 1;
            PreviewButton.Text = "Preview";
            PreviewButton.UseVisualStyleBackColor = true;
            // 
            // EditButton
            // 
            EditButton.Anchor = AnchorStyles.Right;
            EditButton.Location = new Point(343, 1);
            EditButton.Name = "EditButton";
            EditButton.Size = new Size(75, 23);
            EditButton.TabIndex = 0;
            EditButton.Text = "Edit";
            EditButton.UseVisualStyleBackColor = true;
            EditButton.Click += EditButton_Click;
            // 
            // PageEntry
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            Controls.Add(panel1);
            Name = "PageEntry";
            Size = new Size(508, 30);
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panel1;
        private Label DirectoryLabel;
        private Button PreviewButton;
        private Button EditButton;
    }
}
