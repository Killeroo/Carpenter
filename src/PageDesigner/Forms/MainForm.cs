using System;
using System.Collections.Generic;
using System.ComponentModel;
using Carpenter;
using PageDesigner.Controls;
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
        private const string kTemplateFilename = "template.html";

        private Template _template = new Template();

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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            string path = PathTextBox.Text;

            // Sanity check path
            if (Directory.Exists(path) == false)
            {
                return;
            }

            // Find template at root
            string templatePath = Path.Combine(path, kTemplateFilename);
            if (File.Exists(templatePath))
            {
                _template.Load(templatePath);
            }

            foreach (string directory in Directory.GetDirectories(path))
            {
                bool schemaPresent = File.Exists(Path.Combine(path, directory, "SCHEMA"));

                PageEntry entry = new PageEntry(directory, !schemaPresent, _template);
                TableLayoutPanel.Controls.Add(entry, 0, 0);
            }
        }
    }
}
