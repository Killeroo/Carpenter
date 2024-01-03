// TODO: Add link previews
// https://andrejgajdos.com/how-to-create-a-link-preview/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;

using Carpenter;

namespace Carpenter.CommandLine
{
    class Program
    {
        private const string VersionString = "v1.0";

        private const string TemplateFilename = "template.html";
        private const string SchemaFilename = "SCHEMA";

        private const string CompressPreviewImagePostfix = "_Preview";
        private const string CompressDetailedImagePostfix = "_Detailed";

        private const int CompressedPreviewImageQuality = 100;
        private const int CompressedDetailedImageQuality = 100;
        private const float CompressedPreviewImageScale = 0.5f;
        private const float CompressedDetailedImageScale = 1.0f;

        // TODO: Possible arguments:
        // --schema = Specify one schema to process
        // --template = Specify a template file
        // --force-compress = force compress files even if they exist

        static void Main(string[] args)
        {
            string rootDirectory = string.Empty;
            if (args.Length != 0)
            {
                rootDirectory = args[0];
            }
            else
            {
                rootDirectory = Environment.CurrentDirectory;
            }

            Console.WriteLine($"Carpenter {VersionString} - Static photo webpage generator");

            // Ok first thing's first we need to find the template file in the root
            string pathToTemplateFile = Path.Combine(rootDirectory, "template.html");
            if (!File.Exists(pathToTemplateFile))
            {
                Logger.DebugError($"Could not find template file ({TemplateFilename}) at path {rootDirectory}. " +
                    $"Please place template at this path and try again.");
                return;
            }

            // Load the template
            Template template = new Template();
            template.Load(pathToTemplateFile);

            // Now loop through every folder and generate a webpage from the SCHEMA file present in the directory
            int count = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string directory in Directory.GetDirectories(rootDirectory))
            {
                string currentSchemaPath = Path.Combine(directory, SchemaFilename);
                if (!File.Exists(currentSchemaPath))
                {
                    Logger.DebugError($"Could not find ({SchemaFilename}) at {directory}, skipping..");
                    continue;
                }
                else
                {
                    Logger.DebugLog($"Generating page for directory" + Path.GetDirectoryName(directory));
                }

                // Load the schema file
                string pathToSchemaFile = Path.Combine(directory, "SCHEMA");
                Schema schema = new Schema();
                if (!schema.Load(pathToSchemaFile))
                {
                    Logger.DebugError($"Encountered an error parsing schema, skipping..");
                    continue;
                }

                // Compress any images (if specified in schema)
                if (schema.OptionValues[Schema.Option.CompressPreviewImage] == "true"
                    || schema.OptionValues[Schema.Option.CompressDetailedImage] == "true")
                {
                    CompressImages(schema, directory);
                }

                // Finally generate the webpage
                template.Generate(schema, directory);

                count++;
            }

            stopwatch.Stop();
            Logger.DebugLog($"Website generation completed. {count} pages created in {stopwatch.ElapsedMilliseconds}ms.");
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
                        Logger.DebugLog($"Found what looks like an already compressed image ({originalImageName}), skipping...");

                        // Add the original filename (without the postfix) to the image file names to patch
                        newPreviewImageNames.Add(previewImageName, originalImageName);
                    }
                    else
                    {
                        // Generate a compressed preview image
                        previewImageName = originalImageNameWithoutExtension + CompressPreviewImagePostfix + ".jpg";
                        Image previewImage = ImageProcessing.CompressImage(image, CompressedPreviewImageQuality, CompressedPreviewImageScale);
                        previewImage.Save(Path.Combine(currentPath, previewImageName));
                        Logger.DebugLog($"Saved preview image @ {previewImageName}");

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
                        Logger.DebugLog($"Found what looks like an already compressed image ({originalImageName}), skipping...");

                        newDetailedImageNames.Add(detailedImageName, originalImageName);
                    }
                    else
                    {
                        detailedImageName = originalImageNameWithoutExtension + CompressDetailedImagePostfix + ".jpg";
                        Image detailedImage = ImageProcessing.CompressImage(image, CompressedDetailedImageQuality, CompressedDetailedImageScale);
                        detailedImage.Save(Path.Combine(currentPath, detailedImageName));
                        Logger.DebugLog($"Saved detailed image @ {detailedImageName}");

                        newDetailedImageNames.Add(originalImageName, detailedImageName);
                    }

                }
            }

            // Now go through the schema and replace the old filenames with the new ones
            foreach (ImageSection section in schema.ImageSections)
            {
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
            Logger.DebugLog($"Patched schema's sections with new compressed images");
        }


    }
}


