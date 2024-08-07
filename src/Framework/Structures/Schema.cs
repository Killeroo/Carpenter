﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace Carpenter
{
    // TODO: Struct
    public class Schema : IDisposable, IEquatable<Schema>
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

        // Some cosmetic tags used in the schema file 
        private const string kTokensTag = "[TAGS]";
        private const string kClassIdentifierTag = "[CLASS_IDENTIFIERS]";
        private const string kOptionsTag = "[OPTIONS]";

        public Schema() { }
        public Schema(string path) => Load(path);
        public Schema(Schema otherSchema)
        {
            if (otherSchema == null)
            {
                return;
            }

            TokenValues = new Dictionary<Token, string>(otherSchema.TokenValues);
            OptionValues = new Dictionary<Option, string>(otherSchema.OptionValues);
            ImageSections = new List<ImageSection>();
            
            // Need to actually populate the ImageSections manually to avoid copying a reference
            foreach (ImageSection section in otherSchema.ImageSections)
            {
                if (section is StandaloneImageSection standaloneSection)
                {
                    StandaloneImageSection newSection = new StandaloneImageSection();
                    newSection.DetailedImage = standaloneSection.DetailedImage;
                    newSection.PreviewImage = standaloneSection.PreviewImage;
                    ImageSections.Add(newSection);
                }
                else if (section is ColumnImageSection columnSection)
                {
                    ColumnImageSection newColumnSection = new ColumnImageSection();
                    newColumnSection.Sections = new List<StandaloneImageSection>();
                    foreach (ImageSection innerSection in columnSection.Sections)
                    {
                        if (innerSection is StandaloneImageSection innerStandaloneSection)
                        {
                            StandaloneImageSection newSection = new StandaloneImageSection();
                            newSection.DetailedImage = innerStandaloneSection.DetailedImage;
                            newSection.PreviewImage = innerStandaloneSection.PreviewImage;
                            newColumnSection.Sections.Add(newSection);
                        }
                    }
                    ImageSections.Add(newColumnSection);
                }
                else if (section is TitleImageSection titleSection)
                {
                    TitleImageSection newSection = new TitleImageSection();
                    newSection.TitleText = titleSection.TitleText;
                    ImageSections.Add(newSection);
                }
            }
        }

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
                Logger.Log(LogLevel.Error, $"Could not read schema file ({e.GetType()} exception occured)");
                return false;
            }

            // Sanity check size
            if (schemaFileContents.Length == 0)
            {
                Logger.Log(LogLevel.Error, $"Schema file empty!");
                return false;
            }

            // Version check
            if (schemaFileContents[0].Contains(kVersionToken) == false || schemaFileContents[0].Contains('=') == false)
            {
                Logger.Log(LogLevel.Error, $"Could not read schema file version");
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
                Logger.Log(LogLevel.Error, $"[] Could not parse schema version string (\'{versionValue}\') ({e.GetType()} exception occured)");
                return false;
            }
            if (version != kSchemaVersion)
            {
                Logger.Log(LogLevel.Error, $"Incompabitable schema version, found v{version} but can only parse v{kSchemaVersion}. Please update.");
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
                        Logger.Log(LogLevel.Verbose, $"Schema token {TokenTable[token]}={TokenValues[TokenTable[token]]}");
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
                        Logger.Log(LogLevel.Verbose, $"Schema option {_optionsTable[option]}={OptionValues[_optionsTable[option]]}");
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
                Logger.Log(LogLevel.Error, $"Could not find image layout section with tag \"{gridTag}\" while parsing schema. Add it to the schema and try again.");
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
                                Logger.Log(LogLevel.Verbose, "Adding standalone image section to photo grid");
                                break;

                            case ImageTag.Column:
                                currentSectionIndex++;
                                ImageSections.Add(new ColumnImageSection());
                                Logger.Log(LogLevel.Verbose, "Adding column section to photo grid");
                                break;

                            case ImageTag.Title:
                                currentSectionIndex++;
                                ImageSections.Add(new TitleImageSection());
                                Logger.Log(LogLevel.Verbose, "Adding title section to photo grid");
                                break;

                            case ImageTag.Grid:
                                // Ignore this as this is probably the start of our photo grid
                                // TODO: We should worry if we encounter more than one of these
                                break;

                            default:
                                Logger.Log(LogLevel.Error, "Could not parse tag in photo grid");
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
                                Logger.Log(LogLevel.Error, $"Could not parse token ({token}) in photo grid");
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

                                Logger.Log(LogLevel.Verbose, $"Added single image section to photo grid (image_url={imageUrl} detailed_image_url={imageUrl})");
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

                                Logger.Log(LogLevel.Verbose, $"Added image to column section (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }
                            else if (ImageSections[currentSectionIndex].GetType().Equals(typeof(TitleImageSection)))
                            {
                                var titleSection = ImageSections[currentSectionIndex] as TitleImageSection;
                                titleSection.TitleText = imageTitle;

                                Logger.Log(LogLevel.Verbose, $"Added image title section to photo grid (titile={imageTitle})");
                            }

                            // Blank the urls again
                            imageUrl = string.Empty;
                            detailedImageUrl = string.Empty;
                            imageTitle = string.Empty;
                        }
                    }
                }
            }

            // TODO: Check that all tokens are present
            Logger.Log(LogLevel.Verbose, $"Schema parsed (\"{path}\")");
            return true;
        }

        // TODO: Handle thrown exceptions
        // TODO: More logging
        public bool Save(string path)
        {
            // TODO: Sanity check that we have everything setup in the values tables 
            if (!SanityCheckSchema())
            {
                return false;
            }

            // Contents of our schema file
            List<string> schemaFileContents = new List<string>();

            // First add the version
            schemaFileContents.Add(CreateSchemaPair(kVersionToken, kSchemaVersion.ToString("0.0")));
            schemaFileContents.Add(Environment.NewLine);

            // Next add the tokens (Except the class ids)
            schemaFileContents.Add(kTokensTag);
            foreach (KeyValuePair<Token, string> tokenPair in TokenValues)
            {
                if (tokenPair.Key.ToString().Contains("ClassId"))
                {
                    // Don't both printing the class ids yet, we will do that after in a different section
                    continue;
                }

                // Ok we need to find what the string for the given token type is
                // use the original TokenTable for that
                string tokenName = TokenTable.GetKeyOfValue(tokenPair.Key);
                if (string.IsNullOrEmpty(tokenName))
                {
                    // This definitely isn't great to bail on writing a property
                    Logger.Log(LogLevel.Error, $"Couldn't find token name for {tokenPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                schemaFileContents.Add(CreateSchemaPair(tokenName, tokenPair.Value));
            }
            schemaFileContents.Add(Environment.NewLine);

            // Class identifiers next!
            schemaFileContents.Add(kClassIdentifierTag);
            foreach (KeyValuePair<Token, string> tokenPair in TokenValues)
            {
                // We only care about class ids this time
                if (tokenPair.Key.ToString().Contains("ClassId") == false)
                {
                    continue;
                }

                // Same thing again, look up the token name in the TokenTable
                string tokenName = TokenTable.GetKeyOfValue(tokenPair.Key);
                if (string.IsNullOrEmpty(tokenName))
                {
                    Logger.Log(LogLevel.Error, $"Couldn't find token name for {tokenPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                schemaFileContents.Add(CreateSchemaPair(tokenName, tokenPair.Value));
            }
            schemaFileContents.Add(Environment.NewLine);

            // Then the options section
            schemaFileContents.Add(kOptionsTag);
            foreach (KeyValuePair<Option, string> optionPair in OptionValues)
            {
                string optionName = _optionsTable.GetKeyOfValue(optionPair.Key);
                if (string.IsNullOrEmpty(optionName))
                {
                    Logger.Log(LogLevel.Error, $"Couldn't find option name for {optionPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                schemaFileContents.Add(CreateSchemaPair(optionName, optionPair.Value));
            }
            schemaFileContents.Add(Environment.NewLine);

            // Finally we print out the photogrid
            schemaFileContents.Add(_imageTagTable.GetKeyOfValue(ImageTag.Grid));
            schemaFileContents.Add(Environment.NewLine);

            // Just to save some lookups
            string imageUrlTokenName = ImageTokenTable.GetKeyOfValue(Token.Image);
            string detailedImageUrlTokenName = ImageTokenTable.GetKeyOfValue(Token.DetailedImage);
            string titleTokenName = ImageTokenTable.GetKeyOfValue(Token.Title);
            string standaloneTag = _imageTagTable.GetKeyOfValue(ImageTag.Standalone);
            string columnTag = _imageTagTable.GetKeyOfValue(ImageTag.Column);
            string titleTag = _imageTagTable.GetKeyOfValue(ImageTag.Title);

            foreach (ImageSection section in ImageSections)
            {
                if (section == null)
                {
                    continue;
                }

                if (section.GetType() == typeof(StandaloneImageSection))
                {
                    StandaloneImageSection standaloneImageSection = (StandaloneImageSection)section;

                    schemaFileContents.Add(standaloneTag);
                    schemaFileContents.Add(CreateSchemaPair(imageUrlTokenName, standaloneImageSection.PreviewImage));
                    schemaFileContents.Add(CreateSchemaPair(detailedImageUrlTokenName, standaloneImageSection.DetailedImage));
                }
                else if (section.GetType() == typeof(ColumnImageSection))
                {
                    ColumnImageSection columnImageSection = (ColumnImageSection) section;

                    schemaFileContents.Add(columnTag);
                    foreach (StandaloneImageSection columnItem in columnImageSection.Sections)
                    {
                        schemaFileContents.Add(CreateSchemaPair(imageUrlTokenName, columnItem.PreviewImage));
                        schemaFileContents.Add(CreateSchemaPair(detailedImageUrlTokenName, columnItem.DetailedImage));
                    }
                }
                else if (section.GetType() == typeof(TitleImageSection))
                {
                    TitleImageSection titleImageSection = (TitleImageSection)section;

                    schemaFileContents.Add(titleTag);
                    schemaFileContents.Add(CreateSchemaPair(titleTokenName, titleImageSection.TitleText));
                }
                schemaFileContents.Add(Environment.NewLine);
            }

            // Now we try and write it all to a file
            string schemaPath = Path.Combine(path, "SCHEMA");
            File.WriteAllLines(schemaPath, schemaFileContents);
            Logger.Log(LogLevel.Info, $"Schema generated: {schemaPath}");
            return true;
        }

        private bool SanityCheckSchema()
        {
            // TODO: Check table contents etc
            return true;
        }

        // Shorthand to create a schema parameter and it's value. Just means that if we need to modify the format of the schema we can do it in one place.
        private string CreateSchemaPair(string tokenName, string tokenValue)
        {
            return string.Format("{0}={1}", tokenName, tokenValue);
        }

        // Shorthand for parsing a token value from a token
        private string GetTokenValue(string token)
        {
            return token.Split('=').Last().Split("``").First().StripWhitespaces();
        }

        public bool Equals(Schema? other)
        {
            if (other == null)
            {
                return false;
            }

            bool equal = false;
            equal &= TokenValues == other.TokenValues;
            equal &= OptionValues == other.OptionValues;
            equal &= ImageSections == other.ImageSections;
            return equal;
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
