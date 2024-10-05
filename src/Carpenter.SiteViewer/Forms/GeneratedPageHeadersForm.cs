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
    public partial class GeneratedPageHeadersForm : Form
    {
        public GeneratedPageHeadersForm()
        {
            InitializeComponent();

            RightTextBox.Visible = false;
            LeftTextBox.Visible = false;
            SingleTextBox.Visible = true;
        }

        public void SetSingleColumn(string text)
        {
            SingleTextBox.Text = text;
        }

        public void SetLeftText(string text)
        {
            LeftTextBox.Text = text;
        }

        public void SetRightText(string text)
        {
            RightTextBox.Text = text;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SplitColumnCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            RightTextBox.Visible = SplitColumnCheckBox.Checked;
            LeftTextBox.Visible = SplitColumnCheckBox.Checked;
            SingleTextBox.Visible = !SplitColumnCheckBox.Checked;

            RightTextBox.Enabled = SplitColumnCheckBox.Checked;
            LeftTextBox.Enabled = SplitColumnCheckBox.Checked;
            LeftTextBox.Enabled = SplitColumnCheckBox.Checked;
            SingleTextBox.Enabled = !SplitColumnCheckBox.Checked;
        }
    }
}
