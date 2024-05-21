namespace PageDesigner
{
    partial class PreviewImageBox
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
            this.components = new System.ComponentModel.Container();
            this.Panel = new System.Windows.Forms.Panel();
            this.Label = new System.Windows.Forms.Label();
            this.PictureBox = new System.Windows.Forms.PictureBox();
            this.ImageContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.AddImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.InsertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReplaceImageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).BeginInit();
            this.ImageContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel
            // 
            this.Panel.Controls.Add(this.Label);
            this.Panel.Controls.Add(this.PictureBox);
            this.Panel.Location = new System.Drawing.Point(3, 3);
            this.Panel.Name = "Panel";
            this.Panel.Size = new System.Drawing.Size(144, 144);
            this.Panel.TabIndex = 0;
            // 
            // Label
            // 
            this.Label.AutoSize = true;
            this.Label.Location = new System.Drawing.Point(-3, 129);
            this.Label.MinimumSize = new System.Drawing.Size(150, 0);
            this.Label.Name = "Label";
            this.Label.Size = new System.Drawing.Size(150, 15);
            this.Label.TabIndex = 1;
            this.Label.Text = "label1";
            this.Label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // PictureBox
            // 
            this.PictureBox.ContextMenuStrip = this.ImageContextMenuStrip;
            this.PictureBox.Location = new System.Drawing.Point(3, 3);
            this.PictureBox.Name = "PictureBox";
            this.PictureBox.Size = new System.Drawing.Size(141, 123);
            this.PictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
            this.PictureBox.TabIndex = 0;
            this.PictureBox.TabStop = false;
            // 
            // ImageContextMenuStrip
            // 
            this.ImageContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddImageToolStripMenuItem,
            this.InsertToolStripMenuItem,
            this.ReplaceImageToolStripMenuItem});
            this.ImageContextMenuStrip.Name = "contextMenuStrip1";
            this.ImageContextMenuStrip.Size = new System.Drawing.Size(116, 70);
            // 
            // AddImageToolStripMenuItem
            // 
            this.AddImageToolStripMenuItem.Name = "AddImageToolStripMenuItem";
            this.AddImageToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.AddImageToolStripMenuItem.Text = "Add";
            this.AddImageToolStripMenuItem.Click += new System.EventHandler(this.AddImageToolStripMenuItem_Click);
            // 
            // InsertToolStripMenuItem
            // 
            this.InsertToolStripMenuItem.Name = "InsertToolStripMenuItem";
            this.InsertToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.InsertToolStripMenuItem.Text = "Insert";
            this.InsertToolStripMenuItem.Click += new System.EventHandler(this.InsertToolStripMenuItem_Click);
            // 
            // ReplaceImageToolStripMenuItem
            // 
            this.ReplaceImageToolStripMenuItem.Name = "ReplaceImageToolStripMenuItem";
            this.ReplaceImageToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.ReplaceImageToolStripMenuItem.Text = "Replace";
            this.ReplaceImageToolStripMenuItem.Click += new System.EventHandler(this.ReplaceImageToolStripMenuItem_Click);
            // 
            // PreviewImageBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.Panel);
            this.Name = "PreviewImageBox";
            this.Load += new System.EventHandler(this.PreviewImageBox_Load);
            this.Panel.ResumeLayout(false);
            this.Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PictureBox)).EndInit();
            this.ImageContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Panel Panel;
        private PictureBox PictureBox;
        private Label Label;
        private ContextMenuStrip ImageContextMenuStrip;
        private ToolStripMenuItem AddImageToolStripMenuItem;
        private ToolStripMenuItem ReplaceImageToolStripMenuItem;
        private ToolStripMenuItem InsertToolStripMenuItem;
    }
}
