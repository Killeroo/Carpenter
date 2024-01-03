using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    public enum SchemaOptions
    {
        CompressPreviewImage,
        CompressDetailedImage,
        OutputFilename,
    }

    public enum SchemaTokens
    {
        BaseUrl,
        PageUrl,
        Location,
        Title,
        Month,
        Year,
        Author,
        Camera,
        ClassIdImageGrid,
        ClassIdImageColumn,
        ClassIdImageElement,
        ClassIdImageTitle,
        Image,
        DetailedImage,
        ImageTitle
    };

    public enum SchemaImageTags
    {
        Grid,
        Standalone,
        Column,
        Title
    }

    public class Schema
    {
        public readonly Dictionary<string, SchemaTokens> TokenTable = new()
        {
            { "%BASE_URL", SchemaTokens.BaseUrl },
            { "%PAGE_URL", SchemaTokens.PageUrl },
            { "%LOCATION", SchemaTokens.Location },
            { "%TITLE", SchemaTokens.Title },
            { "%MONTH", SchemaTokens.Month },
            { "%YEAR", SchemaTokens.Year },
            { "%CAMERA", SchemaTokens.Camera },
            { "%AUTHOR", SchemaTokens.Author },
            { "image_grid", SchemaTokens.ClassIdImageGrid },
            { "image_column", SchemaTokens.ClassIdImageColumn },
            { "image_element", SchemaTokens.ClassIdImageElement },
            { "image_title",  SchemaTokens.ClassIdImageTitle }
        };

        public readonly Dictionary<string, SchemaTokens> ImageTokenTable = new()
        {
            { "%IMAGE_URL", SchemaTokens.Image },
            { "%DETAILED_IMAGE_URL", SchemaTokens.DetailedImage },
            { "%IMAGE_TITLE", SchemaTokens.ImageTitle },
        };

        private readonly Dictionary<string, SchemaOptions> _optionsTable = new()
        {
            { "compress_preview_image", SchemaOptions.CompressPreviewImage },
            { "compress_detailed_image", SchemaOptions.CompressDetailedImage },
            { "output_file", SchemaOptions.OutputFilename },
        };

        private readonly Dictionary<string, SchemaImageTags> _tagTable = new()
        {
            { "[IMAGE_LAYOUT]", SchemaImageTags.Grid },
            { "[IMAGES_STANDALONE]", SchemaImageTags.Standalone },
            { "[IMAGES_COLUMN]", SchemaImageTags.Column },
            { "[IMAGE_TITLE]", SchemaImageTags.Title },
        };

        public Dictionary<SchemaTokens, string> TokenValues = new();
        public Dictionary<SchemaOptions, string> OptionValues = new();
        public List<ImageSection> ImageSections = new();

        public bool Load(string path)
        {
            // Read the file
            string[] schemaFileContents;
            try
            {
                schemaFileContents = File.ReadAllLines(path);
            }
            catch (Exception e)
            {
                Logger.DebugError($"[{e.GetType()}] Could not read schema file - {e.Message}");
                return false;
            }

            // Parse tokens in file
            for (int i = 0; i < schemaFileContents.Length; i++)
            {
                string line = schemaFileContents[i];

                foreach (string token in TokenTable.Keys)
                {
                    if (line.Contains(token))
                    {
                        TokenValues[TokenTable[token]] = line.Split('=').Last();
                        Logger.DebugLog($"Schema token {TokenTable[token]}={TokenValues[TokenTable[token]]}");
                        break;
                    }
                }
            }

            // Parse options in file
            for (int i = 0; i < schemaFileContents.Length; i++)
            {
                string line = schemaFileContents[i];

                foreach (string option in _optionsTable.Keys)
                {
                    if (line.Contains(option))
                    {
                        OptionValues[_optionsTable[option]] = line.Split('=').Last();
                        Logger.DebugLog($"Schema option {_optionsTable[option]}={OptionValues[_optionsTable[option]]}");
                        break;
                    }
                }
            }

            // Parse photo grid layout
            string gridTag = string.Empty;
            foreach (var item in _tagTable)
            {
                if (item.Value == SchemaImageTags.Grid)
                {
                    gridTag = item.Key;
                }
            }
            Debug.Assert(gridTag != string.Empty);

            // First find where the photo section starts
            int photoSectionStartIndex = schemaFileContents.FindIndexWhichContainsValue(gridTag);
            if (photoSectionStartIndex < 0)
            {
                Logger.DebugError($"Could not find image layout section with tag \"{gridTag}\" while parsing schema. Add it to the schema and try again.");
                return false;
            }

            // Ok lets parse everything to the end of the file and construct our image grid
            int currentSectionIndex = -1;
            string imageUrl = string.Empty;
            string detailedImageUrl = string.Empty;
            string imageTitle = string.Empty;
            for (int i = photoSectionStartIndex; i < schemaFileContents.Length; i++)
            {
                string line = schemaFileContents[i];

                // If the line contains a tag
                foreach (string tag in _tagTable.Keys)
                {
                    if (line.Contains(tag))
                    {
                        switch (_tagTable[tag])
                        {
                            case SchemaImageTags.Standalone:
                                currentSectionIndex++;
                                ImageSections.Add(new StandaloneImageSection());
                                Logger.DebugLog("Adding standalone image section to photo grid");
                                break;

                            case SchemaImageTags.Column:
                                currentSectionIndex++;
                                ImageSections.Add(new ColumnImageSection());
                                Logger.DebugLog("Adding column section to photo grid");
                                break;

                            case SchemaImageTags.Title:
                                currentSectionIndex++;
                                ImageSections.Add(new TitleImageSection());
                                Logger.DebugLog("Adding title section to photo grid");
                                break;

                            case SchemaImageTags.Grid:
                                // Ignore this as this is probably the start of our photo grid
                                // TODO: We should worry if we encounter more than one of these
                                break;

                            default:
                                Logger.DebugError("Could not parse tag in photo grid");
                                return false;
                        }

                        break;
                    }
                }

                // If the line contains an image token
                foreach (string token in ImageTokenTable.Keys)
                {
                    if (line.Contains(token))
                    {
                        string tokenValue = line.Split('=').Last();

                        switch (ImageTokenTable[token])
                        {
                            case SchemaTokens.Image:
                                imageUrl = tokenValue;
                                break;

                            case SchemaTokens.DetailedImage:
                                detailedImageUrl = tokenValue;
                                break;

                            case SchemaTokens.ImageTitle:
                                imageTitle = tokenValue;
                                break;

                            default:
                                // TODO: Stronger identification for bad formatted tags
                                Logger.DebugError($"Could not parse token ({token}) in photo grid");
                                return false;
                        }

                        // TODO: Make this check more robust so we definitely have 2 sections before adding to section (Maybe parse ahead in array?)
                        // TODO: Don't assume this will be ordered image_url then detailed_image_url
                        if (imageTitle != string.Empty || (imageUrl != string.Empty && detailedImageUrl != string.Empty))
                        {
                            if (ImageSections[currentSectionIndex].GetType().Equals(typeof(StandaloneImageSection)))
                            {
                                // If we are dealing with a standalone section
                                var standaloneSection = ImageSections[currentSectionIndex] as StandaloneImageSection;
                                standaloneSection.PreviewImage = imageUrl;
                                standaloneSection.DetailedImage = detailedImageUrl;

                                Logger.DebugLog($"Added single image section to photo grid (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }
                            else if (ImageSections[currentSectionIndex].GetType().Equals(typeof(ColumnImageSection)))
                            {
                                // If we are dealing with a column section
                                var columnSection = ImageSections[currentSectionIndex] as ColumnImageSection;
                                columnSection.Sections.Add(new StandaloneImageSection
                                {
                                    PreviewImage = imageUrl,
                                    DetailedImage = detailedImageUrl,
                                });

                                Logger.DebugLog($"Added image to column section (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }
                            else if (ImageSections[currentSectionIndex].GetType().Equals(typeof(TitleImageSection)))
                            {
                                var titleSection = ImageSections[currentSectionIndex] as TitleImageSection;
                                titleSection.TitleText = imageTitle;

                                Logger.DebugLog($"Added image title section to photo grid (titile={imageTitle})");
                            }

                            // Blank the urls again
                            imageUrl = string.Empty;
                            detailedImageUrl = string.Empty;
                            imageTitle = string.Empty;
                        }
                    }
                }
            }

            Logger.DebugLog($"Schema file parsed (\"{path}\")");
            return true;
        }
    }
}
