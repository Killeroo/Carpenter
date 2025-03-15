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
            components = new System.ComponentModel.Container();
            Panel = new Panel();
            Label = new Label();
            PictureBox = new PictureBox();
            ImageContextMenuStrip = new ContextMenuStrip(components);
            AddImageToolStripMenuItem = new ToolStripMenuItem();
            InsertToolStripMenuItem = new ToolStripMenuItem();
            ReplaceImageToolStripMenuItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            pageImageToolStripMenuItem = new ToolStripMenuItem();
            Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox).BeginInit();
            ImageContextMenuStrip.SuspendLayout();
            SuspendLayout();
            // 
            // Panel
            // 
            Panel.Controls.Add(Label);
            Panel.Controls.Add(PictureBox);
            Panel.Location = new Point(3, 3);
            Panel.Name = "Panel";
            Panel.Size = new Size(144, 144);
            Panel.TabIndex = 0;
            // 
            // Label
            // 
            Label.AutoSize = true;
            Label.Location = new Point(-3, 129);
            Label.MinimumSize = new Size(150, 0);
            Label.Name = "Label";
            Label.Size = new Size(150, 15);
            Label.TabIndex = 1;
            Label.Text = "label1";
            Label.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // PictureBox
            // 
            PictureBox.ContextMenuStrip = ImageContextMenuStrip;
            PictureBox.Location = new Point(3, 3);
            PictureBox.Name = "PictureBox";
            PictureBox.Size = new Size(141, 123);
            PictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            PictureBox.TabIndex = 0;
            PictureBox.TabStop = false;
            // 
            // ImageContextMenuStrip
            // 
            ImageContextMenuStrip.Items.AddRange(new ToolStripItem[] { AddImageToolStripMenuItem, InsertToolStripMenuItem, ReplaceImageToolStripMenuItem, toolStripSeparator1, pageImageToolStripMenuItem });
            ImageContextMenuStrip.Name = "contextMenuStrip1";
            ImageContextMenuStrip.Size = new Size(181, 120);
            // 
            // AddImageToolStripMenuItem
            // 
            AddImageToolStripMenuItem.Name = "AddImageToolStripMenuItem";
            AddImageToolStripMenuItem.Size = new Size(180, 22);
            AddImageToolStripMenuItem.Text = "Add";
            AddImageToolStripMenuItem.Click += AddImageToolStripMenuItem_Click;
            // 
            // InsertToolStripMenuItem
            // 
            InsertToolStripMenuItem.Name = "InsertToolStripMenuItem";
            InsertToolStripMenuItem.Size = new Size(180, 22);
            InsertToolStripMenuItem.Text = "Insert";
            InsertToolStripMenuItem.Click += InsertToolStripMenuItem_Click;
            // 
            // ReplaceImageToolStripMenuItem
            // 
            ReplaceImageToolStripMenuItem.Name = "ReplaceImageToolStripMenuItem";
            ReplaceImageToolStripMenuItem.Size = new Size(180, 22);
            ReplaceImageToolStripMenuItem.Text = "Replace";
            ReplaceImageToolStripMenuItem.Click += ReplaceImageToolStripMenuItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(177, 6);
            // 
            // pageImageToolStripMenuItem
            // 
            pageImageToolStripMenuItem.Name = "pageImageToolStripMenuItem";
            pageImageToolStripMenuItem.Size = new Size(180, 22);
            pageImageToolStripMenuItem.Text = "Page Thumbnail";
            pageImageToolStripMenuItem.Click += pageImageToolStripMenuItem_Click;
            // 
            // PreviewImageBox
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(Panel);
            Name = "PreviewImageBox";
            Load += PreviewImageBox_Load;
            Panel.ResumeLayout(false);
            Panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox).EndInit();
            ImageContextMenuStrip.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel Panel;
        private PictureBox PictureBox;
        private Label Label;
        private ContextMenuStrip ImageContextMenuStrip;
        private ToolStripMenuItem AddImageToolStripMenuItem;
        private ToolStripMenuItem ReplaceImageToolStripMenuItem;
        private ToolStripMenuItem InsertToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripMenuItem pageImageToolStripMenuItem;
    }
}
