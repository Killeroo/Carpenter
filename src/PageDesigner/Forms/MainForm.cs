using PageDesigner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageDesigner.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

        }

        private void button1_Click(object sender, EventArgs e)
        {
            PageEntry entry = new PageEntry();
            // entry.Size = new Size(tableLayoutPanel1.Width, entry.Height);
            //Button button = new Button();
            //button.AutoSize = true;
            //button.Text = "This is a test";

            TableLayoutPanel.Controls.Add(entry, 0, 0);

            //tableLayoutPanel1.ColumnStyles.Clear();
            //for (int i = 0; i < tableLayoutPanel1.ColumnCount; i++)
            //{
            //    tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            //}

            //tableLayoutPanel1.RowStyles.Clear();
            //for (int i = 0; i < tableLayoutPanel1.RowCount; i++)
            //{
            //    tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            //}
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string path = PathTextBox.Text;

            if (Directory.Exists(path) == false)
            {
                return;
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                bool schemaPresent = File.Exists(Path.Combine(path, directory, "SCHEMA"));

                PageEntry entry = new PageEntry(directory, !schemaPresent);
                TableLayoutPanel.Controls.Add(entry, 0, 0);
            }
        }
    }
}
