using Carpenter;
using Carpenter.PageDesigner;
using CefSharp.DevTools.Network;
using JpegMetadataExtractor;
using PageDesigner.Controls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
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
    public partial class PageDesignerForm : Form
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

        /// <summary>
        /// Stores and calculates the aspect ratio of an image
        /// </summary>
        public struct AspectRatio
        {
            public readonly int Width;
            public readonly int Height;

            public AspectRatio(int width, int height)
            {
                // TODO: Where or why is this reversed?????
                Width = height;
                Height = width;
            }

            public int CalculateHeight(int width)
            {
                Debug.Assert(Width != 0);
                Debug.Assert(Height != 0);

                int factor = width / Width;
                return Height * factor;
            }

            public int CalculateWidth(int height)
            {
                Debug.Assert(Width != 0);
                Debug.Assert(Height != 0);

                int factor = height / Height;
                return Width * factor;
            }

            public override string ToString()
            {
                return string.Format("{0}:{1}", Width, Height);
            }
        }

        /// <remarks>
        /// https://stackoverflow.com/a/24199315
        /// </remarks>
        private const int MaxResizeImageCacheSize = 30;
        private static Queue<int> _resizeHashes = new();
        private static Dictionary<int, Bitmap> _resizeImageCache = new();
        private static Bitmap ResizeImage(string path, Image sourceImage, int width, int height)
        {
            // TODO: How unique really is this
            int hash = path.GetHashCode() * width * height;
            if (_resizeImageCache.ContainsKey(hash))
            {
                return _resizeImageCache[hash];
            }

            Rectangle destRect = new(0, 0, width, height);
            Bitmap destImage = new(width, height);

            destImage.SetResolution(sourceImage.HorizontalResolution, sourceImage.VerticalResolution);

            using (Graphics graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (ImageAttributes attributes = new())
                {
                    attributes.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(sourceImage, destRect, 0, 0, sourceImage.Width, sourceImage.Height, GraphicsUnit.Pixel, attributes);
                }
            }

            // Remove oldest item from cache when limit reached
            if (_resizeImageCache.Count > MaxResizeImageCacheSize)
            {
                _resizeImageCache.Remove(_resizeHashes.Dequeue());
            }
            _resizeImageCache.Add(hash, destImage);
            _resizeHashes.Enqueue(hash);

            return destImage;
        }

        /// <summary>
        /// Returns the greatest common factor of 2 numbers, used when caculating the aspect ratio of an image
        /// </summary>
        /// <remarks>
        /// https://stackoverflow.com/a/20824923
        /// </remarks>
        private static int GreatestCommonFactor(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        /// <summary>
        /// Calculates the lowest common factor of 2 numbers
        /// </summary>
        private static int LowestCommonMultiple(int a, int b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        private const string kUnsavedTitleString = " *";
        private const string kImageFileFilter = "Image Files(*.JPG;*.JPEG)|*.JPG;*.JPEG|All files (*.*)|*.*";

        private string _workingPath;
        private string _pagePath;
        private Page _modifiedPage;
        private Page _savedPage;
        private Site _site;
        private PreviewImageBox _selectedPreviewImageControl;
        private GridPictureBox _selectedGridImage;
        private PreviewImageBox _pageImagePreviewImageBox;
        private Dictionary<string, Image> _previewImages = new();
        private Queue<GridPictureBox> _pictureBoxBuffer = new();
        private ChangesStack<Page> _pageChanges = new();

        private LivePreviewForm _livePreviewForm = null;
        private bool _isLivePreviewFormActive = false;
        private string _pageImageName;

        // TODO: DRY
        public PageDesignerForm()
        {
            InitializeComponent();

            _workingPath = Environment.CurrentDirectory;

            JpegParser.UseInternalCache = true;
        }
        public PageDesignerForm(string path, Site site) : this()
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

            if (site == null || site.IsLoaded() == false)
            {
                MessageBox.Show(
                    $"Not provided valid Site, please check and try again",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _site = site;
            _workingPath = path;
            _pagePath = Path.Combine(_workingPath, Config.kPageFileName);
            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);
        }
        public PageDesignerForm(string? path, string? siteRootPath) : this()
        {
            if (Directory.Exists(path) == false || Directory.Exists(siteRootPath) == false)
            {
                MessageBox.Show(
                    $"Could not find page path or site root path, please check and try again",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            try
            {
                _site = new Site(siteRootPath);
                if (_site.IsLoaded() == false)
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Could not parse site ({ex.GetType()}). Check format and try again.",
                    "Carpenter",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                return;
            }

            _workingPath = path;
            _pagePath = Path.Combine(_workingPath, Config.kPageFileName);
        }

        private void AddTextboxCallbacks()
        {
            TitleTextBox.TextChanged += FormTextBox_TextChanged;
            ThumbnailTextBox.TextChanged += FormTextBox_TextChanged;
            LocationTextBox.TextChanged += FormTextBox_TextChanged;
            MonthTextBox.TextChanged += FormTextBox_TextChanged;
            YearTextBox.TextChanged += FormTextBox_TextChanged;
            AuthorTextBox.TextChanged += FormTextBox_TextChanged;
            CameraTextBox.TextChanged += FormTextBox_TextChanged;
            DescriptionTextBox.TextChanged += FormTextBox_TextChanged;
        }

        private void RemoveTextboxCallbacks()
        {
            TitleTextBox.TextChanged -= FormTextBox_TextChanged;
            ThumbnailTextBox.TextChanged -= FormTextBox_TextChanged;
            LocationTextBox.TextChanged -= FormTextBox_TextChanged;
            MonthTextBox.TextChanged -= FormTextBox_TextChanged;
            YearTextBox.TextChanged -= FormTextBox_TextChanged;
            AuthorTextBox.TextChanged -= FormTextBox_TextChanged;
            CameraTextBox.TextChanged -= FormTextBox_TextChanged;
            DescriptionTextBox.TextChanged -= FormTextBox_TextChanged;
        }

        private string GeneratePreviewWebpage()
        {
            if (_modifiedPage == null)
            {
                return string.Empty;
            }

            // TODO: Check if page has changed before loading
            UpdateModifiedPageFromForm();

            // Generate preview page
            Stopwatch stopwatch = Stopwatch.StartNew();

            // TODO: Hadle exceptions
            HtmlGenerator.BuildHtmlForPage(_modifiedPage, _site, true);
            string previewPath = Path.Combine(_workingPath, "index_Preview.html");

            stopwatch.Stop();
            statusToolStripStatusLabel.Text = string.Format("Generated Preview in {0}ms", stopwatch.ElapsedMilliseconds);

            return previewPath;
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
            _savedPage = new Page();
            _modifiedPage = new Page();
            _pageChanges.Reset();

            // Do a bit of recon to try and populate some of these fields
            string folderName = Path.GetFileName(_workingPath);
            string title = string.Empty;
            string[] titleWords = folderName.Split('-');
            foreach (string word in titleWords)
            {
                if (int.TryParse(word, out int numeric))
                    title += string.Format(" (Vol. {0})", numeric);
                else
                    title += string.Format("{2}{0}{1}", char.ToUpper(word[0]), word.Substring(1), word == titleWords.First() ? "" : " ");
            }

            string month = string.Empty;
            string year = string.Empty;
            string model = string.Empty;
            foreach (string imagePath in Directory.GetFiles(_workingPath, "*.jpg"))
            {
                try
                {
                    ImageMetadata metadata = JpegParser.GetMetadata(imagePath);
                    string exifDate = metadata.CreatedDate;
                    model = metadata.CameraModel;
                    if (exifDate != string.Empty)
                    {
                        DateTime dateCreated = DateTime.ParseExact(exifDate.Replace("\0", ""), "yyyy:MM:dd HH:mm:ss", CultureInfo.InvariantCulture);
                        month = dateCreated.ToString("MMMM");
                        year = dateCreated.ToString("yyyy");
                    }
                }
                catch (Exception) { }

                // Silently continue till we find a photo we can parse or we run out of photos
                if (year != string.Empty && month != string.Empty && model != string.Empty)
                {
                    break;
                }
            }

            TitleTextBox.Text = title == string.Empty ? Settings.Default.TitleLastUsedValue : title;
            MonthTextBox.Text = month == string.Empty ? Settings.Default.MonthLastUsedValue : month;
            YearTextBox.Text = year == string.Empty ? Settings.Default.YearLastUsedValue : year;
            CameraTextBox.Text = model == string.Empty ? Settings.Default.CameraLastUsedValue : model;

            LocationTextBox.Text = Settings.Default.LocationLastUsedValue;
            AuthorTextBox.Text = Settings.Default.AuthorLastUsedValue;
            //ThumbnailTextBox.Text = Settings.Default.ThumbnailLastUsedValue;

            GridFlowLayoutPanel.Controls.Clear();
            ImagePreviewFlowLayoutPanel.Controls.Clear();
            AddTextboxCallbacks();
        }

        private void CreateBlankForm()
        {
            ResetForm();
            LoadAvailableImagePreviews();
            SignalPageChange();
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

        private bool LoadPageFromFile(string path)
        {
            string pagePath = Path.Combine(path, Config.kPageFileName);
            if (File.Exists(pagePath) == false)
            {
                return false;
            }

            Page? loadedPage = null;
            try
            {
                loadedPage = new Page(pagePath);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Could not load page. Exception {e.GetType()}: {e.Message}",
                    "Error loading page", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            // Clear any old data before continuing
            ResetForm();

            // We are passed the point where loading could fail, setup the page for displaying the page
            _workingPath = path;
            _pagePath = pagePath;
            _savedPage = loadedPage;
            _modifiedPage = new Page(_savedPage);

            _pageImageName = _modifiedPage.Thumbnail;

            UpdateFormFromModifiedPage();
            LoadAvailableImagePreviews();

            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);

            return true;
        }

        private void SavePageToFile()
        {
            if (_savedPage != null && _modifiedPage == _savedPage)
            {
                return;
            }

            //if (addDetailedPrefixToolStripMenuItem.Checked)
            //{
            //    foreach (Control control in GridFlowLayoutPanel.Controls)
            //    {
            //        if (control is GridPictureBox gridPictureBox)
            //        {
            //            gridPictureBox.DetailImageFilename = gridPictureBox.ImageFilename.Replace("_Preview", "_Detailed");
            //        }
            //    }
            //}

            UpdateModifiedPageFromForm();

            //if (File.Exists(_pagePath))
            //{
            //    if (ShowConfirmSaveDialog() == false)
            //    {
            //        return;
            //    }
            //}

            // TODO: Show an error when page save fails
            if (_modifiedPage.TrySave(Path.GetDirectoryName(_pagePath)))
            {
                statusToolStripStatusLabel.Text = "Page successfully saved.";

                this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);

                // Save whatever values we entered for next time!
                Settings.Default.ThumbnailLastUsedValue = ThumbnailTextBox.Text;
                Settings.Default.TitleLastUsedValue = TitleTextBox.Text;
                Settings.Default.LocationLastUsedValue = LocationTextBox.Text;
                Settings.Default.MonthLastUsedValue = MonthTextBox.Text;
                Settings.Default.YearLastUsedValue = YearTextBox.Text;
                Settings.Default.AuthorLastUsedValue = AuthorTextBox.Text;
                Settings.Default.CameraLastUsedValue = CameraTextBox.Text;
                Settings.Default.Save();
            }
            else
            {

                MessageBox.Show("Could not save page.", "Error saving page", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }

        private void UpdateModifiedPageFromForm()
        {
            if (_modifiedPage == null)
            {
                return;
            }

            // Update tokens values from page
            _modifiedPage.Title = TitleTextBox.Text;
            _modifiedPage.Location = LocationTextBox.Text;
            _modifiedPage.Month = MonthTextBox.Text;
            _modifiedPage.Year = YearTextBox.Text;
            _modifiedPage.Author = AuthorTextBox.Text;
            _modifiedPage.Camera = CameraTextBox.Text;
            _modifiedPage.Thumbnail = ThumbnailTextBox.Text;
            _modifiedPage.Description = DescriptionTextBox.Text;

            // TODO: Have these set via a form on the main form or have them stored somewhere else
            _modifiedPage.GeneratedFilename = Config.kDefaultGeneratedFilename;

            // Parse grid layout
            List<Section> sections = new();
            List<ImageSection>[] columnsBuffer = new List<ImageSection>[]
            {
                new List<ImageSection>(),
                new List<ImageSection>()
            };
            foreach (Control control in GridFlowLayoutPanel.Controls)
            {
                if (control is GridPictureBox gridPictureBox)
                {
                    // Create standalone image from GridPictureBox
                    ImageSection currentImageSection = new()
                    {
                        ImageUrl = gridPictureBox.ImageFilename
                    };
                    if (gridPictureBox.DetailImageFilename != string.Empty 
                        && gridPictureBox.DetailImageFilename != gridPictureBox.ImageFilename)
                    {
                        currentImageSection.AltImageUrl = gridPictureBox.DetailImageFilename;
                    }

                    if (gridPictureBox.GetImageType() == GridPictureBox.ImageType.Standalone)
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

            _modifiedPage.LayoutSections = sections;
        }

        private void UpdateFormFromModifiedPage()
        {
            PopulateTextboxesFromModifiedPage();
            PopulateGridFromModifiedPage();
        }

        private void PopulateGridFromModifiedPage()
        {
            GridFlowLayoutPanel.Controls.Clear();
            foreach (Section section in _modifiedPage.LayoutSections)
            {
                Type sectionType = section.GetType();
                if (sectionType == typeof(ImageSection))
                {
                    ImageSection? standaloneSection = section as ImageSection;
                    if (standaloneSection != null)
                    {
                        string fileName = standaloneSection.ImageUrl;

                        AddLocalImageToGridLayout(standaloneSection.ImageUrl, standaloneSection.AltImageUrl, true);
                    }
                }
                else if (sectionType == typeof(ImageColumnSection))
                {
                    ImageColumnSection columnSection = (ImageColumnSection)section;
                    if (columnSection == null)
                    {
                        continue;
                    }

                    if (_pictureBoxBuffer.Count > 0)
                    {
                        foreach (ImageSection standaloneImage in columnSection.Sections)
                        {
                            GridPictureBox pictureBox = null;
                            if (_pictureBoxBuffer.Count > 0)
                            {
                                pictureBox = _pictureBoxBuffer.Dequeue();
                            }

                            AddLocalImageToGridLayout(standaloneImage.ImageUrl, standaloneImage.AltImageUrl, false, pictureBox);
                        }

                        // Clear anything left in the buffer (this isn't great)
                        _pictureBoxBuffer.Clear();
                    }
                    else
                    {
                        foreach (ImageSection standaloneImage in columnSection.Sections)
                        {
                            if (standaloneImage != null)
                            {
                                AddLocalImageToGridLayout(standaloneImage.ImageUrl, standaloneImage.AltImageUrl, false);

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

        private void PopulateTextboxesFromModifiedPage()
        {
            // We don't want any callbacks triggering when we populate them
            // We only want them to trigger when the user edits them
            RemoveTextboxCallbacks();

            string GetTokenFromPage(Page.Tokens token, string defaultValue)
            {
                if (_modifiedPage == null || _modifiedPage.TokenValues.ContainsKey(token) == false)
                {
                    return defaultValue;
                }

                return _modifiedPage.TokenValues[token];
            }

            TitleTextBox.Text = GetTokenFromPage(Page.Tokens.Title, Settings.Default.TitleLastUsedValue);
            LocationTextBox.Text = GetTokenFromPage(Page.Tokens.Location, Settings.Default.LocationLastUsedValue);
            MonthTextBox.Text = GetTokenFromPage(Page.Tokens.Month, Settings.Default.MonthLastUsedValue);
            YearTextBox.Text = GetTokenFromPage(Page.Tokens.Year, Settings.Default.YearLastUsedValue);
            AuthorTextBox.Text = GetTokenFromPage(Page.Tokens.Author, Settings.Default.AuthorLastUsedValue);
            CameraTextBox.Text = GetTokenFromPage(Page.Tokens.Camera, Settings.Default.CameraLastUsedValue);
            ThumbnailTextBox.Text = GetTokenFromPage(Page.Tokens.Thumbnail, Settings.Default.ThumbnailLastUsedValue);
            DescriptionTextBox.Text = GetTokenFromPage(Page.Tokens.Description, "");

            AddTextboxCallbacks();
        }

        private void SignalPageChange()
        {
            if (_modifiedPage == _pageChanges.GetCurrentChange())
            {
                return;
            }

            UpdateModifiedPageFromForm();
            _pageChanges.Commit(new Page(_modifiedPage));
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

        private void UndoPageChanges()
        {
            Page lastPageChange = new Page(_pageChanges.Undo());
            if (lastPageChange != _modifiedPage)
            {
                _modifiedPage = lastPageChange;

                // Only re-populate the grid if it has actually changed 
                // NOTE: Equality check isn't working
                //if (_modifiedPage.ImageSections != lastPageChange.ImageSections)
                //{
                //    PopulateGridFromModifiedPage();
                //}
                PopulateGridFromModifiedPage();
                PopulateTextboxesFromModifiedPage();


                // Remove unsaved character from title bar if we are at parity with the last saved page
                if (_modifiedPage == _savedPage)
                {
                    this.Text = this.Text.Replace(kUnsavedTitleString, "");
                }
            }
        }

        private void RedoPageChanges()
        {
            Page pageChange = new Page(_pageChanges.Redo());
            if (pageChange != _modifiedPage)
            {
                _modifiedPage = pageChange;

                if (_modifiedPage.LayoutSections != pageChange.LayoutSections)
                {
                    PopulateGridFromModifiedPage();
                }
                PopulateTextboxesFromModifiedPage();


                // Remove unsaved character from title bar if we are at parity with the last saved page
                if (_modifiedPage == _savedPage)
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

                return ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);
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

            JpegParser.CacheSize = imageFilesAtPath.Count();
            JpegParser.ClearCache();

            _previewImages.Clear();
            foreach (string imagePath in imageFilesAtPath)
            {
                if (Path.GetFileName(imagePath).Contains("_Detailed"))
                {
                    // We don't want to load the bigger image files that won't be used directly on the webpage
                    continue;
                }

                // TODO: Have a fallback
                //byte[] thumbnailData = JpegParser.GetThumbnailData(imagePath);
                //using (MemoryStream stream = new MemoryStream(thumbnailData))
                ////using (Image originalImage = Image.FromFile(imagePath))
                //using (Image originalImage = Image.FromStream(stream))

                // Load the metadata for the image (this will cache it for later)
                JpegParser.GetRawMetadata(imagePath);

                using (Image originalImage = ExtractThumbnail(imagePath))
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
            // TODO: Have a fallback
            //byte[] thumbnailData = JpegParser.GetThumbnailData(localImagePath);
            //using (MemoryStream stream = new MemoryStream(thumbnailData))
            ////using (Image originalImage = Image.FromFile(imagePath))
            //using (Image sourceImage = Image.FromStream(stream))

            //using (Image sourceImage = Image.FromFile(localImagePath))


            using (Image sourceImage = ExtractThumbnail(localImagePath))
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
                Image resizedImage = ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);

                // Setup image
                pictureBox.Image = resizedImage;
                pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;

                // Save properties for later
                pictureBox.DetailImageFilename = newDetailedImageName;
                pictureBox.ImageFilename = newPreviewImageName;
                pictureBox.SetImageType(standalone ? GridPictureBox.ImageType.Standalone : GridPictureBox.ImageType.Column);

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
            //byte[] thumbnailData = JpegParser.GetThumbnailData(localImagePath);
            //using (MemoryStream stream = new MemoryStream(thumbnailData))
            ////using (Image originalImage = Image.FromFile(imagePath))
            //using (Image sourceImage = Image.FromStream(stream))
            ////using (Image sourceImage = Image.FromFile(localImagePath))

            using (Image sourceImage = ExtractThumbnail(localImagePath))
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

                Image resizedImage = ResizeImage(localImagePath, sourceImage, targetWidth, targetHeight);

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
                //gridPictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;


                // Save properties for later
                //gridPictureBox.DetailImageFilename = TEMP_CreateDetailedImageName(detailedImageName);
                gridPictureBox.ImageFilename = TEMP_CreatePreviewImageName(previewImageName);
                gridPictureBox.SetImageType(standaloneImage ? GridPictureBox.ImageType.Standalone : GridPictureBox.ImageType.Column);

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

        private Image ExtractThumbnail(string imagePath)
        {
            byte[] thumbnailData = JpegParser.GetThumbnailData(imagePath);
            if (thumbnailData.Length == 0)
            {
                return Image.FromFile(imagePath);
            }

            using (MemoryStream stream = new MemoryStream(thumbnailData))
            {
                return Image.FromStream(stream);
            }
        }

        private void TryAddColumnsBuffer(ref List<ImageSection>[] buffer, ref List<Section> destination)
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
                ImageColumnSection columnSection = new();
                columnSection.Sections = new List<ImageSection>(buffer[index].ToArray());
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
            if (LoadPageFromFile(_workingPath) == false)
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

            SignalPageChange();
        }

        private void ImagePreviewFlowLayoutPanel_Control_DoubleClicked(object? sender, ImageEventArgs e)
        {
            string imageName = e.ImageName;

            if (sender is PreviewImageBox previewImageControl)
            {
                UpdateSelectedPreviewPictureControl(previewImageControl);
                AddLocalImageToGridLayout(imageName, imageName, false);
                SignalPageChange();
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

                Image image = LoadImage(previewImageControl.GetImageName(), _selectedGridImage.GetImageType() == GridPictureBox.ImageType.Standalone);
                if (image == null)
                {
                    // TODO: Show error
                    return;
                }

                _selectedGridImage.SetImage(
                    image,
                    TEMP_CreatePreviewImageName(previewImageControl.GetImageName()),
                    TEMP_CreateDetailedImageName(previewImageControl.GetImageName()),
                    _selectedGridImage.GetImageType());

                SignalPageChange();
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
                ThumbnailTextBox.Text = _pageImageName;
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
            SavePageToFile();
        }

        private void PreviewImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedGridImage != null)
            {
                // Update grid image with new name
                // (bit inefficient to do it each time the text is updated but oh well)
                _selectedGridImage.ImageFilename = PreviewImageTextBox.Text;
            }
        }

        private void DetailedImageTextBox_TextChanged(object sender, EventArgs e)
        {
            if (_selectedGridImage != null)
            {
                // Update grid image with new name
                // (bit inefficient to do it each time the text is updated but oh well)
                _selectedGridImage.DetailImageFilename = DetailedImageTextBox.Text;
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SavePageToFile();
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
                string previewImageName = _selectedGridImage.ImageFilename;
                string detailedImageName = _selectedGridImage.DetailImageFilename;
                GridPictureBox.ImageType imageType = _selectedGridImage.GetImageType();

                _selectedGridImage.SetImage(
                    currentGridPictureBox.Image,
                    currentGridPictureBox.ImageFilename,
                    currentGridPictureBox.DetailImageFilename,
                    currentGridPictureBox.GetImageType());

                currentGridPictureBox.SetImage(
                    image,
                    previewImageName,
                    detailedImageName,
                    imageType);

                SignalPageChange();
            }
        }

        private void GridPictureBox_StandaloneMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                GridPictureBox.ImageType newImageType = gridPictureBox.GetImageType() == GridPictureBox.ImageType.Standalone ? GridPictureBox.ImageType.Column : GridPictureBox.ImageType.Standalone;

                UpdateGridPictureBox(gridPictureBox, gridPictureBox.ImageFilename, gridPictureBox.DetailImageFilename, newImageType == GridPictureBox.ImageType.Standalone);
                gridPictureBox.SetImageType(newImageType);
                SignalPageChange();
            }
        }

        private void GridPictureBox_RemoveMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                GridFlowLayoutPanel.Controls.Remove(gridPictureBox);
                SignalPageChange();
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
                PreviewImageTextBox.Text = gridPictureBox.ImageFilename;
                DetailedImageTextBox.Text = gridPictureBox.DetailImageFilename;

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
            SignalPageChange();
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
                    if (File.Exists(Path.Combine(folderDialog.SelectedPath, Config.kDefaultGeneratedFilename)))
                    {
                        MessageBox.Show(
                            $"Can't create new page in directory, PAGE file already exists.",
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
            SavePageToFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.Title = "Load Carpenter PAGE file";
                fileDialog.InitialDirectory = Directory.Exists(_workingPath) ? _workingPath : Environment.CurrentDirectory;
                fileDialog.RestoreDirectory = true;
                fileDialog.CheckFileExists = true;
                fileDialog.CheckPathExists = true;
                fileDialog.Filter = "Carpenter Page File (PAGE)|PAGE";
                if (fileDialog.ShowDialog(this) == DialogResult.OK)
                    LoadPageFromFile(Path.GetDirectoryName(fileDialog.FileName));
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
            UndoPageChanges();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RedoPageChanges();
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
            addDetailedPrefixToolStripMenuItem.Checked = !addDetailedPrefixToolStripMenuItem.Checked;
            Settings.Default.ConvertImageNames = addDetailedPrefixToolStripMenuItem.Checked;
            Settings.Default.Save();
        }

        private void livePreviewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_isLivePreviewFormActive)
            {
                return;
            }

            if (_livePreviewForm == null || _livePreviewForm.IsDisposed)
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

        private void openInExplorerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(_workingPath))
            {
                Process.Start(_workingPath);
            }
        }

        private void openInNotepadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (File.Exists(_pagePath))
            {
                Process.Start("notepad", _pagePath);
            }
        }

        private void PageDesignerForm_Resize(object sender, EventArgs e)
        {
            foreach (Control ctrl in GridFlowLayoutPanel.Controls)
            {
                // An attempt to make the grid resize
                //ctrl.Width = GridFlowLayoutPanel.ClientSize.Width / GridFlowLayoutPanel.Controls.Count;
            }
        }
}
}
