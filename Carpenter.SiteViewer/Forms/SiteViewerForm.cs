using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

using JpegMetadataExtractor;

using Carpenter;
using Carpenter.SiteViewer;

using SiteViewer;
using SiteViewer.Controls;

using PageDesigner.Forms;
using static System.Windows.Forms.AxHost;
using Carpenter.SiteViewer.Forms;


namespace SiteViewer.Forms
{
    /// <summary>
    /// Form that displays all pages in a site/directory. Allows the user to perform site wide actions such as generating and editing individual pages
    /// </summary>
    public partial class SiteViewerForm : Form
    {
        /// <summary>
        /// The current state of site generation, used to display progress to the user on the site viewer form as site is generated
        /// </summary>
        private class SiteGenerationState
        {
            public List<string> FailedPaths = new();
            public List<string> SuccessfulPaths = new();

            public string Message = string.Empty;
        }

        private const string kPageDesignerAppName = "PageDesigner.exe";

        private FileSystemWatcher _fileSystemWatcher = null;
        private Template _template = new Template();
        private Site _site = new Site();
        private string _rootPath = string.Empty;
        private string _lastCreatedPageName = string.Empty;
        private string _templatePath = string.Empty;

        public SiteViewerForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Try and load pages and template file at a path, silently fail otherwise.
        /// This is because this is called everytime new path text is added 
        /// </summary>
        /// <param name="path"></param>
        private void TryLoadDirectory(string path)
        {
            // Sanity check path
            if (Directory.Exists(path) == false)
            {
                return;
            }

            _rootPath = path;

            // Try and load the site file first
            _site = new Site();
            if (_site.TryLoad(_rootPath) == false)
            {
                return;
            }

            // The site file contains the template file location
            _template = new Template();
            _templatePath = _site.TemplatePath;
            if (_template.TryLoad(_site.TemplatePath) == false)
            {
                return;
            }

            // We have loaded everything we need! Render the pages in the root dir
            RefreshPageList();

            // Save path to settings
            Settings.Default.LastLoadedRootPath = path;
            Settings.Default.Save();

            // Monitor root for any changes so we can update the page list
            if (_fileSystemWatcher == null)
            {
                SetupFileSystemWatcher(path);
            }
            else
            {
                _fileSystemWatcher.Path = path;
            }
        }

        /// <summary>
        /// Opens the PageDesigner form/process in a specific directory with a SCHEMA file
        /// </summary>
        /// <param name="path">Path to run page designer in, will attempt to open a SCHEMA file in the path if one is available or will create a new one</param>
        public void RunPageDesigner(string path)
        {
            if (_template == null)
            {
                Debug.Write("Could not open designer, template not loaded");
                return;
            }

#if false
            PageDesignerForm form = new(path, _template, _site);
            form.ShowDialog();
#else
            ProcessStartInfo startInfo = new(kPageDesignerAppName);
            startInfo.Arguments = $"\"{path}\" \"{_rootPath}\"";
            Process.Start(startInfo);
#endif
        }

        /// <summary>
        /// Generates html for all pages in site in a background worker (ensures that the form is still responsive while generation is running)
        /// </summary>
        private void GenerateAllPagesInBackgroundWorker()
        {
            ResetUIForGenerateOrPublish();
            GenerateSiteBackgroundWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Loop through each directory in the rootPath and remove jpg files that aren't referenced in a schema
        /// </summary>
        private void RemoveUnusedImages()
        {
            int removedFiles = _site.RemoveAllUnusedImages();
            StateToolStripStatusLabel.Text = $"{removedFiles} files removed";
        }

        /// <summary>
        /// Generate headers for each page in the rootPath/site.
        /// </summary>
        /// <remarks>
        /// A header in this context is a small snippet of HTML for each page that can be used on a landing page.
        /// </remarks>
        private void GeneratePageHeaders()
        {
            if (Directory.Exists(PathTextBox.Text) == false)
            {
                return;
            }

            // TODO: Should probably put this somewhere else
            const string kHtmlTemplate = @"
<div class=""container"">
    <a href=""%PAGE_URL/"">
        <img class=""preview_image"" src=""%PAGE_URL/%THUMBNAIL"" width=""%WIDTH"" height=""%HEIGHT"" style=""width:100%"">

        <div style=""margin-left: 1em;"">
            <h3 style=""margin-top: 0; margin-bottom: 0;"">%TITLE</h3>
            <p style=""color: black"">%MONTH %YEAR - %CAMERA - Digital</p>
        </div>
    </a>
</div>
";

            Dictionary<string, int> MonthStringToInt = new()
            {
                { "january", 1 },
                { "february", 2 },
                { "march", 3 },
                { "april", 4 },
                { "may", 5 },
                { "june", 6 },
                { "july", 7 },
                { "august", 8 },
                { "september", 9 },
                { "october", 10 },
                { "november", 11 },
                { "december", 12 }
            };

            Dictionary<DateTime, Schema> schemasWithDate = new();
            foreach (string directory in Directory.EnumerateDirectories(PathTextBox.Text))
            {
                Schema schema = new();
                if (schema.TryLoad(Path.Combine(directory, Config.kSchemaFileName)))
                {
                    DateTime date = new(
                        Convert.ToInt32(schema.TokenValues[Schema.Tokens.Year]),
                        MonthStringToInt[schema.TokenValues[Schema.Tokens.Month].ToLower()],
                        1);

                    while (schemasWithDate.ContainsKey(date))
                    {
                        date = date.AddDays(1);
                    }
                    schemasWithDate.Add(date, schema);
                }
            }

            List<KeyValuePair<DateTime, Schema>> schemasOrderedByDate = schemasWithDate.OrderByDescending(x => x.Key).ToList();
            List<string> generatedPageEntries = new();
            foreach (KeyValuePair<DateTime, Schema> entry in schemasOrderedByDate)
            {
                Schema schema = entry.Value;

                int thumbnailWidth = 0;
                int thumbnailHeight = 0;
                if (schema.Thumbnail != string.Empty)
                {
                    string thumbnailFilename = schema.Thumbnail;
                    foreach (string imageFile in Directory.EnumerateFiles(PathTextBox.Text, string.Format("*.{0}", Path.GetExtension(thumbnailFilename)), SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(imageFile) == thumbnailFilename)
                        {
                            var imageMetadata = JpegParser.GetMetadata(imageFile);
                            thumbnailWidth = imageMetadata.Width;
                            thumbnailHeight = imageMetadata.Height;
                            break;
                        }
                    }
                }
                else
                {
                    // No thumbnail, skip generating this page
                    continue;
                }

                // We have to modify the PAGE_URL to have the site URL included (as we do when generating HTML in template code
                Dictionary<Schema.Tokens, string> modifiedSchemaTokens = new(schema.TokenValues);
                modifiedSchemaTokens[Schema.Tokens.PageUrl] = string.Format("{0}/{1}/", _site.Url, modifiedSchemaTokens[Schema.Tokens.PageUrl]);

                string generatedHtmlSnippet = string.Empty;
                foreach (string line in kHtmlTemplate.Split(Environment.NewLine))
                {
                    string processedLine = line;

                    // We still need to manually find and replace the image and width height tags
                    if (line.Contains(Config.kTemplateImageWidthToken) || line.Contains(Config.kTemplateImageHeightToken))
                    {
                        processedLine = processedLine.Replace(Config.kTemplateImageWidthToken, thumbnailWidth.ToString());
                        processedLine = processedLine.Replace(Config.kTemplateImageHeightToken, thumbnailHeight.ToString());
                    }

                    // Process each line and replace them with values from the schema
                    foreach (KeyValuePair<string, Schema.Tokens> tokenEntry in Schema.TokenTable)
                    {
                        if (schema.TokenValues.ContainsKey(tokenEntry.Value) && !Schema.OptionalTokens.Contains(tokenEntry.Value))
                        {
                            processedLine = processedLine.Replace(tokenEntry.Key, schema.TokenValues[tokenEntry.Value]);
                        }
                    }
                    generatedHtmlSnippet += processedLine + Environment.NewLine;
                }
                generatedPageEntries.Add(generatedHtmlSnippet);
            }


            List<string> GenerateHeadersIntoColumns(bool singleColumn)
            {
                List<string> columns = new() { string.Empty, string.Empty };
                for (int index = 0; index < generatedPageEntries.Count; index++)
                {
                    int columnIndex = singleColumn ? 0 : index % 2;
                    columns[columnIndex] += generatedPageEntries[index];
                }
                return columns;
            }

            GeneratedPageHeadersForm generatedTextForm = new GeneratedPageHeadersForm();

            List<string> splitGeneratedHeaders = GenerateHeadersIntoColumns(false);
            if (splitGeneratedHeaders.Count == 2)
            {
                generatedTextForm.SetLeftText(splitGeneratedHeaders[0]);
                generatedTextForm.SetRightText(splitGeneratedHeaders[1]);
            }

            generatedTextForm.SetSingleColumn(GenerateHeadersIntoColumns(true)[0]);
            generatedTextForm.StartPosition = FormStartPosition.CenterParent;
            generatedTextForm.Show(this);
        }

        #region Form Functionality

        private void ShowNewPageDialog()
        {
            PageCreateForm pageCreateDialog = new(_rootPath, _lastCreatedPageName);
            pageCreateDialog.StartPosition = FormStartPosition.CenterParent;
            //pageCreateDialog.MdiParent = this;
            //pageCreateDialog.Location = new Point(Location.X + (Size.Width - (pageCreateDialog.Width/2)), Location.Y - (Size.Height - (pageCreateDialog.Height/2)));
            pageCreateDialog.ShowDialog(Owner);

            if (pageCreateDialog.CreateButtonPressed)
            {
                _lastCreatedPageName = pageCreateDialog.CurrentPageName;

                string newDirectory = Path.Combine(_rootPath, pageCreateDialog.CurrentPageName);
                try
                {
                    Directory.CreateDirectory(newDirectory);
                }
                catch
                {
                    MessageBox.Show(
                        $"Could not create new page {_lastCreatedPageName}",
                        "Carpenter",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                    return;
                }

                if (pageCreateDialog.OpenInDesigner)
                {
                    RunPageDesigner(newDirectory);
                }

                RefreshPageList();
            }
        }

        private void EnablePageEntryButtons(bool shouldEnable)
        {
            foreach (Control control in TableLayoutPanel.Controls)
            {
                if (control is PageEntryControl entry)
                {
                    entry.EnableButtons(shouldEnable);
                }
            }
        }

        private void SetupFileSystemWatcher(string path)
        {
            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = path;
            _fileSystemWatcher.Filter = "*.*";
            _fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            _fileSystemWatcher.EnableRaisingEvents = true;
            _fileSystemWatcher.Created += FileSystemWatcher_Modification;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Modification;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Modification;
            //_fileSystemWatcher.IncludeSubdirectories = true;
        }

        private void RefreshPageList()
        {
            // Create entries from the directory and order them
            List<PageEntryControl> pageEntries = new List<PageEntryControl>();
            foreach (string directory in Directory.GetDirectories(_rootPath))
            {
                bool schemaPresent = File.Exists(Path.Combine(_rootPath, directory, "SCHEMA"));
                pageEntries.Add(new PageEntryControl(directory, schemaPresent ? PageEntryControl.ButtonTypes.Edit : PageEntryControl.ButtonTypes.Create, _template, _site));
            }
            pageEntries.Sort((x, y) => x.GetDirectoryName().CompareTo(y.GetDirectoryName()));

            // Add to form
            TableLayoutPanel.Controls.Clear();
            TableLayoutPanel.Controls.AddRange(pageEntries.ToArray());
        }

        #endregion

        #region Form callbacks

        private void MainForm_Load(object sender, EventArgs e)
        {
            // Try and load the last entered path
            string previouslyLoadedPath = Settings.Default.LastLoadedRootPath;
            if (string.IsNullOrEmpty(previouslyLoadedPath) == false)
            {
                PathTextBox.Text = previouslyLoadedPath;
                TryLoadDirectory(previouslyLoadedPath);
            }
        }

        private void PathTextBox_TextChanged(object sender, EventArgs e)
        {
            TryLoadDirectory(PathTextBox.Text);
        }

        private void NewFolderButton_Click(object sender, EventArgs e)
        {
            ShowNewPageDialog();
        }

        private void GenerateAllButton_Click(object sender, EventArgs e)
        {
            GenerateAllPagesInBackgroundWorker();
        }

        private void FileSystemWatcher_Modification(object sender, FileSystemEventArgs e)
        {
            BeginInvoke(new Action(() => RefreshPageList()));
        }

        #endregion

        #region Generate Site background worker

        private void GenerateSiteBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            Stopwatch progressStopwatch = Stopwatch.StartNew();
            GenerateSiteBackgroundWorker.ReportProgress(0, "Generating pages...");

            SiteGenerationState state = new();
            _site.GenerateAllPagesInSite((bool success, string directoryPath, int directoriesProcessed, int directoryCount) =>
            {
                if (success)
                {
                    state.SuccessfulPaths.Add(directoryPath);
                }
                else
                {
                    state.FailedPaths.Add(directoryPath);
                }
                state.Message = $"Generated {directoriesProcessed}/{directoryCount}";

                try
                {
                    GenerateSiteBackgroundWorker.ReportProgress((100 * directoriesProcessed) / directoryCount, state);
                }
                catch (InvalidOperationException) { /** Ey don't worry about it */ }
            });

            progressStopwatch.Stop();
            GenerateSiteBackgroundWorker.ReportProgress(100, $"Site generated in {progressStopwatch.ElapsedMilliseconds}ms");
        }

        private void BackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ToolStripProgressBar.Value = e.ProgressPercentage;
            if (e.UserState is string message)
            {
                StateToolStripStatusLabel.Text = message;
            }
            if (e.UserState is SiteGenerationState progressState)
            {
                foreach (Control control in TableLayoutPanel.Controls)
                {
                    if (control is PageEntryControl entry)
                    {
                        // Status has already been set, we don't need to reset it
                        if (entry.GetBuildState() != PageEntryControl.BuildState.Pending)
                        {
                            continue;
                        }

                        if (progressState.SuccessfulPaths.Contains(entry.GetDirectoryPath()))
                        {
                            entry.SetBuildStatus(PageEntryControl.BuildState.Success);
                        }

                        if (progressState.FailedPaths.Contains(entry.GetDirectoryPath()))
                        {
                            entry.SetBuildStatus(PageEntryControl.BuildState.Failure);
                        }
                    }
                }

                if (!string.IsNullOrEmpty(progressState.Message))
                {
                    StateToolStripStatusLabel.Text = progressState.Message;
                }
            }
        }

        private void BackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ToolStripProgressBar.Value = 100;
            EnablePageEntryButtons(true);
        }

        #endregion

        #region Menustrip callbacks

        private void removeUnusedImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveUnusedImages();
        }

        private void generateHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // TODO: Merge these into one
            GeneratePageHeaders();
        }

        private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(PathTextBox.Text) == false)
            {
                return;
            }

            Process.Start("explorer.exe", PathTextBox.Text);
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void newPageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowNewPageDialog();
        }

        private void GenerateHeadersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // TODO: Merge these into one
            GeneratePageHeaders();

        }

        private void siteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateAllPagesInBackgroundWorker();
        }

        #endregion

        private void ResetUIForGenerateOrPublish()
        {
            foreach (Control control in TableLayoutPanel.Controls)
            {
                if (control is PageEntryControl entry)
                {
                    entry.SetBuildStatus(PageEntryControl.BuildState.Pending);
                }
            }

            PublishButton.Enabled = false;
            GenerateAllButton.Enabled = false;
            EnablePageEntryButtons(false);
        }

        private void PublishButton_Click(object sender, EventArgs e)
        {
            ResetUIForGenerateOrPublish();
            PublishSiteBackgroundWorker.RunWorkerAsync();
        }

        private void PublishSiteBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Validate
            {
                PublishSiteBackgroundWorker.ReportProgress(0, "Validating Schemas...");
                _site.ValidateAllSchemas(out List<(string path, SchemaValidator.ValidationResults results)> siteResults,
                    (bool success, string directoryPath, int directoriesProcessed, int directoryCount) =>
                    {
                        PublishSiteBackgroundWorker.ReportProgress((100 * directoriesProcessed) / directoryCount, $"Validated {directoriesProcessed}/{directoryCount}");
                    });
                string errorMessage = string.Empty;
                foreach ((string path, SchemaValidator.ValidationResults results) schemaResult in siteResults)
                {
                    if (schemaResult.results.FailedTests.Count > 0)
                    {
                        errorMessage += $"{Environment.NewLine}- {Path.GetFileName(schemaResult.path)}";
                        schemaResult.results.FailedTests.ForEach(test => errorMessage += $"{Environment.NewLine} -> ({test.Importance}) {test.Name}");
                    }
                }

                if (!string.IsNullOrEmpty(errorMessage))
                {
                    using (ValidationFailedForm form = new(errorMessage))
                    {
                        form.StartPosition = FormStartPosition.CenterParent;
                        if (form.ShowDialog() == DialogResult.No)
                        {
                            PublishSiteBackgroundWorker.ReportProgress(0, $"Cancelled");
                            return;
                        }
                    }
                }
            }

            // Generate
            {
                PublishSiteBackgroundWorker.ReportProgress(0, "Generating pages...");
                SiteGenerationState state = new();
                _site.GenerateAllPagesInSite((bool success, string directoryPath, int directoriesProcessed, int directoryCount) =>
                {
                    if (success)
                    {
                        state.SuccessfulPaths.Add(directoryPath);
                    }
                    else
                    {
                        state.FailedPaths.Add(directoryPath);
                    }
                    state.Message = $"Generated {directoriesProcessed}/{directoryCount}";

                    try
                    {
                        PublishSiteBackgroundWorker.ReportProgress((100 * directoriesProcessed) / directoryCount, state);
                    }
                    catch (InvalidOperationException) { /** Ey don't worry about it */ }
                });
            }

            // Cleanup
            {
                int unusedImageCount = _site.GetAllUnusedImages().Count;
                if (unusedImageCount > 0)
                {
                    if (MessageBox.Show($"{unusedImageCount} unused images found, would you like to remove them?", "Cleanup", MessageBoxButtons.YesNo) == DialogResult.Yes)
                    {
                        _site.RemoveAllUnusedImages();
                    }
                }
            }
        }

        private void ValidateButton_Click(object sender, EventArgs e)
        {
            if (!_site.IsLoaded())
            {
                return;
            }
            
            _site.ValidateAllSchemas(out List<(string path, SchemaValidator.ValidationResults results)> siteResults,
                (bool success, string directoryPath, int directoriesProcessed, int directoryCount) =>
                {
                    ToolStripProgressBar.Value = (100 * directoriesProcessed) / directoryCount;
                });
            string errorMessage = string.Empty;
            foreach ((string path, SchemaValidator.ValidationResults results) schemaResult in siteResults)
            {
                if (schemaResult.results.FailedTests.Count > 0)
                {
                    errorMessage += $"{Environment.NewLine}- {Path.GetFileName(schemaResult.path)}";
                    schemaResult.results.FailedTests.ForEach(test => errorMessage += $"{Environment.NewLine} -> ({test.Importance}) {test.Name}");
                }
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                using (ValidationFailedForm form = new(errorMessage))
                {
                    if (form.ShowDialog() == DialogResult.No)
                    {
                        return;
                    }
                }
            }
        }
    }
}
