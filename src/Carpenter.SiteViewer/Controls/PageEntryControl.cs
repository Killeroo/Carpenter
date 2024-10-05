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

using PageDesigner.Forms;

using Carpenter;
using SiteViewer.Forms;
using Accessibility;

namespace SiteViewer.Controls
{
    /// <summary>
    /// Form control that represents a single page in a list, allows the users to generate a preview or edit the current page
    /// </summary>
    public partial class PageEntryControl : UserControl
    {
        private const string kPageDesignerAppName = "PageDesigner.exe";

        public enum BuildState
        {
            Success,
            Failure,
            Pending
        }

        public enum ButtonTypes
        {
            Create,
            Edit
        }

        /// <summary>
        /// A copy of the root template used to generate all pages
        /// </summary>
        private Template _template;

        /// <summary>
        /// Path to the directory of the page that this control represents
        /// </summary>
        private string _directoryPath;

        /// <summary>
        /// The state of the last page webpage build
        /// </summary>
        private BuildState _buildState;

        /// <summary>
        /// A reference to the SiteViwerForm that owns this control
        /// </summary>
        private SiteViewerForm _owner;

        public PageEntryControl(string directoryPath, bool showCreateButton, Template template)
        {
            InitializeComponent();

            Debug.Assert(template != null);
            Debug.Assert(Directory.Exists(directoryPath));
            Debug.Assert(ParentForm is SiteViewerForm);

            _directoryPath = directoryPath;
            _template = template;
            _owner = ParentForm as SiteViewerForm;
            DirectoryLabel.Text = Path.GetFileName(directoryPath);

            SetControlButtonType(ButtonTypes.Create);
        }

        /// <summary>
        /// Some simple accessors for the paths that this control represents
        /// </summary>
        public string GetDirectoryName() => Path.GetFileName(_directoryPath);
        public string GetDirectoryPath() => _directoryPath;

        public BuildState GetBuildState() => _buildState;

        /// <summary>
        /// Sets the webpage build status for this control
        /// </summary>
        /// <param name="newState">The state to display</param>
        public void SetBuildStatus(BuildState newState)
        {
            if (_buildState == newState)
            {
                return;
            }

            switch (newState)
            {
                case BuildState.Success: StatusButton.BackColor = Color.LightGreen; break;
                case BuildState.Failure: StatusButton.BackColor = Color.Red; break;
                case BuildState.Pending: StatusButton.BackColor = Color.Yellow; break;
            }

            _buildState = newState;
        }

        /// <summary>
        /// Enable or disable all the buttons on this control
        /// </summary>
        /// <remarks></remarks>
        public void EnableButtons(bool shouldEnable)
        {
            EditButton.Enabled = shouldEnable;
            CreateButton.Enabled = shouldEnable;
            PreviewButton.Enabled = shouldEnable;
        }

        /// <summary>
        /// Sets which set of buttons should be show on the control
        /// </summary>
        private void SetControlButtonType(ButtonTypes buttonType)
        {
            if (buttonType == ButtonTypes.Create)
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
#if true
            PageDesignerForm form = new(_directoryPath, _template.FilePath);
            form.Show();
#else
            ProcessStartInfo startInfo = new(kPageDesignerAppName);
            startInfo.Arguments = $"\"{_directoryPath}\" \"{_template.FilePath}\"";
            Process.Start(startInfo);
#endif
        }

        // TODO: Move to main form
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
                    string originalOutputFile = pageSchema.OptionValues[Schema.Options.OutputFilename];
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

        // TODO: TODO: Move to main form
        private void CreateButton_Click(object sender, EventArgs e)
        {
#if true
            PageDesignerForm form = new(_directoryPath, _template.FilePath);
            form.Show();
#else
            ProcessStartInfo startInfo = new(kPageDesignerAppName);
            startInfo.Arguments = $"\"{_directoryPath}\" \"{_template.FilePath}\"";
            Process.Start(startInfo);
#endif
        }
    }
}
