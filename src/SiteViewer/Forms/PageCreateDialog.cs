using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SiteViewer.Forms
{
    public partial class PageCreateDialog : Form
    {
        public string WorkingDirectory = string.Empty;
        public string CurrentPageName = string.Empty;
        public bool OpenInDesigner = false;
        public bool CreateButtonPressed = false;

        public PageCreateDialog()
        {
            InitializeComponent();
        }
        public PageCreateDialog(string workingDirectory, string startingPageName)
        {
            InitializeComponent();

            WorkingDirectory = workingDirectory;
            PageNameTextBox.Text = startingPageName;
            StatusLabel.Text = "";
        }

        private void PageNameTextBox_TextChanged(object sender, EventArgs e)
        {
            string newPath = Path.Combine(WorkingDirectory, PageNameTextBox.Text);
            if (PageNameTextBox.Text.Length == 0)
            {
                StatusLabel.Text = "Please enter a page name!";
                CreateButton.Enabled = false;
            }
            else if (Directory.Exists(newPath))
            {
                StatusLabel.Text = "Page already exists!";
                CreateButton.Enabled = false;
            } 
            else
            {
                StatusLabel.Text = "";
                CreateButton.Enabled = true;
            }

            CurrentPageName = PageNameTextBox.Text;
        }

        private void CreateButton_Click(object sender, EventArgs e)
        {
            CreateButtonPressed = true;
            Close();
        }

        private void OpenInDesignerCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            OpenInDesigner = OpenInDesignerCheckBox.Checked;
        }
    }
}
