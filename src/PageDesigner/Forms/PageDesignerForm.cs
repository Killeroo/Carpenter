using Carpenter;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
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
        Dictionary<string, Image> _previewImages = new();
        Queue<PictureBox> _pictureBoxBuffer = new();

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
            foreach (ImageSection section in _schema.ImageSections)
            {
                Type sectionType = section.GetType();
                if (sectionType == typeof(StandaloneImageSection)) 
                {
                    StandaloneImageSection? standaloneSection = section as StandaloneImageSection;   
                    if (standaloneSection != null)
                    {
                        string fileName = standaloneSection.PreviewImage;

                        AddLocalImageToGridLayout(fileName, true);
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
                            PictureBox pictureBox = null;
                            if (_pictureBoxBuffer.Count > 0)
                            {
                                pictureBox = _pictureBoxBuffer.Dequeue();
                            }

                            AddLocalImageToGridLayout(standaloneImage.PreviewImage, false, pictureBox);
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
                                string fileName = standaloneImage.PreviewImage;
                                AddLocalImageToGridLayout(fileName, false);

                                // Add the picturebox for the other column items
                                PictureBox pictureBox = new();
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
        private void AddLocalImageToGridLayout(string imageName, bool fullSize, PictureBox existingPictureBox = null)
        {
            // Find the image locally and save a resized copy
            string localImagePath = Path.Combine(_workingPath, imageName);
            if (File.Exists(localImagePath) == false)
            {
                return;
            }

            // TODO: Work out size without loading the whole image into memory
            using (Image sourceImage = Image.FromFile(localImagePath))
            {

                AspectRatio ar = ImageUtils.CalculateAspectRatio(sourceImage);
                textBox1.Text = ar.ToString();

                // Find the width that we need to fit into
                int targetWidth = GridFlowLayoutPanel.Width;
                if (fullSize == false)
                {
                    targetWidth /= 2;
                }
                targetWidth -= 20;// 10;
                int targetHeight = ar.CalculateHeight(targetWidth);

                Image resizedImage = ImageUtils.ResizeImage(sourceImage, targetWidth, targetHeight);

                if (existingPictureBox == null)
                {
                    PictureBox pictureBox = new()
                    {
                        Image = resizedImage,
                        SizeMode = PictureBoxSizeMode.AutoSize
                    };
                    GridFlowLayoutPanel.Controls.Add(pictureBox);
                }
                else
                {
                    existingPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    existingPictureBox.Image = resizedImage;
                }

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
            // TODO: Move to model

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

            PreviewImageBox? previewImageControl = sender as PreviewImageBox;
            if (previewImageControl != null)
            {
                if (_selectedPreviewImageControl != null)
                {
                    _selectedPreviewImageControl.BackColor = BackColor;
                }

                previewImageControl.BackColor = Color.Red;

                _selectedPreviewImageControl = previewImageControl;


                AddLocalImageToGridLayout(imageName, false);


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

        private void button2_Click(object sender, EventArgs e)
        {
            //if (_lastControlInGrid != null)
            {
                //GridFlowLayoutPanel.SetFlowBreak(_lastControlInGrid, true);
                //fullSize = !fullSize;


            }
        }

        private void button3_Click(object sender, EventArgs e)
        {

        }
    }
}
