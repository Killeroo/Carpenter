using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace Carpenter
{
    // TODO: Rename to 'Page'
    // TODO: Change to struct
    public class Schema : IDisposable, IEquatable<Schema>
    {
        public const float kSchemaVersion = 2.0f;
        
        private const string kVersionToken = "schema_version";

        // Some cosmetic tags used in the schema file 
        private const string kTokensTag = "[TAGS]";
        private const string kClassIdentifierTag = "[CLASS_IDENTIFIERS]";
        private const string kOptionsTag = "[OPTIONS]";

        public enum Options
        {
            OutputFilename, // TODO: Rename to generatdfilename
            PageImage // TODO: Rename to thumbnail and move to token
        }

        public enum Tokens
        {
            // Page tokens
            PageUrl, // TODO: Rename to url
            Location,
            Title,
            Month,
            Year,
            Author,
            Camera,

            // Image specific tokens
            Image,
            DetailedImage,
            ImageTitle
        };

        private enum ElementTags
        {
            Grid,
            Standalone,
            Column,
            Title
        }

        public static readonly Dictionary<string, Tokens> TokenTable = new()
        {
            { "%PAGE_URL", Tokens.PageUrl },
            { "%LOCATION", Tokens.Location },
            { "%TITLE", Tokens.Title },
            { "%MONTH", Tokens.Month },
            { "%YEAR", Tokens.Year },
            { "%CAMERA", Tokens.Camera },
            { "%AUTHOR", Tokens.Author }
        };

        public static readonly Dictionary<string, Options> OptionsTable = new()
        {
            { "output_file", Options.OutputFilename },
            { "page_image", Options.PageImage }
        };

        public static readonly Dictionary<string, Tokens> ImageTokenTable = new()
        {
            { "%IMAGE_URL", Tokens.Image },
            { "%DETAILED_IMAGE_URL", Tokens.DetailedImage },
            { "%IMAGE_TITLE", Tokens.ImageTitle },
        };

        private static readonly Dictionary<string, ElementTags> _titleTagTable = new() // RENAME
        {
            { "[IMAGE_LAYOUT]", ElementTags.Grid }, // Remove IMAGE part of these sectons
            { "[IMAGES_STANDALONE]", ElementTags.Standalone },
            { "[IMAGES_COLUMN]", ElementTags.Column },
            { "[IMAGE_TITLE]", ElementTags.Title },
        };


        /// <summary>
        /// Accessors for Token values
        /// </summary>
        public string PageUrl 
        {
            get { return _tokenValues.TryGetValue(Tokens.PageUrl, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.PageUrl, value); }
        }
        public string Location 
        {
            get { return _tokenValues.TryGetValue(Tokens.Location, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.Location, value); }
        }
        public string Title 
        {
            get { return _tokenValues.TryGetValue(Tokens.Title, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.Title, value); }
        }
        public string Month 
        {
            get { return _tokenValues.TryGetValue(Tokens.Month, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.Month, value); }
        }
        public string Year 
        {
            get { return _tokenValues.TryGetValue(Tokens.Year, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.Year, value); }
        }
        public string Author 
        {
            get { return _tokenValues.TryGetValue(Tokens.Location, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.Location, value); }
        }
        public string Camera 
        {
            get { return _tokenValues.TryGetValue(Tokens.Title, out string value) ? value : string.Empty; }
            set { _tokenValues.AddOrUpdate(Tokens.Title, value); }
        }

        /// <summary>
        /// Accessors for Option values
        /// </summary>
        public string GeneratedFilename
        {
            get { return _optionValues.TryGetValue(Options.OutputFilename, out string value) ? value : string.Empty; }
            set { _optionValues.AddOrUpdate(Options.OutputFilename, value); }
        }

        public Dictionary<Tokens, string> GetTokenValues() { return _tokenValues; }
        public List<ImageSection> GetImageSections() { return _imageSections; }
        public bool IsLoaded() => _loaded;

        private Dictionary<Tokens, string> _tokenValues = new();
        private Dictionary<Options, string> _optionValues = new();
        private List<ImageSection> _imageSections = new();

        /// <summary>
        /// Denotes if the Schema file was loaded correctly
        /// </summary>
        private bool _loaded = false;

        public Schema() { }
        public Schema(string path) => TryLoad(path);
        public Schema(Schema otherSchema)
        {
            if (otherSchema == null)
            {
                return;
            }

            _tokenValues = new Dictionary<Tokens, string>(otherSchema._tokenValues);
            _optionValues = new Dictionary<Options, string>(otherSchema._optionValues);
            _imageSections = new List<ImageSection>();
            
            // Need to actually populate the ImageSections manually to avoid copying a reference
            foreach (ImageSection section in otherSchema._imageSections)
            {
                if (section is StandaloneImageSection standaloneSection)
                {
                    StandaloneImageSection newSection = new StandaloneImageSection();
                    newSection.DetailedImage = standaloneSection.DetailedImage;
                    newSection.PreviewImage = standaloneSection.PreviewImage;
                    _imageSections.Add(newSection);
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
                    _imageSections.Add(newColumnSection);
                }
                else if (section is TitleImageSection titleSection)
                {
                    TitleImageSection newSection = new TitleImageSection();
                    newSection.TitleText = titleSection.TitleText;
                    _imageSections.Add(newSection);
                }
            }
        }

        // TODO: Throw exception
        public bool TryLoad(string path)
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
            string versionValue = schemaFileContents[0].GetTokenOrOptionValue();
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
                        _tokenValues[TokenTable[token]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Schema token {TokenTable[token]}={_tokenValues[TokenTable[token]]}");
                        break;
                    }
                }
            }

            // Parse options in file
            for (int i = 0; i < schemaFileContents.Length; i++)
            {
                string line = schemaFileContents[i];

                foreach (string option in OptionsTable.Keys)
                {
                    if (line.Contains(option))
                    {
                        _optionValues[OptionsTable[option]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Schema option {OptionsTable[option]}={_optionValues[OptionsTable[option]]}");
                        break;
                    }
                }
            }

            // Parse photo grid layout
            string gridTag = string.Empty;
            foreach (var item in _titleTagTable)
            {
                if (item.Value == ElementTags.Grid)
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
                foreach (string tag in _titleTagTable.Keys)
                {
                    if (line.Contains(tag))
                    {
                        switch (_titleTagTable[tag])
                        {
                            case ElementTags.Standalone:
                                currentSectionIndex++;
                                _imageSections.Add(new StandaloneImageSection());
                                Logger.Log(LogLevel.Verbose, "Adding standalone image section to photo grid");
                                break;

                            case ElementTags.Column:
                                currentSectionIndex++;
                                _imageSections.Add(new ColumnImageSection());
                                Logger.Log(LogLevel.Verbose, "Adding column section to photo grid");
                                break;

                            case ElementTags.Title:
                                currentSectionIndex++;
                                _imageSections.Add(new TitleImageSection());
                                Logger.Log(LogLevel.Verbose, "Adding title section to photo grid");
                                break;

                            case ElementTags.Grid:
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
                            case Tokens.Image:
                                imageUrl = tokenValue;
                                break;

                            case Tokens.DetailedImage:
                                detailedImageUrl = tokenValue;
                                break;

                            case Tokens.ImageTitle:
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
                            if (_imageSections[currentSectionIndex].GetType().Equals(typeof(StandaloneImageSection)))
                            {
                                // If we are dealing with a standalone section
                                var standaloneSection = _imageSections[currentSectionIndex] as StandaloneImageSection;
                                standaloneSection.PreviewImage = imageUrl;
                                standaloneSection.DetailedImage = detailedImageUrl;

                                Logger.Log(LogLevel.Verbose, $"Added single image section to photo grid (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }
                            else if (_imageSections[currentSectionIndex].GetType().Equals(typeof(ColumnImageSection)))
                            {
                                // If we are dealing with a column section
                                var columnSection = _imageSections[currentSectionIndex] as ColumnImageSection;
                                columnSection.Sections.Add(new StandaloneImageSection
                                {
                                    PreviewImage = imageUrl,
                                    DetailedImage = detailedImageUrl,
                                });

                                Logger.Log(LogLevel.Verbose, $"Added image to column section (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }
                            else if (_imageSections[currentSectionIndex].GetType().Equals(typeof(TitleImageSection)))
                            {
                                var titleSection = _imageSections[currentSectionIndex] as TitleImageSection;
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
        public bool TrySave(string path)
        {
            // TODO: Sanity check that we have everything setup in the values tables 
            if (!SanityCheck())
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
            foreach (KeyValuePair<Tokens, string> tokenPair in _tokenValues)
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
            foreach (KeyValuePair<Tokens, string> tokenPair in _tokenValues)
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
            foreach (KeyValuePair<Options, string> optionPair in _optionValues)
            {
                string optionName = OptionsTable.GetKeyOfValue(optionPair.Key);
                if (string.IsNullOrEmpty(optionName))
                {
                    Logger.Log(LogLevel.Error, $"Couldn't find option name for {optionPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                schemaFileContents.Add(CreateSchemaPair(optionName, optionPair.Value));
            }
            schemaFileContents.Add(Environment.NewLine);

            // Finally we print out the photogrid
            schemaFileContents.Add(_titleTagTable.GetKeyOfValue(ElementTags.Grid));
            schemaFileContents.Add(Environment.NewLine);

            // Just to save some lookups
            string imageUrlTokenName = ImageTokenTable.GetKeyOfValue(Tokens.Image);
            string detailedImageUrlTokenName = ImageTokenTable.GetKeyOfValue(Tokens.DetailedImage);
            string titleTokenName = ImageTokenTable.GetKeyOfValue(Tokens.Title);
            string standaloneTag = _titleTagTable.GetKeyOfValue(ElementTags.Standalone);
            string columnTag = _titleTagTable.GetKeyOfValue(ElementTags.Column);
            string titleTag = _titleTagTable.GetKeyOfValue(ElementTags.Title);

            foreach (ImageSection section in _imageSections)
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

        private bool SanityCheck()
        {
            // TODO: Check table contents etc
            // TODO: Check everything is set
            return true;
        }

        // Shorthand to create a schema parameter and it's value. Just means that if we need to modify the format of the schema we can do it in one place.
        private string CreateSchemaPair(string tokenName, string tokenValue)
        {
            return string.Format("{0}={1}", tokenName, tokenValue);
        }

        public bool Equals(Schema? other)
        {
            if (other == null)
            {
                return false;
            }

            bool equal = false;
            equal &= _tokenValues == other._tokenValues;
            equal &= _optionValues == other._optionValues;
            equal &= _imageSections == other._imageSections;
            return equal;
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _tokenValues.Clear();
                _optionValues.Clear();
                _imageSections.Clear();

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
