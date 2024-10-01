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
    public partial class GeneratedTextForm : Form
    {
        public GeneratedTextForm()
        {
            InitializeComponent();
        }

        public void SetText(string text)
        {
            TextBox.Text = text;
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
