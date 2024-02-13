using System;
using System.IO;
using System.Collections.Generic;

using Carpenter;

namespace ImageCompressor
{
    internal class Program
    {
        private const string CompressPreviewImagePostfix = "_Preview";
        private const string CompressDetailedImagePostfix = "_Detailed";

        private const int CompressedPreviewImageQuality = 100;
        private const int CompressedDetailedImageQuality = 100;
        private const float CompressedPreviewImageScale = 0.5f;
        private const float CompressedDetailedImageScale = 1.0f;

        static void Main(string[] args)
        {
            Console.WriteLine("Hello, World!");
        }

        private static void CompressImages(Schema schema, string currentPath)
        {
            Dictionary<string, string> newDetailedImageNames = new();
            Dictionary<string, string> newPreviewImageNames = new();

            // Go through each image in the directory, compressing them as appropriate
            // (track each file we compress)
            // TODO: Multithread 
            // TODO: Preseve rotation
            foreach (string image in Directory.GetFiles(currentPath, "*.jpg"))
            {
                string originalImageName = Path.GetFileName(image);
                string originalImageNameWithoutExtension = Path.GetFileNameWithoutExtension(image);

                //// Skip files that have already been compressed
                //if (originalImageNameWithoutExtension.Contains(CompressPreviewImagePostfix) || originalImageNameWithoutExtension.Contains(CompressDetailedImagePostfix))
                //{
                //    continue;
                //}

                if (schema.OptionValues[Schema.Option.CompressPreviewImage] == "true"
                    && !originalImageNameWithoutExtension.Contains(CompressDetailedImagePostfix)) // HACK: To make sure we don't generate detailed images for previews when they already exist etc..
                {
                    // TODO: Allow force compressing new images
                    string previewImageName = String.Empty;
                    if (originalImageNameWithoutExtension.Contains(CompressPreviewImagePostfix))
                    {
                        previewImageName = originalImageName.Replace(CompressPreviewImagePostfix, "");
                        Logger.Info($"Found what looks like an already compressed image ({originalImageName}), skipping...");

                        // Add the original filename (without the postfix) to the image file names to patch
                        newPreviewImageNames.Add(previewImageName, originalImageName);
                    }
                    else
                    {
                        // Generate a compressed preview image
                        previewImageName = originalImageNameWithoutExtension + CompressPreviewImagePostfix + ".jpg";
                        Image previewImage = ImageUtils.CompressImage(image, CompressedPreviewImageQuality, CompressedPreviewImageScale);
                        previewImage.Save(Path.Combine(currentPath, previewImageName));
                        Logger.Info($"Saved preview image @ {previewImageName}");

                        // Track it in the new file so we can replace it later
                        newPreviewImageNames.Add(originalImageName, previewImageName);
                    }
                }

                if (schema.OptionValues[Schema.Option.CompressDetailedImage] == "true"
                    && !originalImageNameWithoutExtension.Contains(CompressPreviewImagePostfix))
                {
                    string detailedImageName = String.Empty;
                    if (originalImageNameWithoutExtension.Contains(CompressDetailedImagePostfix))
                    {
                        detailedImageName = originalImageName.Replace(CompressDetailedImagePostfix, "");
                        Logger.Info($"Found what looks like an already compressed image ({originalImageName}), skipping...");

                        newDetailedImageNames.Add(detailedImageName, originalImageName);
                    }
                    else
                    {
                        detailedImageName = originalImageNameWithoutExtension + CompressDetailedImagePostfix + ".jpg";
                        Image detailedImage = ImageUtils.CompressImage(image, CompressedDetailedImageQuality, CompressedDetailedImageScale);
                        detailedImage.Save(Path.Combine(currentPath, detailedImageName));
                        Logger.Info($"Saved detailed image @ {detailedImageName}");

                        newDetailedImageNames.Add(originalImageName, detailedImageName);
                    }

                }
            }

            // Now go through the schema and replace the old filenames with the new ones
            foreach (ImageSection section in schema.ImageSections)
            {
                // Avoid setting data in title images (they don't contain any images)
                if (section.GetType() == typeof(TitleImageSection))
                {
                    continue;
                }

                if (schema.OptionValues[Schema.Option.CompressPreviewImage] == "true")
                {
                    foreach (var newImageName in newPreviewImageNames)
                    {
                        section.ReplacePreviewImage(newImageName.Key, newImageName.Value);
                    }
                }

                if (schema.OptionValues[Schema.Option.CompressDetailedImage] == "true")
                {
                    foreach (var newImageName in newDetailedImageNames)
                    {
                        section.ReplaceDetailedImage(newImageName.Key, newImageName.Value);
                    }
                }
            }
            Logger.Info($"Patched schema's sections with new compressed images");
        }
    }
}