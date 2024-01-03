using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PageDesigner
{

    struct AspectRatio
    {
        public readonly int Width;
        public readonly int Height;

        public AspectRatio(int width, int height)
        {
            // TODO: Where is this reversed?????
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

    public partial class PageDesignerForm : Form
    {
        public PageDesignerForm()
        {
            InitializeComponent();

        }


        PreviewImageBox selectedPreviewImageControl; // TODO: Rename _selectedPreviewImageControl
        Control _lastControlInGrid;
        Dictionary<string, Image> _previewImages = new();
        string workingPath;
        bool fullSize = false;

        private void button1_Click(object sender, EventArgs e)
        {
            //pictureBox1.Image = new Bitmap(@"G:\#bearphoto winners\DSC00828.jpg");
            //using (Image originalImage = Image.FromFile(@"G:\#bearphoto winners\DSC00828.jpg"))
            //{
            //    GraphicsUnit unit = GraphicsUnit.Pixel;
            //    RectangleF imageBounds = originalImage.GetBounds(ref unit);


            //    int lowestCommonDemoninator = lcm((int)imageBounds.Size.Width, (int)imageBounds.Size.Height);

            //    textBox1.Text = string.Format("{0}:{1}",
            //        lowestCommonDemoninator / (int)imageBounds.Size.Width,
            //        lowestCommonDemoninator / (int)imageBounds.Size.Height);

            //    pictureBox1.Image = originalImage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            //}
            //Image image = Image.FromFile(@"G:\#bearphoto winners\DSC00828.jpg");
            //pictureBox1.Image = image.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);
            //image.Dispose();

            string inputtedPath = PathTextBox.Text;

            if (string.IsNullOrEmpty(inputtedPath) || Directory.Exists(inputtedPath) == false)
            {
                return;
            }

            string[] imageFilesAtPath = Directory.GetFiles(inputtedPath, "*.jpg");
            if (imageFilesAtPath.Length == 0)
            {
                return;
            }

            workingPath = inputtedPath;

            // TODO: Thread
            foreach (string imagePath in imageFilesAtPath)
            {
                using (Image originalImage = Image.FromFile(imagePath))
                {
                    // TODO: Use DrawImage to properly resize
                    Image previewImage = originalImage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero);

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


            //ImagePreviewFlowLayoutPanel.VerticalScroll.Visible = true;
        }

        // Turns out GetThumbnail does caching anyway so this is basically useless
        private Image FetchOrCatchPreviewImage(string filename, string path)
        {
            string rootTempPath = Path.Combine(Path.GetTempPath(), @"Carpenter", Path.GetDirectoryName(path));
            string cachedFilePath = Path.Combine(rootTempPath, Path.ChangeExtension(filename, "png"));

            if (File.Exists(cachedFilePath))
            {
                return Image.FromFile(cachedFilePath);
            }

            using (Image originalImage = Image.FromFile(Path.Combine(path, filename)))
            {
                Image thumbnail = originalImage.GetThumbnailImage(120, 120, () => false, IntPtr.Zero); // TODO: Fix with aspect ratio
                thumbnail.Save(cachedFilePath);
                return thumbnail;
            }
        }

        private void ImagePreviewFlowLayoutPanel_ControlClicked(object? sender, PreviewImageEventArgs e)
        {
            textBox1.Text = e.ImageName;
            string imageName = e.ImageName;

            PreviewImageBox? previewImageControl = sender as PreviewImageBox;
            if (previewImageControl != null)
            {
                if (selectedPreviewImageControl != null)
                {
                    selectedPreviewImageControl.BackColor = BackColor;
                }

                previewImageControl.BackColor = Color.Red;

                selectedPreviewImageControl = previewImageControl;


                // Find the image locally and save a resized copy
                string localImagePath = Path.Combine(workingPath, imageName);
                if (File.Exists(localImagePath) == false)
                {
                    return;
                }

                using (Image sourceImage = Image.FromFile(localImagePath))
                {

                    AspectRatio ar = CalculateAspectRatio(sourceImage);
                    textBox1.Text = ar.ToString();

                    // Find the width that we need to fit into
                    int targetWidth = GridFlowLayoutPanel.Width;
                    if (fullSize == false)
                    {
                        targetWidth /= 2;
                    }
                    targetWidth -= 10;
                    int targetHeight = ar.CalculateHeight(targetWidth);


                    Image resizedImage = ResizeImage(sourceImage, targetWidth, targetHeight);

                    PictureBox pictureBox = new();
                    pictureBox.Image = resizedImage;
                    pictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
                    GridFlowLayoutPanel.Controls.Add(pictureBox);

                    _lastControlInGrid = pictureBox;
                }


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

        private AspectRatio CalculateAspectRatio(Image image)
        {
            int lowestCommonDemoninator = LowestCommonMultiple(image.Width, image.Height);

            //WidthRatio = lowestCommonDemoninator / image.Width;
            //HeightRatio = lowestCommonDemoninator / image.Height;

            return new AspectRatio(lowestCommonDemoninator / image.Width, lowestCommonDemoninator / image.Height);
        }

        //https://stackoverflow.com/a/24199315
        private Bitmap ResizeImage(Image sourceImage, int width, int height)
        {
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

            return destImage;
        }

        //https://stackoverflow.com/a/20824923
        static int GreatestCommonFactor(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        static int LowestCommonMultiple(int a, int b)
        {
            return (a / GreatestCommonFactor(a, b)) * b;
        }

        private void ImagePreviewFlowLayoutPanel_Click(object sender, EventArgs e)
        {
            if (selectedPreviewImageControl != null)
            {
                selectedPreviewImageControl.BackColor = BackColor;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (_lastControlInGrid != null)
            {
                //GridFlowLayoutPanel.SetFlowBreak(_lastControlInGrid, true);
                fullSize = !fullSize;


            }
        }
    }
}
