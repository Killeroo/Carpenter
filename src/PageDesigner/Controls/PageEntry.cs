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
        public enum Status
        {
            SUCCESS,
            FAILURE,
            PENDING
        }

        private Template _template;
        private string _directoryPath;
        private Status _status;

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
            DirectoryName = Path.GetFileName(directoryPath);

            ToggleButtons(createButton);
        }

        private string DirectoryName;
        // TODO: Null check
        public string GetDirectoryName() { return DirectoryLabel.Text; }
        public string GetDirectoryPath() { return _directoryPath; }

        public void SetStatus(Status newStatus)
        {
            if (_status == newStatus)
            {
                return;
            }

            switch (newStatus)
            {
                case Status.SUCCESS: StatusButton.BackColor = Color.LightGreen; break;
                case Status.FAILURE: StatusButton.BackColor = Color.Red; break;
                case Status.PENDING: StatusButton.BackColor = Color.Yellow; break;
            }

            _status = newStatus;
        }

        public Status GetStatus() => _status;

        private void ToggleButtons(bool showCreateButton)
        {
            if (showCreateButton)
            {
                CreateButton.Visible = true;

                EditButton.Visible = false;
                PreviewButton.Visible = false;
            }
            else
            {
                CreateButton.Visible = false;

                EditButton.Visible = true;
                PreviewButton.Visible = true;
            }
        }

        private void EditButton_Click(object sender, EventArgs e)
        {
            PageDesignerForm form = new(_directoryPath, _template);
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

        // TODO: Don't repeat
        private void CreateButton_Click(object sender, EventArgs e)
        {
            PageDesignerForm form = new(_directoryPath, _template);
            form.ShowDialog();
        }
    }
}
