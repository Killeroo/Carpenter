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
using System.Runtime.InteropServices;

using JpegMetadataExtractor;

using Carpenter;
using Carpenter.SiteViewer;
using Carpenter.SiteViewer.Forms;

using SiteViewer;
using SiteViewer.Controls;

using PageDesigner.Forms;

using static System.Windows.Forms.AxHost;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;


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
        private Site _site = new();
        private string _rootPath = string.Empty;
        private string _lastCreatedPageName = string.Empty;

        public SiteViewerForm()
        {
            InitializeComponent();
            FileTreeView.LabelEdit = true;
            FileTreeView.NodeMouseClick += (sender, args) => FileTreeView.SelectedNode = args.Node;
            EnableDoubleBuffering(FileTreeView);
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

            // We have loaded everything we need! Render the pages in the root dir
            RefreshPageList();
            PopulateTreeView();

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
            Cursor currentCursor = Cursor.Current;
            Cursor = Cursors.WaitCursor;
#if DEBUG
            PageDesignerForm form = new(path, _site.GetRootDir());
            Cursor = currentCursor;
            form.ShowDialog();
#else
            ProcessStartInfo startInfo = new(Path.Combine(Environment.CurrentDirectory, kPageDesignerAppName));
            startInfo.Arguments = $"\"{path}\" \"{_site.GetRootDir()}\"";
            Process.Start(startInfo);
            
            Cursor = currentCursor;
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

        private void PopulateTreeView()
        {
            // Clear the tree
            FileTreeView.Nodes.Clear();

            // If entered directory doesnt exist, dont bother rendering tree
            if (!Directory.Exists(PathTextBox.Text))
                return;

            void AddDirectoryToTreeView(TreeNodeCollection CurrentNodes, string currentDirectory)
            {
                // Get folders and files
                string[] dirs = Directory.GetDirectories(currentDirectory);
                string[] files = Directory.GetFiles(currentDirectory);

                foreach (string dir in dirs)
                {
                    DirectoryInfo dirInfo = new DirectoryInfo(dir);
                    TreeNode node = new TreeNode(dirInfo.Name, 0, 1);

                    try
                    {
                        node.Tag = dir;  //keep the directory's full path in the tag for use later

                        if (dirInfo.GetFiles().Count() > 0 || dirInfo.GetDirectories().Count() > 0)
                        {
                            AddDirectoryToTreeView(node.Nodes, dir);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        //if an unauthorized access exception occured display a locked folder
                        node.ImageIndex = 12;
                        node.SelectedImageIndex = 12;
                    }
                    finally
                    {
                        CurrentNodes.Add(node);
                    }
                }

                foreach (string file in files)
                {
                    // Creat new node with file name
                    TreeNode node = new TreeNode(Path.GetFileName(file), 0, 1);

                    // Display file image on node
                    node.ImageIndex = 13;
                    node.SelectedImageIndex = 13;
                    node.Tag = file;

                    if (file.Contains("SCHEMA") || file.Contains(".html") || file.Contains(".jpg"))
                    {
                        // Add to node
                        CurrentNodes.Add(node);
                    }
                }
            }

            AddDirectoryToTreeView(FileTreeView.Nodes, PathTextBox.Text);
        }

        #region Form Functionality

        private void ShowNewPageDialog(string path)
        {
            PageCreateForm pageCreateDialog = new(path, _lastCreatedPageName);
            pageCreateDialog.StartPosition = FormStartPosition.CenterParent;
            //pageCreateDialog.MdiParent = this;
            //pageCreateDialog.Location = new Point(Location.X + (Size.Width - (pageCreateDialog.Width/2)), Location.Y - (Size.Height - (pageCreateDialog.Height/2)));
            pageCreateDialog.ShowDialog(Owner);

            if (pageCreateDialog.CreateButtonPressed)
            {
                _lastCreatedPageName = pageCreateDialog.CurrentPageName;

                string newDirectory = Path.Combine(path, pageCreateDialog.CurrentPageName);
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
            //foreach (Control control in TableLayoutPanel.Controls)
            //{
            //    if (control is PageEntryControl entry)
            //    {
            //        entry.EnableButtons(shouldEnable);
            //    }
            //}
        }

        private void SetupFileSystemWatcher(string path)
        {
            _fileSystemWatcher = new FileSystemWatcher();
            _fileSystemWatcher.Path = path;
            _fileSystemWatcher.Filter = "SCHEMA";
            _fileSystemWatcher.NotifyFilter = NotifyFilters.DirectoryName | NotifyFilters.FileName | NotifyFilters.LastWrite;
            _fileSystemWatcher.EnableRaisingEvents = true;
            _fileSystemWatcher.Created += FileSystemWatcher_Modification;
            _fileSystemWatcher.Renamed += FileSystemWatcher_Modification;
            _fileSystemWatcher.Deleted += FileSystemWatcher_Modification;
            _fileSystemWatcher.IncludeSubdirectories = true;
        }

        private void RefreshPageList()
        {
            // Create entries from the directory and order them
            List<PageEntryControl> pageEntries = new List<PageEntryControl>();
            foreach (string directory in Directory.GetDirectories(_rootPath))
            {
                bool schemaPresent = File.Exists(Path.Combine(_rootPath, directory, "SCHEMA"));
                pageEntries.Add(new PageEntryControl(directory, schemaPresent ? PageEntryControl.ButtonTypes.Edit : PageEntryControl.ButtonTypes.Create, _site));
            }
            pageEntries.Sort((x, y) => x.GetDirectoryName().CompareTo(y.GetDirectoryName()));

            // Add to form
            //TableLayoutPanel.Controls.Clear();
            //TableLayoutPanel.Controls.AddRange(pageEntries.ToArray());
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
            _site.GenerateAllSchemas((bool success, string directoryPath, int directoriesProcessed, int directoryCount) =>
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
                //foreach (Control control in TableLayoutPanel.Controls)
                //{
                //    if (control is PageEntryControl entry)
                //    {
                //        // Status has already been set, we don't need to reset it
                //        if (entry.GetBuildState() != PageEntryControl.BuildState.Pending)
                //        {
                //            continue;
                //        }

                //        if (progressState.SuccessfulPaths.Contains(entry.GetDirectoryPath()))
                //        {
                //            entry.SetBuildStatus(PageEntryControl.BuildState.Success);
                //        }

                //        if (progressState.FailedPaths.Contains(entry.GetDirectoryPath()))
                //        {
                //            entry.SetBuildStatus(PageEntryControl.BuildState.Failure);
                //        }
                //    }
                //}

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

        private void siteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GenerateAllPagesInBackgroundWorker();
        }

        #endregion

        private void ResetUIForGenerateOrPublish()
        {
            //foreach (Control control in TableLayoutPanel.Controls)
            //{
            //    if (control is PageEntryControl entry)
            //    {
            //        entry.SetBuildStatus(PageEntryControl.BuildState.Pending);
            //    }
            //}

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

            // Generate pages
            {
                PublishSiteBackgroundWorker.ReportProgress(0, "Generating pages...");
                SiteGenerationState state = new();
                _site.GenerateAllSchemas((bool success, string directoryPath, int directoriesProcessed, int directoryCount) =>
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

            // Generate indexes
            {

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

        private void FileTreeView_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (Path.GetFileName(e.Node.Tag as string) == "SCHEMA")
            {
                RunPageDesigner(Path.GetDirectoryName(e.Node.Tag as string));
            }
            else
            {
                // Open it with default app
                Process.Start(new ProcessStartInfo(e.Node.Tag as string)
                {
                    UseShellExecute = true
                });
            }
        }

        private void FileTreeView_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (Path.GetFileName(e.Node.Tag as string) == "SCHEMA")
                {
                    ContextMenuStrip contextMenu = new();
                    contextMenu.Items.Add("Edit", null, (sender, args) => RunPageDesigner(Path.GetDirectoryName(e.Node.Tag as string)));
                    contextMenu.Items.Add("Open in Notepad", null, (sender, args) => Process.Start("notepad.exe", e.Node.Tag as string));
                    contextMenu.Items.Add(new ToolStripSeparator());
                    contextMenu.Items.Add("Build", null, (sender, args) => HtmlGenerator.BuildHtmlForSchema(new Schema(e.Node.Tag as string), _site));
                    contextMenu.Items.Add("Build Preview", null, (sender, args) => HtmlGenerator.BuildHtmlForSchema(new Schema(e.Node.Tag as string), _site, false));
                    contextMenu.Show(this, e.Location, ToolStripDropDownDirection.Right);
                }
                else if (Directory.Exists(e.Node.Tag as string))
                {
                    ContextMenuStrip contextMenu = new();
                    contextMenu.Items.Add("New Page...", null, (sender, args) => ShowNewPageDialog(Path.GetDirectoryName(e.Node.Tag as string)));
                    contextMenu.Items.Add("Rename", null, (sender, args) => e.Node.BeginEdit());
                    // Show in explorer
                    contextMenu.Show(this, e.Location, ToolStripDropDownDirection.Right);
                }
            }
        }

        /// <summary>
        /// Source: https://stackoverflow.com/a/73824330
        /// </summary>
        /// <param name="control"></param>
        public static void EnableDoubleBuffering(Control control)
        {
            SendMessage(control.Handle, TVM_SETEXTENDEDSTYLE, (IntPtr)TVS_EX_DOUBLEBUFFER, (IntPtr)TVS_EX_DOUBLEBUFFER);
        }

        private const int TVM_SETEXTENDEDSTYLE = 0x1100 + 44;
        private const int TVM_GETEXTENDEDSTYLE = 0x1100 + 45;
        private const int TVS_EX_DOUBLEBUFFER = 0x0004;

        [DllImport("user32.dll")]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wp, IntPtr lp);

        private void FileTreeView_AfterLabelEdit(object sender, NodeLabelEditEventArgs args)
        {
            string oldPath = args.Node.Tag as string;
            string newPath = Path.Combine(Path.GetDirectoryName(oldPath), args.Label);
            try
            {
                Directory.Move(args.Node.Tag as string, newPath);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Could not rename directory: {e.GetType()}:{e.Message}.",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                args.CancelEdit = true;
                args.Node.Text = Path.GetFileName(args.Node.Tag as string);
                return;
            }
            
            void UpdateNodeTagString(TreeNode CurrentNode, string old, string @new)
            {
                CurrentNode.Tag = (CurrentNode.Tag as string).Replace(oldPath, newPath);
                foreach (TreeNode Node in CurrentNode.Nodes)
                {
                    UpdateNodeTagString(Node, old, @new);
                }
            }
            
            UpdateNodeTagString(args.Node, oldPath, newPath);
            args.Node.Text = args.Label;
        }

        private void FileTreeView_BeforeLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            // Don't allow editing if it isn't a directory
            if (!Directory.Exists(e.Node.Tag as string))
            {
                e.CancelEdit = true;
            }
        }

        private void FileTreeView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F2 || e.KeyCode == Keys.Enter)
            {
                if (Directory.Exists(FileTreeView.SelectedNode.Tag as string))
                {
                    FileTreeView.SelectedNode.BeginEdit();
                }
            }
        }
    }
}
