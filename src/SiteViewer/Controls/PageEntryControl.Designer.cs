namespace SiteViewer.Controls
{
    partial class PageEntryControl
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
            this.panel1 = new System.Windows.Forms.Panel();
            this.StatusButton = new System.Windows.Forms.Button();
            this.CreateButton = new System.Windows.Forms.Button();
            this.DirectoryLabel = new System.Windows.Forms.Label();
            this.PreviewButton = new System.Windows.Forms.Button();
            this.EditButton = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panel1.Controls.Add(this.StatusButton);
            this.panel1.Controls.Add(this.CreateButton);
            this.panel1.Controls.Add(this.DirectoryLabel);
            this.panel1.Controls.Add(this.PreviewButton);
            this.panel1.Controls.Add(this.EditButton);
            this.panel1.Location = new System.Drawing.Point(3, 3);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(502, 24);
            this.panel1.TabIndex = 0;
            // 
            // StatusButton
            // 
            this.StatusButton.BackColor = System.Drawing.Color.Yellow;
            this.StatusButton.Enabled = false;
            this.StatusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StatusButton.Location = new System.Drawing.Point(3, 3);
            this.StatusButton.Name = "StatusButton";
            this.StatusButton.Size = new System.Drawing.Size(22, 19);
            this.StatusButton.TabIndex = 4;
            this.StatusButton.UseVisualStyleBackColor = false;
            // 
            // CreateButton
            // 
            this.CreateButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.CreateButton.Location = new System.Drawing.Point(424, 1);
            this.CreateButton.Name = "CreateButton";
            this.CreateButton.Size = new System.Drawing.Size(75, 23);
            this.CreateButton.TabIndex = 3;
            this.CreateButton.Text = "Create";
            this.CreateButton.UseVisualStyleBackColor = true;
            this.CreateButton.Click += new System.EventHandler(this.CreateButton_Click);
            // 
            // DirectoryLabel
            // 
            this.DirectoryLabel.AutoSize = true;
            this.DirectoryLabel.Location = new System.Drawing.Point(31, 5);
            this.DirectoryLabel.Name = "DirectoryLabel";
            this.DirectoryLabel.Size = new System.Drawing.Size(52, 15);
            this.DirectoryLabel.TabIndex = 2;
            this.DirectoryLabel.Text = "Example";
            // 
            // PreviewButton
            // 
            this.PreviewButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.PreviewButton.Location = new System.Drawing.Point(424, 1);
            this.PreviewButton.Name = "PreviewButton";
            this.PreviewButton.Size = new System.Drawing.Size(75, 23);
            this.PreviewButton.TabIndex = 1;
            this.PreviewButton.Text = "Preview";
            this.PreviewButton.UseVisualStyleBackColor = true;
            this.PreviewButton.Click += new System.EventHandler(this.PreviewButton_Click);
            // 
            // EditButton
            // 
            this.EditButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.EditButton.Location = new System.Drawing.Point(343, 1);
            this.EditButton.Name = "EditButton";
            this.EditButton.Size = new System.Drawing.Size(75, 23);
            this.EditButton.TabIndex = 0;
            this.EditButton.Text = "Edit";
            this.EditButton.UseVisualStyleBackColor = true;
            this.EditButton.Click += new System.EventHandler(this.EditButton_Click);
            // 
            // PageEntry
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.panel1);
            this.Name = "PageEntry";
            this.Size = new System.Drawing.Size(508, 30);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Panel panel1;
        private Label DirectoryLabel;
        private Button PreviewButton;
        private Button EditButton;
        private Button CreateButton;
        private Button StatusButton;
    }
}
