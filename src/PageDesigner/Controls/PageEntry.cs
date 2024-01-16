using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Carpenter;

namespace PageDesigner.Controls
{
    public partial class PageEntry : UserControl
    {
        private Template _template;
        private string _directoryPath;

        public PageEntry()
        {
            InitializeComponent();
        }

        public PageEntry(string directoryPath, bool createButton, Template template)
        {
            InitializeComponent();

            // TODO: Check yeah yeah yeah check some stuff...
            _directoryPath = directoryPath;
            _template = template;
            
            DirectoryLabel.Text = Path.GetFileName(directoryPath);

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

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            string schemaPath = Path.Combine(_directoryPath, "SCHEMA");
            if (File.Exists(schemaPath) == false)
            {
                return;
            }

            if (_template == null)
            {
                return;
            }

            string temp = Path.GetTempPath();
            using (Schema pageSchema = new(schemaPath))
            {
                // Generate preview page
                if (_template.Generate(pageSchema, _directoryPath, true))
                {
                    string originalOutputFile = pageSchema.OptionValues[Schema.Option.OutputFilename];
                    string previewName = Path.GetFileNameWithoutExtension(originalOutputFile) + "_preview";
                    string previewPath = Path.Combine(_directoryPath, previewName + Path.GetExtension(originalOutputFile));

                    // Open it with default app
                    if (File.Exists(previewPath))
                    { 
                        Process.Start(new ProcessStartInfo(previewPath)
                        {
                            UseShellExecute = true
                        });
                    }
                }

            }
        }
    }
}
