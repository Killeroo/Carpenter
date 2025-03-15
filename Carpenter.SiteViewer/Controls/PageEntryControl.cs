using Accessibility;

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
using SiteViewer.Forms;

using PageDesigner.Forms;

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
            None,
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
        /// A reference of the root template used to generate all pages
        /// </summary>
        private Template _template;

        /// <summary>
        /// A reference to the site currently being used for all pages
        /// </summary>
        private Site _site;

        /// <summary>
        /// Path to the directory of the page that this control represents
        /// </summary>
        private string _directoryPath;

        /// <summary>
        /// The state of the last page webpage build
        /// </summary>
        private BuildState _buildState;

        public PageEntryControl(string directoryPath, ButtonTypes buttonType, Template template, Site site)
        {
            InitializeComponent();

            Debug.Assert(template != null);
            Debug.Assert(site != null);
            Debug.Assert(Directory.Exists(directoryPath));

            _directoryPath = directoryPath;
            _template = template;
            _site = site;
            DirectoryLabel.Text = Path.GetFileName(directoryPath);

            SetControlButtonType(buttonType);

            if (buttonType == ButtonTypes.Create)
            {
                // If we are showing the create button then we don't have a PAGE file, so no point in showing a build status
                SetBuildStatus(BuildState.None);
            }
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
                case BuildState.None:    StatusButton.BackColor = Color.LightGray; break;
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
            PageDesignerForm form = new(_directoryPath, _site.GetSiteRoot());
            form.Show();
#else
            ProcessStartInfo startInfo = new(kPageDesignerAppName);
            startInfo.Arguments = $"\"{_directoryPath}\" \"{_site.GetPath()}\"";
            Process.Start(startInfo);
#endif
        }

        // TODO: Move to main form
        private void PreviewButton_Click(object sender, EventArgs e)
        {
            string schemaPath = Path.Combine(_directoryPath, Config.kSchemaFileName);
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
                if (_template.GeneratePreviewHtmlForSchema(pageSchema, _site, _directoryPath, out string fileName))
                {
                    // Open it with default app
                    string previewPath = Path.Combine(_directoryPath, fileName);
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
            PageDesignerForm form = new(_directoryPath, _site.GetSiteRoot());
            form.Show();
#else
            ProcessStartInfo startInfo = new(kPageDesignerAppName);
            startInfo.Arguments = $"\"{_directoryPath}\" \"{_site.GetPath()}\"";
            Process.Start(startInfo);
#endif
        }
    }
}
