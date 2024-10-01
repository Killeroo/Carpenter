using Carpenter;
using CefSharp.DevTools.Network;
using JpegMetadataExtractor;
using PageDesigner.Controls;
using PageDesigner.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace PageDesigner.Forms
{
    public class ChangesStack<T> where T : class
    {
        private List<T> _changes;
        private int _current = 0;
        private int _head = 0;

        public ChangesStack()
        {
            _changes = new List<T>();
        }

        public void Commit(T change)
        {
            if (change == GetCurrentChange())
            {
                return;
            }

            if (_current != _head)
            {
                _changes.RemoveRange(_current, _head - _current);
            }

            _changes.Add(change);
            _head = _current = _changes.Count - 1;
        }

        public T GetCurrentChange()
        {
            if (_changes.Count == 0)
            {
                return null;
            }

            return _changes[_current];
        }

        public T Undo()
        {
            _current = Math.Max(--_current, 0);
            return _changes[_current];
        }

        public T Redo()
        {
            _current = Math.Min(++_current, _head);
            return _changes[_current];
        }

        public void Reset()
        {
            _changes.Clear();
            _current = _head = 0;
        }
    }

    public partial class PageDesignerForm : Form
    {
        private const string kUnsavedTitleString = " *";
        private const string kImageFileFilter = "Image Files(*.JPG;*.JPEG)|*.JPG;*.JPEG|All files (*.*)|*.*";

        private string _workingPath;
        private string _schemaPath;
        private Schema _workingSchema;
        private Schema _savedSchema;
        private Template _template;

        private PreviewImageBox _selectedPreviewImageControl;
        private GridPictureBox _selectedGridImage;
        private PreviewImageBox _pageImagePreviewImageBox;
        private Dictionary<string, Image> _previewImages = new();
        private Queue<GridPictureBox> _pictureBoxBuffer = new();
        private ChangesStack<Schema> _schemaChanges = new();

        private LivePreviewForm _livePreviewForm = null;
        private bool _isLivePreviewFormActive = false;
        private string _pageImageName;

        // TODO: DRY
        public PageDesignerForm()
        {
            InitializeComponent();

            _workingPath = Environment.CurrentDirectory;

            // TODO: Move to designer
            LivePreviewGenerateTimer.Tick += LivePreviewGenerateTimer_Tick;
        }
        public PageDesignerForm(string path, Template template) : this()
        {
            if (Directory.Exists(path) == false)
            {
                MessageBox.Show(
                    $"Could not find path, please check and try again",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _template = template;
            _workingPath = path;
            _schemaPath = Path.Combine(_workingPath, "SCHEMA");
            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);
        }
        public PageDesignerForm(string path, string templatePath) : this()
        {
            if (Directory.Exists(path) == false || File.Exists(templatePath) == false)
            {
                MessageBox.Show(
                    $"Could not find directory path or page path, please check and try again",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                _template = new Template(templatePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not parse template ({ex.GetType()}). Check format and try again.",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _workingPath = path;
            _schemaPath = Path.Combine(_workingPath, "SCHEMA");
        }

        private void AddTextboxCallbacks()
        {
            //PreviewImageTextBox.TextChanged += FormTextBox_TextChanged;
            //DetailedImageTextBox.TextChanged += FormTextBox_TextChanged;
            PageUrlTextBox.TextChanged += FormTextBox_TextChanged;
            TitleTextBox.TextChanged += FormTextBox_TextChanged;
            BaseUrlTextBox.TextChanged += FormTextBox_TextChanged;
            LocationTextBox.TextChanged += FormTextBox_TextChanged;
            MonthTextBox.TextChanged += FormTextBox_TextChanged;
            YearTextBox.TextChanged += FormTextBox_TextChanged;
            AuthorTextBox.TextChanged += FormTextBox_TextChanged;
            CameraTextBox.TextChanged += FormTextBox_TextChanged;
        }

        private void RemoveTextboxCallbacks()
        {
            //PreviewImageTextBox.TextChanged -= FormTextBox_TextChanged;
            //DetailedImageTextBox.TextChanged -= FormTextBox_TextChanged;
            PageUrlTextBox.TextChanged -= FormTextBox_TextChanged;
            TitleTextBox.TextChanged -= FormTextBox_TextChanged;
            BaseUrlTextBox.TextChanged -= FormTextBox_TextChanged;
            LocationTextBox.TextChanged -= FormTextBox_TextChanged;
            MonthTextBox.TextChanged -= FormTextBox_TextChanged;
            YearTextBox.TextChanged -= FormTextBox_TextChanged;
            AuthorTextBox.TextChanged -= FormTextBox_TextChanged;
            CameraTextBox.TextChanged -= FormTextBox_TextChanged;
        }

        // TODO: Move to utils 
        private string GeneratePreviewWebpage()
        {
            if (_workingSchema == null || _template == null)
            {
                return string.Empty;
            }

            // TODO: Check if schema has changed before loading
            UpdateWorkingSchemaFromForm();

            // Generate preview page
            Stopwatch stopwatch = Stopwatch.StartNew();
            if (_template.Generate(_workingSchema, _workingPath, true))
            {
                string originalOutputFile = _workingSchema.OptionValues[Schema.Option.OutputFilename];
                string previewName = Path.GetFileNameWithoutExtension(originalOutputFile) + "_preview";
                string previewPath = Path.Combine(_workingPath, previewName + Path.GetExtension(originalOutputFile));

                stopwatch.Stop();
                statusToolStripStatusLabel.Text = string.Format("Generated Preview in {0}ms", stopwatch.ElapsedMilliseconds);

                return previewPath;
            }

            // TODO: Log error
            return string.Empty;
        }

        private void DisplayPreviewInNewProcess()
        {
            string previewPath = GeneratePreviewWebpage();

            // Open it with default app
            if (File.Exists(previewPath))
            {
                Process.Start(new ProcessStartInfo(previewPath)
                {
                    UseShellExecute = true
                });
            }
        }

        private void ResetForm()
        {
            RemoveTextboxCallbacks();
            _savedSchema = new Schema();
            _workingSchema = new Schema();
            _schemaChanges.Reset();

            BaseUrlTextBox.Text = Settings.Default.BaseUrlLastUsedValue;
            PageUrlTextBox.Text = Settings.Default.PageUrlLastUsedValue;
            TitleTextBox.Text = Settings.Default.TitleLastUsedValue;
            LocationTextBox.Text = Settings.Default.LocationLastUsedValue;
            MonthTextBox.Text = Settings.Default.MonthLastUsedValue;
            YearTextBox.Text = Settings.Default.YearLastUsedValue;
            AuthorTextBox.Text = Settings.Default.AuthorLastUsedValue;
            CameraTextBox.Text = Settings.Default.CameraLastUsedValue;

            GridFlowLayoutPanel.Controls.Clear();
            ImagePreviewFlowLayoutPanel.Controls.Clear();
            AddTextboxCallbacks();
        }

        private void CreateBlankForm()
        {
            ResetForm();
            LoadAvailableImagePreviews();
            SignalSchemaChange();
            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);

            if (ImagePreviewFlowLayoutPanel.Controls.Count == 0)
            {
                DialogResult result = MessageBox.Show("Seems you have loaded a blank page, would you like to import images to the page?", "Import images", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    ShowImportImageDialog();
                }
            }
        }

        private bool LoadSchemaFromFile(string path)
        {
            string schemaPath = Path.Combine(path, "SCHEMA");
            if (File.Exists(schemaPath) == false)
            {
                return false;
            }

            Schema schema = new Schema();
            if (schema.Load(schemaPath) == false)
            {
                // TODO: Raise an error or something
                return false;
            }

            // Clear any old data before continuing
            ResetForm();

            // We are passed the point where loading could fail, setup the page for displaying the schema
            _workingPath = path;
            _schemaPath = schemaPath;
            _savedSchema = schema;
            _workingSchema = new Schema(_savedSchema);

            if (_workingSchema.OptionValues.TryGetValue(Schema.Option.PageImage, out string pageImageName))
            {
                _pageImageName = pageImageName;
            }

            UpdateFormFromWorkingSchema();

            // Load available images in the directory
            LoadAvailableImagePreviews();

            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);

            return true;
        }

        private void SaveSchemaToFile()
        {
            if (_savedSchema != null && _workingSchema == _savedSchema)
            {
                return;
            }

            // Temp: Add _detailed prefix to all images in the grid
            //foreach (Control control in GridFlowLayoutPanel.Controls)
            //{
            //    if (control is GridPictureBox gridPictureBox)
            //    {
            //        gridPictureBox.DetailedImageName = gridPictureBox.PreviewImageName.Replace("_Preview", "_Detailed");
            //    }
            //}

            if (string.IsNullOrEmpty(_pageImageName) == false)
            {
                _workingSchema.OptionValues.AddOrUpdate(Schema.Option.PageImage, _pageImageName);
            }

            UpdateWorkingSchemaFromForm();
            

            //if (File.Exists(_schemaPath))
            //{
            //    if (ShowConfirmSaveDialog() == false)
            //    {
            //        return;
            //    }
            //}

            // TODO: Show an error when schema save fails
            if (_workingSchema.Save(Path.GetDirectoryName(_schemaPath)))
            {
                MessageBox.Show("Schema successfully saved.", "File saved");
            }

            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);

            // Save whatever values we entered for next time!
            Settings.Default.BaseUrlLastUsedValue = BaseUrlTextBox.Text;
            Settings.Default.PageUrlLastUsedValue = PageUrlTextBox.Text;
            Settings.Default.TitleLastUsedValue = TitleTextBox.Text;
            Settings.Default.LocationLastUsedValue = LocationTextBox.Text;
            Settings.Default.MonthLastUsedValue = MonthTextBox.Text;
            Settings.Default.YearLastUsedValue = YearTextBox.Text;
            Settings.Default.AuthorLastUsedValue = AuthorTextBox.Text;
            Settings.Default.CameraLastUsedValue = CameraTextBox.Text;
            Settings.Default.Save();
        }

        private void UpdateWorkingSchemaFromForm()
        {
            if (_workingSchema == null)
            {
                return;
            }

            // Update tokens values from page
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.BaseUrl, BaseUrlTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.PageUrl, PageUrlTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.Title, TitleTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.Location, LocationTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.Month, MonthTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.Year, YearTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.Author, AuthorTextBox.Text);
            _workingSchema.TokenValues.AddOrUpdate(Schema.Token.Camera, CameraTextBox.Text);

            // Copy or add class id and option base values
            if (_savedSchema.TokenValues != _workingSchema.TokenValues && _savedSchema.TokenValues.Count != 0)
            {
                // TODO: HACK: Copy values from original (WHICH WON'T ALWAYS EXIST)
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageColumn, _savedSchema.TokenValues[Schema.Token.ClassIdImageColumn]);
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageElement, _savedSchema.TokenValues[Schema.Token.ClassIdImageElement]);
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageGrid, _savedSchema.TokenValues[Schema.Token.ClassIdImageGrid]);
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageTitle, _savedSchema.TokenValues[Schema.Token.ClassIdImageTitle]);
            }
            else
            {
                // Create some default values
                // TODO: Have these set via a form on the main form or have them stored somewhere else
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageColumn, "photo_column");
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageElement, "photo_image");
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageGrid, "photo_grid");
                _workingSchema.TokenValues.AddOrUpdate(Schema.Token.ClassIdImageTitle, "photo_title");
            }

            if (_savedSchema.OptionValues != _workingSchema.OptionValues)
            {
                // TODO: Have these set via a form on the main form or have them stored somewhere else
                _workingSchema.OptionValues = _savedSchema.OptionValues;
            }
            else
            {
                _workingSchema.OptionValues.AddOrUpdate(Schema.Option.OutputFilename, "index.html");
            }

            // Parse grid layout
            List<ImageSection> sections = new();
            List<StandaloneImageSection>[] columnsBuffer = new List<StandaloneImageSection>[]
            {
                new List<StandaloneImageSection>(),
                new List<StandaloneImageSection>()
            };
            foreach (Control control in GridFlowLayoutPanel.Controls)
            {
                if (control is GridPictureBox gridPictureBox)
                {
                    // Create standalone image from GridPictureBox
                    StandaloneImageSection currentImageSection = new()
                    {
                        DetailedImage = gridPictureBox.DetailedImageName,
                        PreviewImage = gridPictureBox.PreviewImageName
                    };

                    if (gridPictureBox.IsStandaloneImage())
                    {
                        // Attempt to add contents of previous column buffer if we encounter a standalone image
                        TryAddColumnsBuffer(ref columnsBuffer, ref sections);

                        // Add it straight to sections
                        sections.Add(currentImageSection);
                    }
                    else
                    {
                        // Add to appropriate column
                        // NOTE: Because columns are displayed side by side we need to add images to columns tit for tat (aka one image per time to each column)

                        // Check size of column buffers to see where to add image to
                        int currentColumn = 1; // Add to second column by default
                        if ((columnsBuffer[0].Count == 0 && columnsBuffer[1].Count == 0)
                            || columnsBuffer[0].Count <= columnsBuffer[1].Count)
                        {
                            // Add to first column
                            currentColumn = 0;
                        }

                        // Add new image to column
                        columnsBuffer[currentColumn].Add(currentImageSection);
                    }
                }
            }
            TryAddColumnsBuffer(ref columnsBuffer, ref sections);

            _workingSchema.ImageSections = sections;
        }

        private void UpdateFormFromWorkingSchema()
        {
            PopulateTextboxesFromWorkingSchema();
            PopulateGridFromWorkingSchema();
        }

        private void PopulateGridFromWorkingSchema()
        {
            GridFlowLayoutPanel.Controls.Clear();
            foreach (ImageSection section in _workingSchema.ImageSections)
            {
                Type sectionType = section.GetType();
                if (sectionType == typeof(StandaloneImageSection))
                {
                    StandaloneImageSection? standaloneSection = section as StandaloneImageSection;
                    if (standaloneSection != null)
                    {
                        string fileName = standaloneSection.PreviewImage;

                        AddLocalImageToGridLayout(standaloneSection.PreviewImage, standaloneSection.DetailedImage, true);
                    }
                }
                else if (sectionType == typeof(ColumnImageSection))
                {
                    ColumnImageSection columnSection = (ColumnImageSection)section;
                    if (columnSection == null)
                    {
                        continue;
                    }

                    if (_pictureBoxBuffer.Count > 0)
                    {
                        foreach (StandaloneImageSection standaloneImage in columnSection.Sections)
                        {
                            GridPictureBox pictureBox = null;
                            if (_pictureBoxBuffer.Count > 0)
                            {
                                pictureBox = _pictureBoxBuffer.Dequeue();
                            }

                            AddLocalImageToGridLayout(standaloneImage.PreviewImage, standaloneImage.DetailedImage, false, pictureBox);
                        }

                        // Clear anything left in the buffer (this isn't great)
                        _pictureBoxBuffer.Clear();
                    }
                    else
                    {
                        foreach (StandaloneImageSection standaloneImage in columnSection.Sections)
                        {
                            if (standaloneImage != null)
                            {
                                AddLocalImageToGridLayout(standaloneImage.PreviewImage, standaloneImage.DetailedImage, false);

                                // Add the picturebox for the other column items
                                GridPictureBox pictureBox = new();
                                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                                GridFlowLayoutPanel.Controls.Add(pictureBox);
                                _pictureBoxBuffer.Enqueue(pictureBox);
                            }
                        }
                    }
                }
            }
        }

        private void PopulateTextboxesFromWorkingSchema()
        {
            // We don't want any callbacks triggering when we populate them
            // We only want them to trigger when the user edits them
            RemoveTextboxCallbacks();

            string GetTokenFromSchema(Schema.Token token, string defaultValue)
            {
                if (_workingSchema == null || _workingSchema.TokenValues.ContainsKey(token) == false)
                {
                    return defaultValue;
                }

                return _workingSchema.TokenValues[token];
            }
            BaseUrlTextBox.Text = GetTokenFromSchema(Schema.Token.BaseUrl, Settings.Default.BaseUrlLastUsedValue);
            PageUrlTextBox.Text = GetTokenFromSchema(Schema.Token.PageUrl, Settings.Default.PageUrlLastUsedValue);
            TitleTextBox.Text = GetTokenFromSchema(Schema.Token.Title, Settings.Default.TitleLastUsedValue);
            LocationTextBox.Text = GetTokenFromSchema(Schema.Token.Location, Settings.Default.LocationLastUsedValue);
            MonthTextBox.Text = GetTokenFromSchema(Schema.Token.Month, Settings.Default.MonthLastUsedValue);
            YearTextBox.Text = GetTokenFromSchema(Schema.Token.Year, Settings.Default.YearLastUsedValue);
            AuthorTextBox.Text = GetTokenFromSchema(Schema.Token.Author, Settings.Default.AuthorLastUsedValue);
            CameraTextBox.Text = GetTokenFromSchema(Schema.Token.Camera, Settings.Default.CameraLastUsedValue);

            AddTextboxCallbacks();
        }

        private void SignalSchemaChange()
        {
            if (_workingSchema == _schemaChanges.GetCurrentChange())
            {
                return;
            }

            UpdateWorkingSchemaFromForm();
            _schemaChanges.Commit(new Schema(_workingSchema));
            if (this.Text.Contains(kUnsavedTitleString) == false)
                this.Text += kUnsavedTitleString;

            if (_isLivePreviewFormActive)
            {
                // We don't want to constantly be generating previews, wait a bit before actually generating the preview
                // TODO: Make an instant preview and then wait for a bit before generating another
                if (LivePreviewGenerateTimer.Enabled == false)
                {
                    //if (_livePreviewForm == null)
                    //{
                    //    return;
                    //}
                    //_livePreviewForm.LoadUrl(GeneratePreviewWebpage());
                    LivePreviewGenerateTimer.Start();
                }
            }
        }

        private void LivePreviewGenerateTimer_Tick(object? sender, EventArgs e)
        {
            if (_livePreviewForm == null)
            {
                return;
            }

            _livePreviewForm.LoadUrl(GeneratePreviewWebpage());
            LivePreviewGenerateTimer.Stop();
        }

        private void UndoSchemaChanges()
        {
            Schema lastSchemaChange = new Schema(_schemaChanges.Undo());
            if (lastSchemaChange != _workingSchema)
            {
                _workingSchema = lastSchemaChange;

                // Only re-populate the grid if it has actually changed 
                // NOTE: Equality check isn't working
                //if (_workingSchema.ImageSections != lastSchemaChange.ImageSections)
                //{
                //    PopulateGridFromWorkingSchema();
                //}
                PopulateGridFromWorkingSchema();
                PopulateTextboxesFromWorkingSchema();


                // Remove unsaved character from title bar if we are at parity with the last saved schema
                if (_workingSchema == _savedSchema)
                {
                    this.Text = this.Text.Replace(kUnsavedTitleString, "");
                }
            }
        }

        private void RedoSchemaChanges()
        {
            Schema schemaChange = new Schema(_schemaChanges.Redo());
            if (schemaChange != _workingSchema)
            {
                _workingSchema = schemaChange;

                if (_workingSchema.ImageSections != schemaChange.ImageSections)
                {
                    PopulateGridFromWorkingSchema();
                }
                PopulateTextboxesFromWorkingSchema();


                // Remove unsaved character from title bar if we are at parity with the last saved schema
                if (_workingSchema == _savedSchema)
                {
                    this.Text = this.Text.Replace(kUnsavedTitleString, "");
                }
            }
        }

        private Image LoadImage(string imageName, bool standalone)
        {
            // Find the image locally and save a resized copy
            string localImagePath = Path.Combine(_workingPath, imageName);
            if (File.Exists(localImagePath) == false)
            {
                return null;
            }
            using (Image sourceImage = Image.FromFile(localImagePath)) // TODO: Cache this
            {
                // Calcuating the aspect ratio for images that have been resized is hard.
                // The ImageUtils.CalculateAspectRatio I used produces horrible results so we hardcode the aspect ratio for now.
                // TODO: use the actual aspect ratio of the image.
                AspectRatio ar = new AspectRatio(1, 1);// 3, 4);//ImageUtils.CalculateAspectRatio(sourceImage);

                // Find the width that we need to fit into
                int targetWidth = GridFlowLayoutPanel.Width - 10;
                if (standalone == false)
                {
                    targetWidth /= 2;
                }

                // There is a bug where, when we add an image to a picturebox from the buffer there is some deadspace
                // causing an overflow, meaning we have to reduce this width for now
                targetWidth -= 20;// 10; 
                int targetHeight = ar.CalculateHeight(targetWidth);

                return ImageUtils.ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);
            }
        }

        private void LoadAvailableImagePreviews()
        {
            if (string.IsNullOrEmpty(_workingPath) || Directory.Exists(_workingPath) == false)
            {
                return;
            }

            string[] imageFilesAtPath = Directory.GetFiles(_workingPath, "*.jpg");
            if (imageFilesAtPath.Length == 0)
            {
                return;
            }

            // Failed attempt to get thumbnails from the JpegParser
            // This works when you save the thumbnail to a file first... but as a MemoryStream it doesn't work

            //foreach (string imagePath in imageFilesAtPath)
            //{
            //    data.Add(JpegParser.GetRawMetadata(imagePath));

            //    using (Image previewImage = Image.FromStream(new MemoryStream(data[data.Count - 1].ThumbnailData)))
            //    {
            //        // TODO: Wish we didn't have to force aspectratio but CalculateAspectRatio is broken
            //        //AspectRatio ar = new AspectRatio(3, 4);//ImageUtils.CalculateAspectRatio(originalImage);

            //        //int desiredWidth = 120;
            //        //int desiredHeight = ar.CalculateHeight(desiredWidth);

            //        //// TODO: Use DrawImage to properly resize
            //        //Image previewImage2 = previewImage.GetThumbnailImage(desiredWidth, desiredHeight, () => false, IntPtr.Zero);

            //        var width = previewImage.Width;
            //        _previewImages.Add(Path.GetFileName(imagePath), previewImage);

            //        //_previewImages.Add(Path.GetFileName(imagePath), previewImage2);

            //        // Cache in temp directory
            //        // TODO: Fix with aspect ratio
            //        PreviewImageBox previewImageBox = new PreviewImageBox(
            //            Path.GetFileName(imagePath), previewImage);
            //        //FetchOrCatchPreviewImage(Path.GetFileName(filename), inputtedPath));  //

            //        //previewImageBox.MouseClick += ImagePreviewFlowLayoutPanel_MouseClick;
            //        previewImageBox.ControlClicked += ImagePreviewFlowLayoutPanel_Control_Clicked;
            //        previewImageBox.ControlDoubleClicked += ImagePreviewFlowLayoutPanel_Control_DoubleClicked;
            //        previewImageBox.AddContextItemClicked += ImagePreviewFlowLayoutPanel_Control_DoubleClicked;
            //        previewImageBox.InsertContextItemClicked += ImagePreviewFlowPanel_InsertContextItem_Clicked;
            //        previewImageBox.ReplaceContextItemClicked += ImagePreviewFlowPanel_ReplaceContextItem_Clicked;

            //        ImagePreviewFlowLayoutPanel.Controls.Add(previewImageBox);
            //    }


            foreach (string imagePath in imageFilesAtPath)
            {
                if (Path.GetFileName(imagePath).Contains("_Detailed"))
                {
                    // We don't want to load the bigger image files that won't be used directly on the webpage
                    continue;
                }

                using (Image originalImage = Image.FromFile(imagePath))
                {
                    // TODO: Wish we didn't have to force aspectratio but CalculateAspectRatio is broken
                    AspectRatio ar = new AspectRatio(1, 1);//ImageUtils.CalculateAspectRatio(originalImage);

                    int desiredWidth = 120;
                    int desiredHeight = ar.CalculateHeight(desiredWidth);

                    // TODO: Use DrawImage to properly resize
                    Image previewImage = originalImage.GetThumbnailImage(desiredWidth, desiredHeight, () => false, IntPtr.Zero);

                    _previewImages.Add(Path.GetFileName(imagePath), previewImage);

                    // Cache in temp directory
                    // TODO: Fix with aspect ratio
                    PreviewImageBox previewImageBox = new PreviewImageBox(
                        Path.GetFileName(imagePath), previewImage);

                    if (_pageImageName == Path.GetFileName(imagePath))
                    {
                        previewImageBox.SetPageImageChecked(true);
                    }
                    //FetchOrCatchPreviewImage(Path.GetFileName(filename), inputtedPath));  //

                    //previewImageBox.MouseClick += ImagePreviewFlowLayoutPanel_MouseClick;
                    previewImageBox.ControlClicked += ImagePreviewFlowLayoutPanel_Control_Clicked;
                    previewImageBox.ControlDoubleClicked += ImagePreviewFlowLayoutPanel_Control_DoubleClicked;
                    previewImageBox.AddContextItemClicked += ImagePreviewFlowLayoutPanel_Control_DoubleClicked;
                    previewImageBox.InsertContextItemClicked += ImagePreviewFlowPanel_InsertContextItem_Clicked;
                    previewImageBox.ReplaceContextItemClicked += ImagePreviewFlowPanel_ReplaceContextItem_Clicked;
                    previewImageBox.ImagePageContextItemClicked += ImagePreviewFlowPanel_ImagePageContextItem_Clicked;

                    ImagePreviewFlowLayoutPanel.Controls.Add(previewImageBox);
                }
            }
        }

        // TODO: DRY
        private GridPictureBox CreateGridPictureBox(string previewImageName, string detailedImageName, bool standaloneImage)
        {
            // Find the image locally and save a resized copy
            string localImagePath = Path.Combine(_workingPath, previewImageName);
            if (File.Exists(localImagePath) == false)
            {
                return null;
            }

            // Load original image, resize it to fit into grid
            // TODO: Work out size without loading the whole image into memory
            using (Image sourceImage = Image.FromFile(localImagePath)) // TODO: Cache this
            {
                // TODO: This is just awful, so we hardcode the aspect ratio
                AspectRatio ar = new AspectRatio(3, 4);//ImageUtils.CalculateAspectRatio(sourceImage);

                // Find the width that we need to fit into
                int targetWidth = GridFlowLayoutPanel.Width - 10;
                if (standaloneImage == false)
                {
                    targetWidth /= 2;
                }

                // There is a bug where, when we add an image to a picturebox from the buffer there is some deadspace
                // causing an overflow, meaning we have to reduce this width for now
                targetWidth -= 20;// 10; 
                int targetHeight = ar.CalculateHeight(targetWidth);

                //ImageMetadata metadata = JpegParser.GetMetadata(localImagePath);
                if (sourceImage.Height > sourceImage.Width)// || metadata.Orientation != OrientationType.Horizontal)
                {
                    targetHeight *= 2;
                }

                Image resizedImage = ImageUtils.ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);

                GridPictureBox gridPictureBox = new();

                // Setup image
                gridPictureBox.Image = resizedImage;
                gridPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                // Save properties for later
                gridPictureBox.DetailedImageName = detailedImageName;
                gridPictureBox.PreviewImageName = previewImageName;
                gridPictureBox.SetStandaloneImage(standaloneImage);

                // Add callbacks
                gridPictureBox.Click += GridPictureBox_Click;
                gridPictureBox.Paint += GridPictureBox_Paint;
                gridPictureBox.SwapMenuItemClicked += GridPictureBox_SwapMenuItemClicked;
                gridPictureBox.RemoveMenuItemClicked += GridPictureBox_RemoveMenuItemClicked;
                gridPictureBox.StandaloneMenuItemClicked += GridPictureBox_StandaloneMenuItemClicked;

                return gridPictureBox;
            }
        }

        // TODO: DRY
        private void UpdateGridPictureBox(GridPictureBox pictureBox, string newPreviewImageName, string newDetailedImageName, bool standalone)
        {

            // Find the image locally and save a resized copy
            string localImagePath = Path.Combine(_workingPath, newPreviewImageName);
            if (File.Exists(localImagePath) == false)
            {
                return;
            }

            // Load original image, resize it to fit into grid
            // TODO: Work out size without loading the whole image into memory
            using (Image sourceImage = Image.FromFile(localImagePath))
            {
                // TODO: This is just awful, so we hardcode the aspect ratio
                AspectRatio ar = new AspectRatio(3, 4);//ImageUtils.CalculateAspectRatio(sourceImage);

                // Find the width that we need to fit into
                int targetWidth = GridFlowLayoutPanel.Width - 10;
                if (standalone == false)
                {
                    targetWidth /= 2;
                }

                // There is a bug where, when we add an image to a picturebox from the buffer there is some deadspace
                // causing an overflow, meaning we have to reduce this width for now
                targetWidth -= 20;// 10; 
                int targetHeight = ar.CalculateHeight(targetWidth);

                // Resize
                Image resizedImage = ImageUtils.ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);

                // Setup image
                pictureBox.Image = resizedImage;
                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                // Save properties for later
                pictureBox.DetailedImageName = newDetailedImageName;
                pictureBox.PreviewImageName = newPreviewImageName;
                pictureBox.SetStandaloneImage(standalone);

                //// Add callbacks
                //pictureBox.Click += GridPictureBox_Click;
                //pictureBox.Paint += GridPictureBox_Paint;
            }
        }

        // TODO: Cache and retrieve (do in another thread
        private GridPictureBox? AddLocalImageToGridLayout(string previewImageName, string detailedImageName, bool standaloneImage, GridPictureBox existingPictureBox = null)
        {
            // Find the image locally and save a resized copy
            string localImagePath = Path.Combine(_workingPath, previewImageName);
            if (File.Exists(localImagePath) == false)
            {
                return null;
            }

            // Load original image, resize it to fit into grid
            // TODO: Work out size without loading the whole image into memory
            using (Image sourceImage = Image.FromFile(localImagePath))
            {
                // TODO: This is just awful, so we hardcode the aspect ratio
                AspectRatio ar = new AspectRatio(3, 4);//ImageUtils.CalculateAspectRatio(sourceImage);

                // Find the width that we need to fit into
                int targetWidth = GridFlowLayoutPanel.Width - 10;
                if (standaloneImage == false)
                {
                    targetWidth /= 2;
                }

                // There is a bug where, when we add an image to a picturebox from the buffer there is some deadspace
                // causing an overflow, meaning we have to reduce this width for now
                targetWidth -= 20;// 10; 
                int targetHeight = ar.CalculateHeight(targetWidth);

                // Increase the vertical space an image can take if it is portrait
                if (sourceImage.Height > sourceImage.Width)// || metadata.Orientation != OrientationType.Horizontal)
                {
                    targetHeight *= 2;
                }

                Image resizedImage = ImageUtils.ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);

                // Create image (or re-use image) and add it to the grid
                GridPictureBox gridPictureBox = null;
                if (existingPictureBox != null)
                {
                    // Reused picture box if we are provided one
                    gridPictureBox = existingPictureBox;
                }
                else
                {
                    // Otherwise create a new one and add to grid
                    gridPictureBox = new GridPictureBox();
                    GridFlowLayoutPanel.Controls.Add(gridPictureBox);

                    // Add callbacks
                    //gridPictureBox.Click += GridPictureBox_Click;
                    //gridPictureBox.Paint += GridPictureBox_Paint;
                    //gridPictureBox.SwapMenuItemClicked += GridPictureBox_SwapMenuItemClicked;
                    //gridPictureBox.RemoveMenuItemClicked += GridPictureBox_RemoveMenuItemClicked;
                    //gridPictureBox.StandaloneMenuItemClicked += GridPictureBox_StandaloneMenuItemClicked;

                }

                // Setup image
                gridPictureBox.Image = resizedImage;
                gridPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                // Save properties for later
                gridPictureBox.DetailedImageName = TEMP_CreateDetailedImageName(detailedImageName);
                gridPictureBox.PreviewImageName = TEMP_CreatePreviewImageName(previewImageName);
                gridPictureBox.SetStandaloneImage(standaloneImage);

                //// Add callbacks
                ///// TODO: This really show be above not here
                gridPictureBox.Click += GridPictureBox_Click;
                gridPictureBox.Paint += GridPictureBox_Paint;
                gridPictureBox.SwapMenuItemClicked += GridPictureBox_SwapMenuItemClicked;
                gridPictureBox.RemoveMenuItemClicked += GridPictureBox_RemoveMenuItemClicked;
                gridPictureBox.StandaloneMenuItemClicked += GridPictureBox_StandaloneMenuItemClicked;

                return gridPictureBox;
            }
        }

        private void TryAddColumnsBuffer(ref List<StandaloneImageSection>[] buffer, ref List<ImageSection> destination)
        {
            // First check that the buffer has anything in it
            if (buffer[0].Count == 0 && buffer[1].Count == 0)
            {
                return;
            }

            // Do the same to the second column
            for (int index = 0; index < buffer.Length; index++)
            {
                // Skip if there is nothing in the column
                if (buffer[index].Count == 0)
                {
                    continue;
                }

                // Add the column
                ColumnImageSection columnSection = new();
                columnSection.Sections = new List<StandaloneImageSection>(buffer[index].ToArray());
                destination.Add(columnSection);

                // Clear buffer 
                buffer[index].Clear();
            }
        }

        private void UpdateSelectedPreviewPictureControl(PreviewImageBox previewImageControl)
        {
            if (previewImageControl == null)
            {
                return;
            }

            if (_selectedPreviewImageControl != null)
            {
                _selectedPreviewImageControl.BackColor = BackColor;
                _selectedPreviewImageControl.SetSelected(false);
                _selectedPreviewImageControl.Invalidate();
            }

            previewImageControl.SetSelected(true);
            previewImageControl.Invalidate();


            _selectedPreviewImageControl = previewImageControl;
        }

        private bool ShowConfirmSaveDialog()
        {
            DialogResult result = MessageBox.Show(
                "Schema file already exists.\nDo you want to replace it?",
                "File exists",
                MessageBoxButtons.YesNo);

            return result == DialogResult.Yes;
        }

        // TODO: No, remove this, handle preview and detailed images better than hack hardcoding them
        private string TEMP_CreateDetailedImageName(string originalName)
        {
            //if (originalName.Contains('_') == false)
            //{
            //    return $"{originalName}_Detailed";
            //}
            //else
            //{
            //    return $"{originalName.Split('_').Last()}_Detailed";

            //}

            // LOL
            return originalName;

        }

        private string TEMP_CreatePreviewImageName(string originalName)
        {
            return originalName;
            //if (originalName.Contains('_') == false)
            //{
            //    return $"{originalName}_Preview";
            //}
            //else
            //{
            //    return $"{originalName.Split('_').Last()}_Preview";

            //}

        }

        private void PageDesignerForm_Load(object sender, EventArgs e)
        {
            if (LoadSchemaFromFile(_workingPath) == false)
            {
                CreateBlankForm();
            }
        }

        private void ImagePreviewFlowLayoutPanel_Control_Clicked(object? sender, ImageEventArgs e)
        {
            if (sender is PreviewImageBox previewImageControl)
            {
                UpdateSelectedPreviewPictureControl(previewImageControl);
            }
        }

        private void ImagePreviewFlowPanel_InsertContextItem_Clicked(object? sender, ImageEventArgs e)
        {
            List<Control> newControlCollection = new();

            foreach (Control control in GridFlowLayoutPanel.Controls)
            {
                if (control is GridPictureBox pictureBox)
                {
                    newControlCollection.Add(control);

                    if (control == _selectedGridImage)
                    {
                        GridPictureBox? newPictureBox = AddLocalImageToGridLayout(
                            TEMP_CreatePreviewImageName(e.ImageName),
                            TEMP_CreateDetailedImageName(e.ImageName),
                            false);

                        newControlCollection.Add(newPictureBox);
                    }
                }
            }

            GridFlowLayoutPanel.Controls.Clear();
            GridFlowLayoutPanel.Controls.AddRange(newControlCollection.ToArray());

            SignalSchemaChange();
        }

        private void ImagePreviewFlowLayoutPanel_Control_DoubleClicked(object? sender, ImageEventArgs e)
        {
            string imageName = e.ImageName;

            if (sender is PreviewImageBox previewImageControl)
            {
                UpdateSelectedPreviewPictureControl(previewImageControl);
                AddLocalImageToGridLayout(imageName, imageName, false);
                SignalSchemaChange();
                //// Load the preview straight into the flow panel
                //if (_previewImages.ContainsKey(imageName))
                //{
                //    PictureBox picture = new();
                //    picture.Image = _previewImages[imageName];
                //    // picture.Size = new Size(picture.Width, picture.Height);
                //    picture.SizeMode = PictureBoxSizeMode.AutoSize;
                //    GridFlowLayoutPanel.Controls.Add(picture);

                //    _lastControlInGrid = picture;
                //}
            }
        }

        private void ImagePreviewFlowPanel_AddContextItem_Clicked(object? sender, ImageEventArgs e)
        {

        }

        private void ImagePreviewFlowPanel_ReplaceContextItem_Clicked(object? sender, ImageEventArgs e)
        {
            if (sender is PreviewImageBox previewImageControl)
            {
                if (_selectedGridImage == null)
                {
                    return;
                }

                Image image = LoadImage(previewImageControl.GetImageName(), _selectedGridImage.IsStandaloneImage());
                if (image == null)
                {
                    // TODO: Show error
                    return;
                }

                _selectedGridImage.SetImage(
                    image,
                    TEMP_CreatePreviewImageName(previewImageControl.GetImageName()),
                    TEMP_CreateDetailedImageName(previewImageControl.GetImageName()),
                    _selectedGridImage.IsStandaloneImage());

                SignalSchemaChange();
            }
        }

        private void ImagePreviewFlowPanel_ImagePageContextItem_Clicked(object? sender, ImageEventArgs e)
        {
            if (sender is PreviewImageBox previewImageControl)
            {
                if (_pageImagePreviewImageBox != null && _pageImagePreviewImageBox != previewImageControl)
                {
                    _pageImagePreviewImageBox.SetPageImageChecked(false);
                }

                _pageImageName = previewImageControl.GetImageName();

                previewImageControl.SetPageImageChecked(true);
                _pageImagePreviewImageBox = previewImageControl;
            }
        }

        private void ImagePreviewFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            if (_selectedPreviewImageControl != null)
            {
                _selectedPreviewImageControl.BackColor = BackColor;
            }
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        { 
            DisplayPreviewInNewProcess();
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            SaveSchemaToFile();
        }

        private void PreviewImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedGridImage != null)
            {
                // Update grid image with new name
                // (bit inefficient to do it each time the text is updated but oh well)
                _selectedGridImage.PreviewImageName = PreviewImageTextBox.Text;
            }
        }

        private void DetailedImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedGridImage != null)
            {
                // Update grid image with new name
                // (bit inefficient to do it each time the text is updated but oh well)
                _selectedGridImage.DetailedImageName = DetailedImageTextBox.Text;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveSchemaToFile();
        }

        private void GridPictureBox_SwapMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox currentGridPictureBox)
            {
                if (_selectedGridImage == null)
                {
                    return;
                }

                Image image = _selectedGridImage.Image;
                string previewImageName = _selectedGridImage.PreviewImageName;
                string detailedImageName = _selectedGridImage.DetailedImageName;
                bool standalone = _selectedGridImage.IsStandaloneImage();

                _selectedGridImage.SetImage(
                    currentGridPictureBox.Image,
                    currentGridPictureBox.PreviewImageName,
                    currentGridPictureBox.DetailedImageName,
                    currentGridPictureBox.IsStandaloneImage());

                currentGridPictureBox.SetImage(
                    image,
                    previewImageName,
                    detailedImageName,
                    standalone);

                SignalSchemaChange();
            }
        }

        private void GridPictureBox_StandaloneMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                bool newStandaloneValue = !gridPictureBox.IsStandaloneImage();

                UpdateGridPictureBox(gridPictureBox, gridPictureBox.PreviewImageName, gridPictureBox.DetailedImageName, newStandaloneValue);
                gridPictureBox.SetStandaloneImage(newStandaloneValue);
                SignalSchemaChange();
            }
        }

        private void GridPictureBox_RemoveMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                GridFlowLayoutPanel.Controls.Remove(gridPictureBox);
                SignalSchemaChange();
            }
        }

        private void GridPictureBox_Click(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                // Update currently selected image
                if (_selectedGridImage != null)
                {
                    //_selectedGridImage.BackColor = Color.LightGray;
                    //_selectedGridImage.BorderStyle = BorderStyle.None;

                    _selectedGridImage.Invalidate();
                }
                _selectedGridImage = gridPictureBox;

                // Retrieve details from selected image
                PreviewImageTextBox.Text = gridPictureBox.PreviewImageName;
                DetailedImageTextBox.Text = gridPictureBox.DetailedImageName;

                // Highlight image
                //gridPictureBox.BorderStyle = BorderStyle.FixedSingle;
                //gridPictureBox.BackColor = Color.Blue;

                gridPictureBox.Invalidate();
            }
        }

        private void GridPictureBox_Paint(object? sender, PaintEventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                if (gridPictureBox == _selectedGridImage)
                {
                    //Rectangle rect = new Rectangle()
                    using (Brush b = new SolidBrush(Color.FromArgb(100, Color.Blue)))
                    {
                        e.Graphics.FillRectangle(b, gridPictureBox.DisplayRectangle);
                    }

                }
            }
        }

        private void FormTextBox_TextChanged(object sender, EventArgs e)
        {
            SignalSchemaChange();
        }

        #region Menu Strip Callbacks

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog folderDialog = new())
            {
                folderDialog.Description = "Pick a directory to create a new page";
                folderDialog.UseDescriptionForTitle = true;
                folderDialog.ShowNewFolderButton = true;
                folderDialog.InitialDirectory = Directory.Exists(_workingPath) ? _workingPath : Environment.CurrentDirectory;

                DialogResult result = folderDialog.ShowDialog();
                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(folderDialog.SelectedPath))
                {
                    if (File.Exists(Path.Combine(folderDialog.SelectedPath, "SCHEMA")))
                    {
                        MessageBox.Show(
                            $"Can't create new page in directory, SCHEMA file already exists.",
                            "Carpenter",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
                        return;
                    }

                    _workingPath = folderDialog.SelectedPath;
                    CreateBlankForm();
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveSchemaToFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Load Carpenter SCHEMA file";
                fileDialog.InitialDirectory = Directory.Exists(_workingPath) ? _workingPath : Environment.CurrentDirectory;
                fileDialog.RestoreDirectory = true;
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Filter = "Carpenter Schema File (SCHEMA)|SCHEMA";
                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                    LoadSchemaFromFile(Path.GetDirectoryName(fileDialog.FileName));
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void webpageToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void previewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DisplayPreviewInNewProcess();
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UndoSchemaChanges();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedoSchemaChanges();
        }

        private void importToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowImportImageDialog();
        }

        private void ShowImportImageDialog()
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Load SC4Cartographer map properties";
                fileDialog.InitialDirectory = Directory.GetCurrentDirectory();
                fileDialog.RestoreDirectory = true;
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Multiselect = true;
                fileDialog.Filter = kImageFileFilter;

                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    List<string> exceptions = new();
                    foreach (string filePath in fileDialog.FileNames)
                    {
                        if (File.Exists(filePath))
                        {
                            string fileName = Path.GetFileName(filePath);
                            try
                            {
                                File.Copy(filePath, Path.Combine(_workingPath, fileName));
                            }
                            catch (Exception copyException)
                            {
                                exceptions.Add(copyException.GetType().Name);
                            }
                        }
                    }

                    if (exceptions.Count > 0)
                    {
                        MessageBox.Show(
                            $"Could not find import {exceptions.Count} files.",
                            "Carpenter",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning);
                    }

                    LoadAvailableImagePreviews();
                }
            }
        }

        #endregion

        private void adddetailedPrefixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Control control in GridFlowLayoutPanel.Controls)
            {
                if (control is GridPictureBox gridPictureBox)
                {
                    gridPictureBox.DetailedImageName = gridPictureBox.PreviewImageName.Replace("_Preview", "_Detailed");
                }
            }
        }

        private void livePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isLivePreviewFormActive)
            {
                return;
            }

            if (_livePreviewForm == null)
            {
                _livePreviewForm = new();
                _livePreviewForm.ShowInTaskbar = true;
                _livePreviewForm.ShowIcon = false;

                _livePreviewForm.Shown += livePreviewForm_Shown;
                _livePreviewForm.FormClosed += livePreviewForm_FormClosed;
            }

            _livePreviewForm.StartPosition = FormStartPosition.CenterParent;
            _livePreviewForm.Show(Owner);
        }

        private void livePreviewForm_Shown(object? sender, EventArgs e)
        {
            _isLivePreviewFormActive = true;
            _livePreviewForm.LoadUrl(GeneratePreviewWebpage());
        }

        private void livePreviewForm_FormClosed(object? sender, FormClosedEventArgs e)
        {
            _isLivePreviewFormActive = false;
        }


    }
}
