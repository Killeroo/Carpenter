namespace SiteViewer.Forms
{
    partial class GeneratedTextForm
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
            TextBox = new TextBox();
            ClipboardButton = new Button();
            CloseButton = new Button();
            SuspendLayout();
            // 
            // TextBox
            // 
            TextBox.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            TextBox.Location = new Point(12, 12);
            TextBox.Multiline = true;
            TextBox.Name = "TextBox";
            TextBox.ReadOnly = true;
            TextBox.ScrollBars = ScrollBars.Vertical;
            TextBox.Size = new Size(480, 434);
            TextBox.TabIndex = 0;
            // 
            // ClipboardButton
            // 
            ClipboardButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            ClipboardButton.Location = new Point(261, 452);
            ClipboardButton.Name = "ClipboardButton";
            ClipboardButton.Size = new Size(137, 23);
            ClipboardButton.TabIndex = 1;
            ClipboardButton.Text = "Copy to Clipboard";
            ClipboardButton.UseVisualStyleBackColor = true;
            // 
            // CloseButton
            // 
            CloseButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            CloseButton.Location = new Point(404, 452);
            CloseButton.Name = "CloseButton";
            CloseButton.Size = new Size(88, 23);
            CloseButton.TabIndex = 2;
            CloseButton.Text = "Close";
            CloseButton.UseVisualStyleBackColor = true;
            CloseButton.Click += CloseButton_Click;
            // 
            // GeneratedTextForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(504, 487);
            Controls.Add(CloseButton);
            Controls.Add(ClipboardButton);
            Controls.Add(TextBox);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "GeneratedTextForm";
            ShowIcon = false;
            Text = "Generated Headers";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox TextBox;
        private Button ClipboardButton;
        private Button CloseButton;
    }
}