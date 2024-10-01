﻿using System;
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

using Carpenter;

using SiteViewer;
using SiteViewer.Controls;
using PageDesigner.Forms;
using JpegMetadataExtractor;

namespace SiteViewer.Forms
{
    public partial class MainForm : Form
    {
        private class WebpageGenerationState
        {
            public List<string> ProcessedPaths = new();
            public List<string> FailedPaths = new();
            public List<string> SuccessfulPaths = new();

            public string ProgressMessage = string.Empty;
        }

        private const string kTemplateFilename = "template.html";
        private const string kPageDesignerAppName = "PageDesigner.exe";

        private FileSystemWatcher _fileSystemWatcher = null;
        private Template _template = new Template();
        private string _rootPath = string.Empty;
        private string _templatePath = string.Empty;
        private string _lastCreatedPageName = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        public void RunPageDesigner(string path)
        {
            if (_template == null)
            {
                Debug.Write("Could not open designer, template not loaded");
                return;
            }

#if false
            PageDesignerForm form = new(path, _template);
            form.ShowDialog();
#else
            ProcessStartInfo startInfo = new(kPageDesignerAppName);
            startInfo.Arguments = $"\"{path}\" \"{_template.FilePath}\"";
            Process.Start(startInfo);
#endif
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

        private void FileSystemWatcher_Modification(object sender, FileSystemEventArgs e)
        {
            BeginInvoke(new Action(() => RefreshPageList()));
        }

        // Try and load pages and template file at a path, silently fail otherwise.
        // This is because this is called everytime new path text is added 
        private void TryLoadDirectory(string path)
        {
            // Sanity check path
            if (Directory.Exists(path) == false)
            {
                return;
            }

            _rootPath = path;

            // We only want to try and load the template if it actually exits
            _templatePath = Path.Combine(_rootPath, kTemplateFilename);
            if (File.Exists(_templatePath))
            {
                _template = new Template(_templatePath);
            }

            RefreshPageList();

            // Save path to settings
            Settings.Default.LastLoadedRootPath = path;
            Settings.Default.Save();

            // Change path for system watcher to monitor
            if (_fileSystemWatcher == null)
            {
                SetupFileSystemWatcher(path);
            }
            else
            {
                _fileSystemWatcher.Path = path;
            }
        }

        private void RefreshPageList()
        {
            // Create entries from the directory and order them
            List<PageEntry> pageEntries = new List<PageEntry>();
            foreach (string directory in Directory.GetDirectories(_rootPath))
            {
                bool schemaPresent = File.Exists(Path.Combine(_rootPath, directory, "SCHEMA"));
                pageEntries.Add(new PageEntry(directory, !schemaPresent, _template));
            }
            pageEntries.Sort((x, y) => x.GetDirectoryName().CompareTo(y.GetDirectoryName()));

            // Add to form
            TableLayoutPanel.Controls.Clear();
            TableLayoutPanel.Controls.AddRange(pageEntries.ToArray());
        }

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
            ShowNewFolderDialog();
        }

        private void ShowNewFolderDialog()
        {
            // TODO: Put into method
            //FolderBrowser.InitialDirectory = _rootPath;
            //FolderBrowser.ShowNewFolderButton = true;
            //FolderBrowser.Description = "Create new folder";
            //FolderBrowser.UseDescriptionForTitle = true;

            //if (FolderBrowser.ShowDialog() == DialogResult.OK)
            //{
            //    RefreshPageList();
            //}

            PageCreateDialog pageCreateDialog = new(_rootPath, _lastCreatedPageName);
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

        private void GenerateAllButton_Click(object sender, EventArgs e)
        {
            ResetStatusButtons();
            EnablePageEntryButtons(false);
            GenerateSiteBackgroundWorker.RunWorkerAsync();
        }

        private void GenerateSiteBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (_template == null)
            {
                return;
            }

            if (Directory.Exists(_rootPath) == false)
            {
                return;
            }

            WebpageGenerationState state = new();

            Stopwatch progressStopwatch = Stopwatch.StartNew();
            string[] localDirectories = Directory.GetDirectories(_rootPath);
            for (int i = 0; i < localDirectories.Length; i++)
            {
                string localSchemaPath = Path.Combine(_rootPath, Path.GetFileName(localDirectories[i]), "SCHEMA");
                if (File.Exists(localSchemaPath))
                {
                    GenerateSiteBackgroundWorker.ReportProgress((100 * i) / localDirectories.Length, $"Generating '{Path.GetFileName(localDirectories[i])}'...");

                    string currentDirectoryPath = Path.GetDirectoryName(localSchemaPath);
                    using Schema localSchema = new(localSchemaPath);
                    if (_template.Generate(localSchema, currentDirectoryPath))
                    {
                        state.SuccessfulPaths.Add(currentDirectoryPath);
                    }
                    else
                    {
                        state.FailedPaths.Add(currentDirectoryPath);
                    }
                    state.ProcessedPaths.Add(currentDirectoryPath);

                    GenerateSiteBackgroundWorker.ReportProgress((100 * i) / localDirectories.Length, state);
                }
            }
            progressStopwatch.Stop();

            GenerateSiteBackgroundWorker.ReportProgress(100, $"Site generated in {progressStopwatch.ElapsedMilliseconds}ms");
        }

        private void GenerateSiteBackgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            ToolStripProgressBar.Value = e.ProgressPercentage;
            if (e.UserState is string message)
            {
                StateToolStripStatusLabel.Text = message;
            }
            if (e.UserState is WebpageGenerationState progressState)
            {
                foreach (Control control in TableLayoutPanel.Controls)
                {
                    if (control is PageEntry entry)
                    {
                        // Status has already been set, we don't need to reset it
                        if (entry.GetStatus() != PageEntry.Status.PENDING)
                        {
                            continue;
                        }

                        if (progressState.SuccessfulPaths.Contains(entry.GetDirectoryPath()))
                        {
                            entry.SetStatus(PageEntry.Status.SUCCESS);
                        }

                        if (progressState.FailedPaths.Contains(entry.GetDirectoryPath()))
                        {
                            entry.SetStatus(PageEntry.Status.FAILURE);
                        }
                    }
                }
            }

        }

        private void GenerateSiteBackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            ToolStripProgressBar.Value = 100;
            EnablePageEntryButtons(true);
        }

        private void ResetStatusButtons()
        {
            foreach (Control control in TableLayoutPanel.Controls)
            {
                if (control is PageEntry entry)
                {
                    entry.SetStatus(PageEntry.Status.PENDING);
                }
            }
        }

        private void EnablePageEntryButtons(bool shouldEnable)
        {
            foreach (Control control in TableLayoutPanel.Controls)
            {
                if (control is PageEntry entry)
                {
                    entry.EnableButtons(shouldEnable);
                }
            }
        }

        private void CleanButton_Click(object sender, EventArgs e)
        {

        }

        private void OrderButton_Click(object sender, EventArgs e)
        {

        }

        void RemoveUnusedImages()
        {
            string[] localDirectories = Directory.GetDirectories(_rootPath);
            int count = 0;
            for (int i = 0; i < localDirectories.Length; i++)
            {
                string localPath = Path.Combine(_rootPath, Path.GetFileName(localDirectories[i]));
                string localSchemaPath = Path.Combine(localPath, "SCHEMA");
                if (File.Exists(localSchemaPath))
                {
                    // try and load the schema in the directory
                    string currentDirectoryPath = Path.GetDirectoryName(localSchemaPath);
                    using Schema localSchema = new(localSchemaPath);

                    // Construct a list of all referenced images in the schema so we can
                    // work out what files aren't referenced
                    List<string> referencedImages = new List<string>();
                    foreach (ImageSection section in localSchema.ImageSections)
                    {
                        if (section is ColumnImageSection)
                        {
                            ColumnImageSection columnSection = section as ColumnImageSection;
                            foreach (StandaloneImageSection innerImage in columnSection.Sections)
                            {
                                referencedImages.Add(innerImage.PreviewImage);
                                referencedImages.Add(innerImage.DetailedImage);
                            }
                        }
                        else if (section is StandaloneImageSection)
                        {
                            StandaloneImageSection standaloneSection = section as StandaloneImageSection;
                            referencedImages.Add(standaloneSection.PreviewImage);
                            referencedImages.Add(standaloneSection.DetailedImage);
                        }
                    }

                    // Loop through and remove any files that aren't referenced in the schema 
                    foreach (string imagePath in Directory.GetFiles(localPath, "*.jpg"))
                    {
                        string imageName = Path.GetFileName(imagePath);
                        if (referencedImages.Contains(imageName) == false)
                        {
                            File.Delete(imagePath);
                            count++;
                        }
                    }
                }
            }

            StateToolStripStatusLabel.Text = count + " files removed";
        }

        void GeneratePageHeaders(bool singleColumn)
        {
            if (Directory.Exists(PathTextBox.Text) == false)
            {
                return;
            }

            // TODO: Should probably put this somewhere else
            const string kHtmlTemplate = @"
<div class=""container"">
    <a href=""%BASE_URL/%PAGE_URL/"">
        <img class=""preview_image"" src=""%BASE_URL/%PAGE_URL/%THUMBNAIL"" width=""%T_WIDTH"" height=""%T_HEIGHT"" style=""width:100%"">

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
                if (schema.Load(Path.Combine(directory, "SCHEMA")))
                {
                    DateTime date = new(
                        Convert.ToInt32(schema.TokenValues[Schema.Token.Year]),
                        MonthStringToInt[schema.TokenValues[Schema.Token.Month].ToLower()],
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

                string thumbnailName = string.Empty;
                int thumbnailWidth = 0;
                int thumbnailHeight = 0;
                if (schema.OptionValues.TryGetValue(Schema.Option.PageImage, out string pageImage)) 
                {
                    thumbnailName = pageImage;
                    foreach (string file in Directory.EnumerateFiles(PathTextBox.Text, "*.jpg", SearchOption.AllDirectories))
                    {
                        if (Path.GetFileName(file) == pageImage)
                        {
                            var imageMetadata = JpegParser.GetMetadata(file);
                            thumbnailWidth = imageMetadata.Width;
                            thumbnailHeight = imageMetadata.Height;
                            break;
                        }
                    }
                }

                string generatedHtmlSnippet = string.Empty;
                foreach (string line in kHtmlTemplate.Split(Environment.NewLine))
                {
                    string processedLine = line;

                    // HACK: Remove once we change thumbnail to be a token
                    if (string.IsNullOrEmpty(thumbnailName) == false)
                    {
                        processedLine = processedLine.Replace(@"%THUMBNAIL", thumbnailName);
                        processedLine = processedLine.Replace(@"%T_WIDTH", thumbnailWidth.ToString());
                        processedLine = processedLine.Replace(@"%T_HEIGHT", thumbnailHeight.ToString());
                    }

                    foreach (KeyValuePair<string, Schema.Token> tokenEntry in schema.TokenTable)
                    {
                        processedLine = processedLine.Replace(tokenEntry.Key, schema.TokenValues[tokenEntry.Value]);
                    }
                    generatedHtmlSnippet += processedLine + Environment.NewLine;
                }
                generatedPageEntries.Add(generatedHtmlSnippet);
            }

            List<string> columns = new() { string.Empty, string.Empty };
            for (int index = 0; index < generatedPageEntries.Count; index++)
            {
                int columnIndex = singleColumn ? 0 : index % 2;
                columns[columnIndex] += generatedPageEntries[index];
            }

            string finalGeneratedText = string.Empty;
            foreach (string column in columns)
            {
                finalGeneratedText += column;
                finalGeneratedText += "-----------------------" + Environment.NewLine;
            }

            GeneratedTextForm generatedTextForm = new GeneratedTextForm();
            generatedTextForm.SetText(finalGeneratedText);
            generatedTextForm.StartPosition = FormStartPosition.CenterParent;
            generatedTextForm.Show(this);
        }

        private void removeUnusedImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RemoveUnusedImages();
        }

        private void generateHeaders1ColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {

            // TODO: Merge these into one
            GeneratePageHeaders(true);
        }

        private void generateHeaders2ColumnsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GeneratePageHeaders(false);
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
            ShowNewFolderDialog();
        }
    }
}
