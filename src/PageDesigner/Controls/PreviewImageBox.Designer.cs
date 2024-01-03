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
            contextMenuStrip1 = new ContextMenuStrip(components);
            testToolStripMenuItem = new ToolStripMenuItem();
            anothewrWiderTestToolStripMenuItem = new ToolStripMenuItem();
            Panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)PictureBox).BeginInit();
            contextMenuStrip1.SuspendLayout();
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
            PictureBox.ContextMenuStrip = contextMenuStrip1;
            PictureBox.Location = new Point(3, 3);
            PictureBox.Name = "PictureBox";
            PictureBox.Size = new Size(141, 123);
            PictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
            PictureBox.TabIndex = 0;
            PictureBox.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { testToolStripMenuItem, anothewrWiderTestToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(179, 48);
            // 
            // testToolStripMenuItem
            // 
            testToolStripMenuItem.Name = "testToolStripMenuItem";
            testToolStripMenuItem.Size = new Size(178, 22);
            testToolStripMenuItem.Text = "test";
            // 
            // anothewrWiderTestToolStripMenuItem
            // 
            anothewrWiderTestToolStripMenuItem.Name = "anothewrWiderTestToolStripMenuItem";
            anothewrWiderTestToolStripMenuItem.Size = new Size(178, 22);
            anothewrWiderTestToolStripMenuItem.Text = "anothewr wider test";
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
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel Panel;
        private PictureBox PictureBox;
        private Label Label;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem testToolStripMenuItem;
        private ToolStripMenuItem anothewrWiderTestToolStripMenuItem;
    }
}
