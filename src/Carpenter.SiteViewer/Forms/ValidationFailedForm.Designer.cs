namespace Carpenter.SiteViewer.Forms
{
    partial class ValidationFailedForm
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
            label1 = new Label();
            ErrorTextBox = new TextBox();
            label2 = new Label();
            NoButton = new Button();
            YesButton = new Button();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(69, 21);
            label1.Name = "label1";
            label1.Size = new Size(232, 15);
            label1.TabIndex = 0;
            label1.Text = "Validation failed for the following Schemas";
            // 
            // ErrorTextBox
            // 
            ErrorTextBox.Location = new Point(69, 50);
            ErrorTextBox.Multiline = true;
            ErrorTextBox.Name = "ErrorTextBox";
            ErrorTextBox.ReadOnly = true;
            ErrorTextBox.ScrollBars = ScrollBars.Vertical;
            ErrorTextBox.Size = new Size(392, 126);
            ErrorTextBox.TabIndex = 1;
            ErrorTextBox.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(69, 191);
            label2.Name = "label2";
            label2.Size = new Size(255, 15);
            label2.TabIndex = 2;
            label2.Text = "Would you like to continue with the operation?";
            // 
            // NoButton
            // 
            NoButton.Location = new Point(386, 216);
            NoButton.Name = "NoButton";
            NoButton.Size = new Size(75, 23);
            NoButton.TabIndex = 3;
            NoButton.Text = "No";
            NoButton.UseVisualStyleBackColor = true;
            NoButton.Click += NoButton_Click;
            // 
            // YesButton
            // 
            YesButton.Location = new Point(305, 216);
            YesButton.Name = "YesButton";
            YesButton.Size = new Size(75, 23);
            YesButton.TabIndex = 0;
            YesButton.Text = "Yes";
            YesButton.UseVisualStyleBackColor = true;
            YesButton.Click += YesButton_Click;
            // 
            // ValidationFailedForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(473, 251);
            ControlBox = false;
            Controls.Add(YesButton);
            Controls.Add(NoButton);
            Controls.Add(label2);
            Controls.Add(ErrorTextBox);
            Controls.Add(label1);
            Name = "ValidationFailedForm";
            Text = "ValidationFailedForm";
            Paint += ValidationFailedForm_Paint;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox ErrorTextBox;
        private Label label2;
        private Button NoButton;
        private Button YesButton;
    }
}