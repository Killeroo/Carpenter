using Carpenter;
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
        private Schema _schema;

        PreviewImageBox _selectedPreviewImageControl; // TODO: Rename _selectedPreviewImageControl
        GridPictureBox _selectedGridImage;
        Dictionary<string, Image> _previewImages = new();
        Queue<GridPictureBox> _pictureBoxBuffer = new();

        public PageDesignerForm(string path)
        {
            InitializeComponent();

            _workingPath = path;

            this.Text = string.Format("{0} - {1}", "Carpenter", _workingPath);
        }

        private void PageDesignerForm_Load(object sender, EventArgs e)
        {
            if (LoadSchemaIfAvailable())
            {
                // TODO: Populate default values
            }

            LoadAvailableImagePreviews();

        }

        private bool LoadSchemaIfAvailable()
        {
            string schemaPath = Path.Combine(_workingPath, "SCHEMA");
            if (File.Exists(schemaPath) == false)
            {
                return false;
            }

            _schema = new Schema();

            if (_schema.Load(schemaPath) == false)
            {
                // TODO: Raise an error or something
                return false;
            }

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
            foreach (ImageSection section in _schema.ImageSections)
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
                    ColumnImageSection columnSection = (ColumnImageSection) section;
                    if (columnSection == null)
                    {
                        // TODO: Error and stuff
                        continue;
                    }

                    if (_pictureBoxBuffer.Count > 0)
                    {
                        //if (_pictureBoxBuffer.Count < )
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

        // TODO: Cache and retrieve (do in another thread
        private GridPictureBox? AddLocalImageToGridLayout(string previewImageName, string detailedPictureName, bool standaloneImage, GridPictureBox existingPictureBox = null)
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
                AspectRatio ar = ImageUtils.CalculateAspectRatio(sourceImage);
                textBox1.Text = ar.ToString();

                // Find the width that we need to fit into
                int targetWidth = GridFlowLayoutPanel.Width;
                if (standaloneImage == false)
                {
                    targetWidth /= 2;
                }
                
                // There is a bug where, when we add an image to a picturebox from the buffer there is some deadspace
                // causing an overflow, meaning we have to reduce this width for now
                targetWidth -= 20;// 10; 
                int targetHeight = ar.CalculateHeight(targetWidth);

                Image resizedImage = ImageUtils.ResizeImage(sourceImage, targetWidth, targetHeight);

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
                gridPictureBox.Standalone = standaloneImage;
                gridPictureBox.DetailedImageName = detailedPictureName;
                gridPictureBox.PreviewImageName = previewImageName;

                // Add callbacks
                gridPictureBox.Click += GridPictureBox_Click;

                return gridPictureBox;
            }
        }

        private void GridPictureBox_Click(object? sender, EventArgs e)
        {
            if (sender is GridPictureBox gridPictureBox)
            {
                // Update currently selected image
                if (_selectedGridImage != null)
                {
                    _selectedGridImage.BackColor = Color.LightGray;
                    _selectedGridImage.BorderStyle = BorderStyle.None;
                }
                _selectedGridImage = gridPictureBox;

                // Retrieve details from selected image
                PreviewImageTextBox.Text = gridPictureBox.PreviewImageName;
                DetailedImageTextBox.Text = gridPictureBox.DetailedImageName;

                // Highlight image
                gridPictureBox.BorderStyle = BorderStyle.FixedSingle;
                gridPictureBox.BackColor = Color.Blue;
            }
        }

        private string GetTokenFromSchema(Schema.Token token, string defaultValue)
        {
            if (_schema == null || _schema.TokenValues.ContainsKey(token) == false)
            {
                return defaultValue;
            }

            return _schema.TokenValues[token];
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
                    AspectRatio ar = ImageUtils.CalculateAspectRatio(originalImage);

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
                    previewImageBox.ControlClicked += ImagePreviewFlowLayoutPanel_ControlClicked;

                    ImagePreviewFlowLayoutPanel.Controls.Add(previewImageBox);
                }
            }
        }

        private void ImagePreviewFlowLayoutPanel_ControlClicked(object? sender, PreviewImageEventArgs e)
        {
            textBox1.Text = e.ImageName;
            string imageName = e.ImageName;

            if (sender is PreviewImageBox previewImageControl)
            {
                if (_selectedPreviewImageControl != null)
                {
                    _selectedPreviewImageControl.BackColor = BackColor;
                }

                previewImageControl.BackColor = Color.Red;

                _selectedPreviewImageControl = previewImageControl;


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

        private void ImagePreviewFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            if (_selectedPreviewImageControl != null)
            {
                _selectedPreviewImageControl.BackColor = BackColor;
            }
        }

        private void PreviewButton_Click(object sender, EventArgs e)
        {
            if (_schema == null)
            {
                return;
            }

            
        }

        private void GenerateButton_Click(object sender, EventArgs e)
        {
            if (_schema == null)
            {
                return;
            }

            // Update tokens values from page
            // TODO: Copy or add class id and option base values
            _schema.TokenValues.AddOrUpdate(Schema.Token.BaseUrl, BaseUrlTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.PageUrl, PageUrlTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.Title, TitleTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.Location, LocationTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.Month, MonthTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.Year, YearTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.Author, AuthorTextBox.Text);
            _schema.TokenValues.AddOrUpdate(Schema.Token.Camera, CameraTextBox.Text);

            // Parse grid layout
            List<ImageSection> sections = new ();
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

                    if (gridPictureBox.Standalone)
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

            _schema.ImageSections = sections;

            _schema.Save(Path.Combine(_workingPath, "test"));
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
    }
}
