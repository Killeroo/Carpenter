using System;
using System.Collections.Generic;
using System.ComponentModel;
using Carpenter;
using PageDesigner.Controls;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using System.Diagnostics;

namespace PageDesigner.Forms
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

        private FileSystemWatcher _fileSystemWatcher = null;
        private Template _template = new Template();
        private string _rootPath = string.Empty;

        public MainForm()
        {
            InitializeComponent();
        }

        private void SetupFileSystemWatcher(string path)
        {
            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = path;
            _fileSystemWatcher.Filter = "*.*";
            _fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName;
            _fileSystemWatcher.EnableRaisingEvents = true;
            _fileSystemWatcher.Created += FileSystemWatcher_Created;
            _fileSystemWatcher.Renamed += _fileSystemWatcher_Renamed;
            _fileSystemWatcher.Deleted += _fileSystemWatcher_Deleted;
            //_fileSystemWatcher.IncludeSubdirectories = true;
        }

        // TODO: Dry (call common method)
        private void _fileSystemWatcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //throw new NotImplementedException();
            BeginInvoke(new Action(() =>
            {
                //FileAttributes attributes = File.GetAttributes(e.FullPath);
                //if (attributes.HasFlag(FileAttributes.Directory))
                //{
                    RefreshDirectoryList();
                //}

            }));
        }

        
        private void _fileSystemWatcher_Renamed(object sender, RenamedEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                //FileAttributes attributes = File.GetAttributes(e.FullPath);
                //if (attributes.HasFlag(FileAttributes.Directory))
                //{
                    RefreshDirectoryList();
                //}

            }));
        }


        private void FileSystemWatcher_Created(object sender, FileSystemEventArgs e)
        {
            BeginInvoke(new Action(() =>
            {
                //FileAttributes attributes = File.GetAttributes(e.FullPath);
                //if (attributes.HasFlag(FileAttributes.Directory))
                //{
                    //TryLoadDirectory(e.FullPath);
                    RefreshDirectoryList();
                //}
            }));
            //throw new NotImplementedException();
        }

        private bool TryLoadDirectory(string path)
        {
            // Sanity check path
            if (Directory.Exists(path) == false)
            {
                return false;
            }

            _rootPath = path;

            // Find template at root
            string templatePath = Path.Combine(_rootPath, kTemplateFilename);
            if (File.Exists(templatePath))
            {
                _template.Load(templatePath);
            }

            RefreshDirectoryList();

            // Save path to settings
            Properties.Settings.Default.LastLoadedRootPath = path;
            Properties.Settings.Default.Save();

            // Change path for system watcher to monitor
            if (_fileSystemWatcher == null)
            {
                SetupFileSystemWatcher(path);
            }
            else
            {
                _fileSystemWatcher.Path = path;
            }

            return true;
        }

        private void RefreshDirectoryList()
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
            string previouslyLoadedPath = Properties.Settings.Default.LastLoadedRootPath;
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
            // TODO: Put into method
            FolderBrowser.InitialDirectory = _rootPath;
            FolderBrowser.ShowNewFolderButton = true;
            FolderBrowser.Description = "Create new folder";
            FolderBrowser.UseDescriptionForTitle = true;

            if (FolderBrowser.ShowDialog() == DialogResult.OK)
            {
                RefreshDirectoryList();
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

        private void GenerateWebpages()
        {

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
    }
}
