using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Carpenter.SiteViewer.Forms
{
    public partial class ValidationFailedForm : Form
    {
        public ValidationFailedForm(string errorMessage)
        {
            InitializeComponent();

            ErrorTextBox.Text = errorMessage;
        }

        private void ValidationFailedForm_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.DrawIcon(SystemIcons.Warning, 16, 16);
        }

        private void YesButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Yes;
            Close();
        }

        private void NoButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.No;
            Close();
        }
    }
}
