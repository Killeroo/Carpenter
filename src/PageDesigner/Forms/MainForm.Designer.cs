namespace PageDesigner.Forms
{
    partial class MainForm
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
            OpenFolderButton = new Button();
            TableLayoutPanel = new TableLayoutPanel();
            PathTextBox = new TextBox();
            StatusStrip = new StatusStrip();
            StateToolStripStatusLabel = new ToolStripStatusLabel();
            ToolStripProgressBar = new ToolStripProgressBar();
            StatusStrip.SuspendLayout();
            SuspendLayout();
            // 
            // OpenFolderButton
            // 
            OpenFolderButton.Location = new Point(512, 8);
            OpenFolderButton.Name = "OpenFolderButton";
            OpenFolderButton.Size = new Size(28, 23);
            OpenFolderButton.TabIndex = 0;
            OpenFolderButton.Text = "...";
            OpenFolderButton.UseVisualStyleBackColor = true;
            OpenFolderButton.Click += button1_Click;
            // 
            // TableLayoutPanel
            // 
            TableLayoutPanel.AutoScroll = true;
            TableLayoutPanel.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            TableLayoutPanel.ColumnCount = 1;
            TableLayoutPanel.ColumnStyles.Add(new ColumnStyle());
            TableLayoutPanel.Location = new Point(12, 37);
            TableLayoutPanel.Name = "TableLayoutPanel";
            TableLayoutPanel.RowCount = 1;
            TableLayoutPanel.RowStyles.Add(new RowStyle());
            TableLayoutPanel.Size = new Size(529, 346);
            TableLayoutPanel.TabIndex = 1;
            // 
            // PathTextBox
            // 
            PathTextBox.Location = new Point(12, 8);
            PathTextBox.Name = "PathTextBox";
            PathTextBox.Size = new Size(494, 23);
            PathTextBox.TabIndex = 2;
            PathTextBox.Text = "C:\\Users\\Shadowfax\\Desktop\\Website-20240103T211813Z-001\\Website\\photos";
            // 
            // StatusStrip
            // 
            StatusStrip.Items.AddRange(new ToolStripItem[] { StateToolStripStatusLabel, ToolStripProgressBar });
            StatusStrip.Location = new Point(0, 402);
            StatusStrip.Name = "StatusStrip";
            StatusStrip.Size = new Size(553, 22);
            StatusStrip.TabIndex = 3;
            StatusStrip.Text = "statusStrip1";
            // 
            // StateToolStripStatusLabel
            // 
            StateToolStripStatusLabel.Name = "StateToolStripStatusLabel";
            StateToolStripStatusLabel.Size = new Size(141, 17);
            StateToolStripStatusLabel.Text = "Site generated @ 10.00.00";
            // 
            // ToolStripProgressBar
            // 
            ToolStripProgressBar.Name = "ToolStripProgressBar";
            ToolStripProgressBar.Size = new Size(100, 16);
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(553, 424);
            Controls.Add(StatusStrip);
            Controls.Add(PathTextBox);
            Controls.Add(TableLayoutPanel);
            Controls.Add(OpenFolderButton);
            Name = "MainForm";
            Text = "Carpenter";
            Load += MainForm_Load;
            StatusStrip.ResumeLayout(false);
            StatusStrip.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button OpenFolderButton;
        private TableLayoutPanel TableLayoutPanel;
        private TextBox PathTextBox;
        private StatusStrip StatusStrip;
        private ToolStripStatusLabel StateToolStripStatusLabel;
        private ToolStripProgressBar ToolStripProgressBar;
    }
}