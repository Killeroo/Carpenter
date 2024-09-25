namespace SiteViewer.Forms
{
    partial class PageCreateDialog
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
            PageNameTextBox = new TextBox();
            CreateButton = new Button();
            StatusLabel = new Label();
            OpenInDesignerCheckBox = new CheckBox();
            SuspendLayout();
            // 
            // PageNameTextBox
            // 
            PageNameTextBox.Location = new Point(12, 12);
            PageNameTextBox.Name = "PageNameTextBox";
            PageNameTextBox.Size = new Size(360, 27);
            PageNameTextBox.TabIndex = 0;
            PageNameTextBox.TextChanged += PageNameTextBox_TextChanged;
            // 
            // CreateButton
            // 
            CreateButton.Location = new Point(281, 75);
            CreateButton.Name = "CreateButton";
            CreateButton.Size = new Size(94, 29);
            CreateButton.TabIndex = 1;
            CreateButton.Text = "Create";
            CreateButton.UseVisualStyleBackColor = true;
            CreateButton.Click += CreateButton_Click;
            // 
            // StatusLabel
            // 
            StatusLabel.AutoSize = true;
            StatusLabel.ForeColor = Color.Firebrick;
            StatusLabel.Location = new Point(12, 79);
            StatusLabel.Name = "StatusLabel";
            StatusLabel.Size = new Size(39, 20);
            StatusLabel.TabIndex = 3;
            StatusLabel.Text = "-----";
            // 
            // OpenInDesignerCheckBox
            // 
            OpenInDesignerCheckBox.AutoSize = true;
            OpenInDesignerCheckBox.Location = new Point(12, 45);
            OpenInDesignerCheckBox.Name = "OpenInDesignerCheckBox";
            OpenInDesignerCheckBox.Size = new Size(213, 24);
            OpenInDesignerCheckBox.TabIndex = 4;
            OpenInDesignerCheckBox.Text = "Open new page in designer";
            OpenInDesignerCheckBox.UseVisualStyleBackColor = true;
            OpenInDesignerCheckBox.CheckedChanged += OpenInDesignerCheckBox_CheckedChanged;
            // 
            // PageCreateDialog
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(387, 114);
            Controls.Add(OpenInDesignerCheckBox);
            Controls.Add(StatusLabel);
            Controls.Add(CreateButton);
            Controls.Add(PageNameTextBox);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "PageCreateDialog";
            ShowIcon = false;
            ShowInTaskbar = false;
            SizeGripStyle = SizeGripStyle.Hide;
            Text = "Create new page..";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox PageNameTextBox;
        private Button CreateButton;
        private Label StatusLabel;
        private CheckBox OpenInDesignerCheckBox;
    }
}