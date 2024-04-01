﻿using Carpenter;
using PageDesigner.Controls;
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
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.Pkcs;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageDesigner
{


    public partial class PageDesignerForm : Form
    {
        private string _workingPath;
        private string _schemaPath;
        private Schema _modifiedSchema;
        private Schema _originalSchema;
        private Template _template;

        PreviewImageBox _selectedPreviewImageControl; // TODO: Rename _selectedPreviewImageControl
        GridPictureBox _selectedGridImage;
        Dictionary<string, Image> _previewImages = new();
        Queue<GridPictureBox> _pictureBoxBuffer = new();

        public PageDesignerForm(string path, Template template)
        {
            InitializeComponent();

            _workingPath = path;
            _template = template;

            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);
        }

        public string GetSelectedPreviewImageName() { return _selectedPreviewImageControl == null ? string.Empty : _selectedPreviewImageControl.GetImageName; }
        public string GetSelectedGridImageName() { return _selectedGridImage == null ? string.Empty : _selectedGridImage.PreviewImageName; }

        private void PageDesignerForm_Load(object sender, EventArgs e)
        {
            if (LoadSchemaIfAvailable())
            {
                SetupBlankSchema();
            }

            LoadAvailableImagePreviews();

        }

        private void SetupBlankSchema()
        {
            // Just leave this blank
            _modifiedSchema = new Schema();

            // Fill in some default values
            // TODO: Populate from program settings
            BaseUrlTextBox.Text = "https://matthewcarney.info";
            PageUrlTextBox.Text = "photos/Sept-2016";
            TitleTextBox.Text = "Donegal, Ireland";
            LocationTextBox.Text = "Ireland";
            MonthTextBox.Text = "September";
            YearTextBox.Text = "2016";
            AuthorTextBox.Text = "Matthew Carney";
            CameraTextBox.Text = "Canon EOS 600D";

        }

        private bool LoadSchemaIfAvailable()
        {
            _schemaPath = Path.Combine(_workingPath, "SCHEMA");
            if (File.Exists(_schemaPath) == false)
            {
                return false;
            }

            _originalSchema = new Schema();
            if (_originalSchema.Load(_schemaPath) == false)
            {
                // TODO: Raise an error or something
                return false;
            }

            // Create a copy to actually work on
            _modifiedSchema = new Schema(_originalSchema);

            // Populate text fields first
            // TODO: Store default values in a application settings somewhere
            BaseUrlTextBox.Text = GetTokenFromSchema(Schema.Token.BaseUrl, "https://matthewcarney.info");
            PageUrlTextBox.Text = GetTokenFromSchema(Schema.Token.PageUrl, "photos/Sept-2016");
            TitleTextBox.Text = GetTokenFromSchema(Schema.Token.Title, "Donegal, Ireland");
            LocationTextBox.Text = GetTokenFromSchema(Schema.Token.Location, "Ireland");
            MonthTextBox.Text = GetTokenFromSchema(Schema.Token.Month, "September");
            YearTextBox.Text = GetTokenFromSchema(Schema.Token.Year, "2016");
            AuthorTextBox.Text = GetTokenFromSchema(Schema.Token.Author, "Matthew Carney");
            CameraTextBox.Text = GetTokenFromSchema(Schema.Token.Camera, "Canon EOS 600D");

            // Populate the grid
            // TODO: Cleanup
            foreach (ImageSection section in _modifiedSchema.ImageSections)
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
                        // TODO: Error and stuff
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

                // TODO: support the text sections..
            }

            return true;
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

                }

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
                bool standalone = _selectedGridImage.IsStandaloneImage;

                _selectedGridImage.SetImage(
                    currentGridPictureBox.Image,
                    currentGridPictureBox.PreviewImageName,
                    currentGridPictureBox.DetailedImageName,
                    currentGridPictureBox.IsStandaloneImage);

                currentGridPictureBox.SetImage(
                    image,
                    previewImageName,
                    detailedImageName,
                    standalone);

            }
        }

        private void GridPictureBox_StandaloneMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                bool newStandaloneValue = !gridPictureBox.IsStandaloneImage;

                UpdateGridPictureBox(gridPictureBox, gridPictureBox.PreviewImageName, gridPictureBox.DetailedImageName, newStandaloneValue);
                gridPictureBox.SetStandaloneImage(newStandaloneValue);
            }
        }

        private void GridPictureBox_RemoveMenuItemClicked(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                GridFlowLayoutPanel.Controls.Remove(gridPictureBox);
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

        private string GetTokenFromSchema(Schema.Token token, string defaultValue)
        {
            if (_modifiedSchema == null || _modifiedSchema.TokenValues.ContainsKey(token) == false)
            {
                return defaultValue;
            }

            return _modifiedSchema.TokenValues[token];
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

            // TODO: Thread
            foreach (string imagePath in imageFilesAtPath)
            {
                using (Image originalImage = Image.FromFile(imagePath))
                {
                    // TODO: Wish we didn't have to force aspectratio but CalculateAspectRatio is broken
                    AspectRatio ar = new AspectRatio(3, 4);//ImageUtils.CalculateAspectRatio(originalImage);

                    int desiredWidth = 120;
                    int desiredHeight = ar.CalculateHeight(desiredWidth);

                    // TODO: Use DrawImage to properly resize
                    Image previewImage = originalImage.GetThumbnailImage(desiredWidth, desiredHeight, () => false, IntPtr.Zero);

                    _previewImages.Add(Path.GetFileName(imagePath), previewImage);

                    // Cache in temp directory
                    // TODO: Fix with aspect ratio
                    PreviewImageBox previewImageBox = new PreviewImageBox(
                        Path.GetFileName(imagePath), previewImage);
                    //FetchOrCatchPreviewImage(Path.GetFileName(filename), inputtedPath));  //

                    //previewImageBox.MouseClick += ImagePreviewFlowLayoutPanel_MouseClick;
                    previewImageBox.ControlClicked += ImagePreviewFlowLayoutPanel_Control_Clicked;
                    previewImageBox.ControlDoubleClicked += ImagePreviewFlowLayoutPanel_Control_DoubleClicked;
                    previewImageBox.AddContextItemClicked += ImagePreviewFlowLayoutPanel_Control_DoubleClicked;
                    previewImageBox.InsertContextItemClicked += ImagePreviewFlowPanel_InsertContextItem_Clicked;
                    previewImageBox.ReplaceContextItemClicked += ImagePreviewFlowPanel_ReplaceContextItem_Clicked;

                    ImagePreviewFlowLayoutPanel.Controls.Add(previewImageBox);
                }
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
                        GridPictureBox newPictureBox = CreateGridPictureBox(e.ImageName, e.ImageName, false);
                        newControlCollection.Add(newPictureBox);
                    }
                }
            }

            GridFlowLayoutPanel.Controls.Clear();
            GridFlowLayoutPanel.Controls.AddRange(newControlCollection.ToArray());
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

        private void ImagePreviewFlowLayoutPanel_Control_DoubleClicked(object? sender, ImageEventArgs e)
        {
            string imageName = e.ImageName;

            if (sender is PreviewImageBox previewImageControl)
            {
                UpdateSelectedPreviewPictureControl(previewImageControl);



                AddLocalImageToGridLayout(imageName, imageName, false);


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
 


            GeneratePreview();

        }


        // TODO: Move to utils 
        private void UpdateSchemaFromForm()
        {
            if (_modifiedSchema == null)
            {
                return;
            }

            // Update tokens values from page
            // TODO: Copy or add class id and option base values
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.BaseUrl, BaseUrlTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.PageUrl, PageUrlTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.Title, TitleTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.Location, LocationTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.Month, MonthTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.Year, YearTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.Author, AuthorTextBox.Text);
            _modifiedSchema.TokenValues.AddOrUpdate(Schema.Token.Camera, CameraTextBox.Text);

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

                    if (gridPictureBox.IsStandaloneImage)
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

            _modifiedSchema.ImageSections = sections;
        }

        // TODO: Move to utils 
        private void GeneratePreview()
        {
            if (_modifiedSchema == null || _template == null)
            {
                return;
            }

            UpdateSchemaFromForm();

            // Generate preview page
            if (_template.Generate(_modifiedSchema, _workingPath, true))
            {
                string originalOutputFile = _modifiedSchema.OptionValues[Schema.Option.OutputFilename];
                string previewName = Path.GetFileNameWithoutExtension(originalOutputFile) + "_preview";
                string previewPath = Path.Combine(_workingPath, previewName + Path.GetExtension(originalOutputFile));

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

        // TODO: rename
        private void SaveSchema()
        {
            if (_originalSchema != null && _modifiedSchema == _originalSchema)
            {
                return;
            }

            if (File.Exists(_schemaPath))
            {
                if (ShowConfirmSaveDialog() == false)
                {
                    return;
                }
            }

            if (_modifiedSchema.Save(Path.GetDirectoryName(_schemaPath)))
            {
                MessageBox.Show("Schema successfully saved.", "File saved");
            }

            // TODO: Update original schema?
        }

        // TODO: Cleanup
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            UpdateSchemaFromForm();
            // TODO: Save schema
            SaveSchema();
            // Generate html file

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
            SaveSchema();
        }

        // TODO: Really?
        private bool ShowConfirmSaveDialog()
        {
            DialogResult result = MessageBox.Show(
                "Schema file already exists.\nDo you want to replace it?",
                "File exists",
                MessageBoxButtons.YesNo);

            return result == DialogResult.Yes;
        }
    }
}
