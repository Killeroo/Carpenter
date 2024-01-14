using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Encoder = System.Drawing.Imaging.Encoder;
using InterpolationMode = System.Drawing.Drawing2D.InterpolationMode;
using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace Carpenter
{
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

    public static class ImageUtils
    {
        /// <summary>
        /// Compresses an image with the specified quality (resizes image if scalePercent is specified)
        /// </summary>
        /// <param name="filename">The path to the image that will be compressed</param>
        /// <param name="quality">The quality of the new image, between 0 - 100% of the original image</param>
        /// <param name="scalePercent">The percentage to scale down the image size</param>
        /// <returns></returns>
        /// Based off: https://stackoverflow.com/a/24651073
        public static Image CompressImage(string filename, int quality, float scalePercent = 1.0f)
        {
            Logger.DebugLog($"Compressing image {Path.GetFileName(filename)} with {quality}% quality and scaled by {scalePercent * 100}%");
            using (Image image = Image.FromFile(filename))
            {
                float newWidth = image.Width * scalePercent;
                float newHeight = image.Height * scalePercent;

                using (Image originalImageData = new Bitmap(image, (int)newWidth, (int)newHeight))
                {
                    // Setup the new image properties and set the quality encoder 
                    // (we could set other properties here)
                    ImageCodecInfo imageCodecInfo = GetEncoderInfo("image/jpeg");
                    Encoder qualityEncoder = Encoder.Quality;
                    EncoderParameter newImageQualityParameter = new EncoderParameter(qualityEncoder, quality);
                    EncoderParameters newImageParameters = new EncoderParameters(1);
                    newImageParameters.Param[0] = newImageQualityParameter;

                    // Time to construct the new image
                    using (MemoryStream newImageData = new MemoryStream())
                    {
                        originalImageData.Save(newImageData, imageCodecInfo, newImageParameters);
                        Image newImage = Image.FromStream(newImageData);
                        ImageAttributes newImageAttributes = new ImageAttributes();
                        using (Graphics g = Graphics.FromImage(newImage))
                        {
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                            // Doesn't seem to make much difference
                            g.CompositingMode = CompositingMode.SourceCopy;
                            g.CompositingQuality = CompositingQuality.HighQuality;
                            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            g.SmoothingMode = SmoothingMode.HighQuality;
                            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                            g.DrawImage(
                                newImage,
                                new Rectangle(Point.Empty, newImage.Size),
                                0,
                                0,
                                newImage.Width,
                                newImage.Height,
                                GraphicsUnit.Pixel,
                                newImageAttributes);
                        }

                        return newImage;
                    }
                }
            }
        }

        private static ImageCodecInfo? GetEncoderInfo(string mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }

        public static AspectRatio CalculateAspectRatio(Image image)
        {
            int lowestCommonDemoninator = MathUtils.LowestCommonMultiple(image.Width, image.Height);

            //WidthRatio = lowestCommonDemoninator / image.Width;
            //HeightRatio = lowestCommonDemoninator / image.Height;

            return new AspectRatio(lowestCommonDemoninator / image.Width, lowestCommonDemoninator / image.Height);
        }

        // TODO: Cache (Maybe just a general caching method
        //https://stackoverflow.com/a/24199315
        public static Bitmap ResizeImage(Image sourceImage, int width, int height)
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
    }
}
