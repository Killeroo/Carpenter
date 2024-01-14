using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Reflection.PortableExecutable;

namespace Carpenter
{
    public class Schema : IDisposable
    {
        public const float kSchemaVersion = 2.0f;

        public enum Option
        {
            OutputFilename
        }

        public enum Token
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

        public enum ImageTag
        {
            Grid,
            Standalone,
            Column,
            Title
        }

        public readonly Dictionary<string, Token> TokenTable = new()
        {
            { "%BASE_URL", Token.BaseUrl },
            { "%PAGE_URL", Token.PageUrl },
            { "%LOCATION", Token.Location },
            { "%TITLE", Token.Title },
            { "%MONTH", Token.Month },
            { "%YEAR", Token.Year },
            { "%CAMERA", Token.Camera },
            { "%AUTHOR", Token.Author },
            { "image_grid", Token.ClassIdImageGrid },
            { "image_column", Token.ClassIdImageColumn },
            { "image_element", Token.ClassIdImageElement },
            { "image_title",  Token.ClassIdImageTitle }
        };

        public readonly Dictionary<string, Token> ImageTokenTable = new()
        {
            { "%IMAGE_URL", Token.Image },
            { "%DETAILED_IMAGE_URL", Token.DetailedImage },
            { "%IMAGE_TITLE", Token.ImageTitle },
        };

        private readonly Dictionary<string, Option> _optionsTable = new()
        {
            { "output_file", Option.OutputFilename },
        };

        private readonly Dictionary<string, ImageTag> _imageTagTable = new()
        {
            { "[IMAGE_LAYOUT]", ImageTag.Grid },
            { "[IMAGES_STANDALONE]", ImageTag.Standalone },
            { "[IMAGES_COLUMN]", ImageTag.Column },
            { "[IMAGE_TITLE]", ImageTag.Title },
        };

        public Dictionary<Token, string> TokenValues = new();
        public Dictionary<Option, string> OptionValues = new();
        public List<ImageSection> ImageSections = new();

        private const string kVersionToken = "schema_version";

        public Schema() { }
        public Schema(string path) => Load(path);

        // TODO: Throw exception
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

            // Sanity check size
            if (schemaFileContents.Length == 0)
            {
                Logger.DebugError($"Schema file empty!");
                return false;
            }

            // Version check
            if (schemaFileContents[0].Contains(kVersionToken) == false || schemaFileContents[0].Contains('=') == false)
            {
                Logger.DebugError($"Could not read schema file version");
                return false;
            }
            string versionValue = GetTokenValue(schemaFileContents[0]);
            float version = 0.0f;
            try
            {
                version = float.Parse(versionValue);
            }
            catch (Exception e) 
            {
                Logger.DebugError($"[{e.GetType()}] Could not parse schema version (\'{versionValue}\') - {e.Message}");
                return false;
            }
            if (version != kSchemaVersion)
            {
                Logger.DebugError($"Incompabitable schema version, found v{version} but can only parse v{kSchemaVersion}. Please update.");
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
                        TokenValues[TokenTable[token]] = GetTokenValue(line);
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
                        OptionValues[_optionsTable[option]] = GetTokenValue(line);
                        Logger.DebugLog($"Schema option {_optionsTable[option]}={OptionValues[_optionsTable[option]]}");
                        break;
                    }
                }
            }

            // Parse photo grid layout
            string gridTag = string.Empty;
            foreach (var item in _imageTagTable)
            {
                if (item.Value == ImageTag.Grid)
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
                foreach (string tag in _imageTagTable.Keys)
                {
                    if (line.Contains(tag))
                    {
                        switch (_imageTagTable[tag])
                        {
                            case ImageTag.Standalone:
                                currentSectionIndex++;
                                ImageSections.Add(new StandaloneImageSection());
                                Logger.DebugLog("Adding standalone image section to photo grid");
                                break;

                            case ImageTag.Column:
                                currentSectionIndex++;
                                ImageSections.Add(new ColumnImageSection());
                                Logger.DebugLog("Adding column section to photo grid");
                                break;

                            case ImageTag.Title:
                                currentSectionIndex++;
                                ImageSections.Add(new TitleImageSection());
                                Logger.DebugLog("Adding title section to photo grid");
                                break;

                            case ImageTag.Grid:
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
                            case Token.Image:
                                imageUrl = tokenValue;
                                break;

                            case Token.DetailedImage:
                                detailedImageUrl = tokenValue;
                                break;

                            case Token.ImageTitle:
                                imageTitle = tokenValue;
                                break;

                            default:
                                // TODO: Stronger identification for bad formatted tags
                                Logger.DebugError($"Could not parse token ({token}) in photo grid");
                                return false;
                        }

                        // TODO: Make this check more robust so we definitely have 2 sections before adding to section (Maybe parse ahead in array?)
                        // TODO: Don't assume this will be ordered image_url then detailed_image_url
                        if (imageTitle != string.Empty || imageUrl != string.Empty && detailedImageUrl != string.Empty)
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

        // Shorthand for parsing a token value from a token
        private string GetTokenValue(string token)
        {
            return token.Split('=').Last().Split("``").First().StripWhitespaces();
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                TokenValues.Clear();
                OptionValues.Clear();
                ImageSections.Clear();

                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
