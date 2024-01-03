using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;

using Encoder = System.Drawing.Imaging.Encoder;
using InterpolationMode = System.Drawing.Drawing2D.InterpolationMode;

namespace Carpenter
{
    public class ImageProcessing
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
                            //g.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceCopy;
                            //g.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                            //g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                            //g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                            //g.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

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

        private static ImageCodecInfo? GetEncoderInfo(String mimeType)
        {
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            foreach (ImageCodecInfo ici in encoders)
                if (ici.MimeType == mimeType) return ici;

            return null;
        }
    }
}
