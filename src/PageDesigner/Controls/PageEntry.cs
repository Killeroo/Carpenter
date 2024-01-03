using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageDesigner.Controls
{
    public partial class PageEntry : UserControl
    {
        private string _directoryPath;
        private string _directoryName;

        public PageEntry()
        {
            InitializeComponent();
        }

        public PageEntry(string directoryPath, bool createButton)
        {
            InitializeComponent();

            // TODO: Check yeah yeah yeah check some stuff...
            _directoryName = Path.GetFileName(directoryPath);
            _directoryPath = directoryPath;
            
            DirectoryLabel.Text = _directoryName;

            if (createButton)
            {
                EditButton.Text = "Create";
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            PageDesignerForm form = new (_directoryPath);
            form.ShowDialog();
        }
    }
}
