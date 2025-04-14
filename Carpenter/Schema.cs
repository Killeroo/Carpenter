using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;

namespace Carpenter
{
    // TODO: Rename to 'Page'
    /// <summary>
    /// Defines a page in a site, contains values that, when used with a template, are used to generate a finished webpage
    /// </summary>
    public class Schema : IDisposable, IEquatable<Schema>
    {
        // Some cosmetic tags used in the schema file 
        private const string kTokensTag = "[TAGS]";
        private const string kOptionsTag = "[OPTIONS]";

        public enum Options
        {
            OutputFilename,
        }

        public enum Tokens
        {
            // Page tokens
            PageUrl, // Note: Not specified in schema file, generated dynamically in template.cs
            Location,
            Title,
            Month,
            Year,
            Author,
            Camera,
            Thumbnail,
            Description,
            GridTitle,

            // Layout specific tokens
            Image,
            AlternateImage,
            
            // Image specific tokens
            ImageWidth,
            ImageHeight
        };

        private enum ElementTags
        {
            Grid,
            Standalone,
            Column,
            Title
        }

        /// <summary>
        /// A table containing all possible tokens that can be found in a SCHEMA along with
        /// their corresponding string that is actually used to store their value in the SCHEMA file
        /// </summary>
        public static readonly Dictionary<string, Tokens> TokenTable = new()
        {
            { "%PAGE_URL", Tokens.PageUrl },
            { "%LOCATION", Tokens.Location },
            { "%TITLE", Tokens.Title },
            { "%MONTH", Tokens.Month },
            { "%YEAR", Tokens.Year },
            { "%CAMERA", Tokens.Camera },
            { "%AUTHOR", Tokens.Author },
            { "%THUMBNAIL", Tokens.Thumbnail },
            { "%DESCRIPTION", Tokens.Description },
            { "%IMAGE", Tokens.Image },
            { "%ALT_IMAGE", Tokens.AlternateImage },
            { "%WIDTH", Tokens.ImageWidth },
            { "%HEIGHT", Tokens.ImageHeight }
        };

        /// <summary>
        /// A table containing all possible options that can be found in a SCHEMA along with
        /// their corresponding string that is actually used to store their value in the SCHEMA file
        /// </summary>
        public static readonly Dictionary<string, Options> OptionsTable = new()
        {
            { "output_file", Options.OutputFilename }
        };

        /// <summary>
        /// Table that contains all the tokens that can be used in the layout portion of the SCHEMA file
        /// </summary>
        public static readonly Dictionary<string, Tokens> LayoutTokenTable = new()
        {
            { "%IMAGE", Tokens.Image },
            { "%ALT_IMAGE", Tokens.AlternateImage },
            { "%GRID_TITLE", Tokens.GridTitle },
        };

        /// <summary>
        /// List of tags that are optional and don't have to be explictly specified in the schema 
        /// </summary>
        public static readonly HashSet<Tokens> OptionalTokens = new()
        {
            Tokens.Description,
            Tokens.GridTitle
            // Tokens.PageUrl
        };

        /// <summary>
        /// Table containing all the tags that are used to represent different layout sections in the SCHEMA file
        /// </summary>
        private static readonly Dictionary<string, ElementTags> _layoutTagTable = new()
        {
            { "[LAYOUT]", ElementTags.Grid },
            { "[IMAGE_STANDALONE]", ElementTags.Standalone },
            { "[IMAGE_COLUMN]", ElementTags.Column },
            { "[TITLE]", ElementTags.Title },
        };

        /// <summary>
        /// All the tokens and their values that can have been found in the SCHEMA
        /// </summary>
        public Dictionary<Tokens, string> TokenValues { get; private set; } = new();
        /// <summary>
        /// All the options and their values that have been found in the SCHEMA
        /// </summary>
        public Dictionary<Options, string> OptionValues { get; private set; } = new();
        /// <summary>
        /// The sections found in the layout portion of the SCHEMA
        /// </summary>
        public List<Section> LayoutSections { get; set; } = new();

        /// <summary>
        /// Denotes if the Schema file was loaded from a file correctly
        /// </summary>
        private bool _loaded = false;

        /// <summary>
        /// The directory containing the schema file
        /// </summary>
        public string _workingDirectory = string.Empty;

        /// <summary>
        /// Accessors for Token values
        /// </summary>
        public string Location 
        {
            get { return TokenValues.TryGetValue(Tokens.Location, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Location, value); }
        }
        public string Title 
        {
            get { return TokenValues.TryGetValue(Tokens.Title, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Title, value); }
        }
        public string Month 
        {
            get { return TokenValues.TryGetValue(Tokens.Month, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Month, value); }
        }
        public string Year 
        {
            get { return TokenValues.TryGetValue(Tokens.Year, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Year, value); }
        }
        public string Author 
        {
            get { return TokenValues.TryGetValue(Tokens.Author, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Author, value); }
        }
        public string Camera 
        {
            get { return TokenValues.TryGetValue(Tokens.Camera, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Camera, value); }
        }
        public string Thumbnail 
        {
            get { return TokenValues.TryGetValue(Tokens.Thumbnail, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Thumbnail, value); }
        }
        public string Description 
        {
            get { return TokenValues.TryGetValue(Tokens.Description, out string value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Description, value); }
        }

        /// <summary>
        /// Accessors for Option values
        /// </summary>
        public string GeneratedFilename
        {
            get { return OptionValues.TryGetValue(Options.OutputFilename, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.OutputFilename, value); }
        }

        public bool IsLoaded() => _loaded;
        public string WorkingDirectory() => _workingDirectory;


        public Schema() => _loaded = true;
        public Schema(string path) => TryLoad(path);
        public Schema(Schema otherSchema)
        {
            if (otherSchema == null)
            {
                return;
            }

            _loaded = otherSchema._loaded;
            _workingDirectory = otherSchema._workingDirectory;

            TokenValues = new Dictionary<Tokens, string>(otherSchema.TokenValues);
            OptionValues = new Dictionary<Options, string>(otherSchema.OptionValues);
            LayoutSections = new List<Section>();
            
            // Need to actually populate the ImageSections manually to avoid copying a reference
            foreach (Section section in otherSchema.LayoutSections)
            {
                if (section is ImageSection standaloneSection)
                {
                    ImageSection newSection = new ImageSection();
                    newSection.AltImageUrl = standaloneSection.AltImageUrl;
                    newSection.ImageUrl = standaloneSection.ImageUrl;
                    LayoutSections.Add(newSection);
                }
                else if (section is ImageColumnSection columnSection)
                {
                    ImageColumnSection newColumnSection = new ImageColumnSection();
                    newColumnSection.Sections = new List<ImageSection>();
                    foreach (Section innerSection in columnSection.Sections)
                    {
                        if (innerSection is ImageSection innerStandaloneSection)
                        {
                            ImageSection newSection = new ImageSection();
                            newSection.AltImageUrl = innerStandaloneSection.AltImageUrl;
                            newSection.ImageUrl = innerStandaloneSection.ImageUrl;
                            newColumnSection.Sections.Add(newSection);
                        }
                    }
                    LayoutSections.Add(newColumnSection);
                }
                else if (section is TitleSection titleSection)
                {
                    TitleSection newSection = new TitleSection();
                    newSection.TitleText = titleSection.TitleText;
                    LayoutSections.Add(newSection);
                }
            }
        }

        /// <summary>
        /// Attempts to load the schema from a file in the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <returns>If the schema was sucesfully loaded or not</returns>
        public bool TryLoad(string path)
        {
            _loaded = false;

            // Read the file
            string[] schemaFileContents = File.ReadAllLines(path);
            _workingDirectory = Path.GetDirectoryName(path);

            // Sanity check size
            if (schemaFileContents.Length == 0)
            {
                Logger.Log(LogLevel.Error, $"Schema file empty!");
                return false;
            }

            // Version check
            if (schemaFileContents[0].Contains(Config.kVersionOption) == false || schemaFileContents[0].Contains('=') == false)
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
                Logger.Log(LogLevel.Error, $"Could not parse schema version string (\'{versionValue}\') ({e.GetType()} exception occured)");
                return false;
            }
            if (version != Config.kVersion)
            {
                Logger.Log(LogLevel.Error, $"Incompabitable schema version, found v{version} but can only parse v{Config.kVersion}. Please update.");
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
                        TokenValues[TokenTable[token]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Schema token {TokenTable[token]}={TokenValues[TokenTable[token]]}");
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
                        OptionValues[OptionsTable[option]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Schema option {OptionsTable[option]}={OptionValues[OptionsTable[option]]}");
                        break;
                    }
                }
            }

            // Parse photo grid layout
            string gridTag = string.Empty;
            foreach (var item in _layoutTagTable)
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
            string altImageUrl = string.Empty;
            string imageTitle = string.Empty;
            for (int i = photoSectionStartIndex; i < schemaFileContents.Length; i++)
            {
                string line = schemaFileContents[i];

                // If the line contains a tag
                foreach (string tag in _layoutTagTable.Keys)
                {
                    if (line.Contains(tag))
                    {
                        switch (_layoutTagTable[tag])
                        {
                            case ElementTags.Standalone:
                                currentSectionIndex++;
                                LayoutSections.Add(new ImageSection());
                                Logger.Log(LogLevel.Verbose, "Adding standalone image section to photo grid");
                                break;

                            case ElementTags.Column:
                                currentSectionIndex++;
                                LayoutSections.Add(new ImageColumnSection());
                                Logger.Log(LogLevel.Verbose, "Adding column section to photo grid");
                                break;

                            case ElementTags.Title:
                                currentSectionIndex++;
                                LayoutSections.Add(new TitleSection());
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
                foreach (string token in LayoutTokenTable.Keys)
                {
                    if (line.Contains(token))
                    {
                        string tokenValue = line.Split('=').Last();

                        switch (LayoutTokenTable[token])
                        {
                            case Tokens.Image:
                                imageUrl = tokenValue;
                                break;

                            case Tokens.AlternateImage:
                                altImageUrl = tokenValue;
                                break;

                            case Tokens.GridTitle:
                                imageTitle = tokenValue;
                                break;

                            default:
                                // TODO: Stronger identification for bad formatted tags
                                Logger.Log(LogLevel.Error, $"Could not parse token ({token}) in photo grid");
                                return false;
                        }

                        if (imageUrl != string.Empty)
                        {
                            ImageSection imageSectionToAdd = new();
                            imageSectionToAdd.ImageUrl = imageUrl;
                            
                            // See if the next line contains an alt image definition.
                            if (i != schemaFileContents.Length - 1 && schemaFileContents[i + 1].Contains(LayoutTokenTable.GetKeyOfValue(Tokens.AlternateImage)))
                            {
                                imageSectionToAdd.AltImageUrl = schemaFileContents[i++].Split('=').Last();
                            }
                            
                            if (LayoutSections[currentSectionIndex].GetType().Equals(typeof(ImageSection)))
                            {
                                var standaloneSection = LayoutSections[currentSectionIndex] as ImageSection;
                                standaloneSection.ImageUrl = imageSectionToAdd.ImageUrl;
                                standaloneSection.AltImageUrl = imageSectionToAdd.AltImageUrl;

                                Logger.Log(LogLevel.Verbose,
                                    $"Added single image section to photo grid (image_url={imageUrl} alternate_image_url={imageUrl})");
                            }
                            else if (LayoutSections[currentSectionIndex].GetType().Equals(typeof(ImageColumnSection)))
                            {
                                // If we are dealing with a column section
                                var columnSection = LayoutSections[currentSectionIndex] as ImageColumnSection;
                                columnSection.Sections.Add(imageSectionToAdd);

                                Logger.Log(LogLevel.Verbose,
                                    $"Added image to column section (image_url={imageUrl} alternate_image_url={imageUrl})");
                            }
                        }
                        else if (imageTitle != string.Empty && LayoutSections[currentSectionIndex].GetType().Equals(typeof(TitleSection)))
                        {
                            var titleSection = LayoutSections[currentSectionIndex] as TitleSection;
                            titleSection.TitleText = imageTitle;

                            Logger.Log(LogLevel.Verbose, $"Added image title section to photo grid (titile={imageTitle})");
                        }

                        // Blank the urls again
                        imageUrl = string.Empty;
                        altImageUrl = string.Empty;
                        imageTitle = string.Empty;
                    }
                }
            }

            if (IsValid())
            {
                Logger.Log(LogLevel.Verbose, $"Schema parsed (\"{path}\")");
                _loaded = true;
                return true;
            }
            else
            {
                Logger.Log(LogLevel.Error, $"Schema failed sanity check");
                Reset();
                _loaded = false;
                return false;
            }

        }

        /// <summary>
        /// Attempts to save the schema to a file at the given path
        /// </summary>
        /// <returns>If saving the schema was successfully written to a file or not</returns>
        public bool TrySave(string path)
        {
            if (!IsValid())
            {
                Logger.Log(LogLevel.Error, $"Could not save schema, sanity check failed");
                return false;
            }

            // Shorthand to create a schema parameter and it's value. Just means that if we need to modify the format of the schema we can do it in one place.
            string CreateNameValuePair(string name, string value)
            {
                return string.Format("{0}={1}", name, value);
            }

            // Contents of our schema file
            List<string> schemaFileContents = new List<string>();

            // First add the version
            schemaFileContents.Add(CreateNameValuePair(Config.kVersionOption, Config.kVersion.ToString("0.0")));
            schemaFileContents.Add(Environment.NewLine);

            // Next add the tokens (Except the class ids)
            schemaFileContents.Add(kTokensTag);
            foreach (KeyValuePair<Tokens, string> tokenPair in TokenValues)
            {
                // This tag is generated so don't bother saving it a file
                if (tokenPair.Key == Tokens.PageUrl) continue;
                
                // Ok we need to find what the string for the given token type is
                // use the original TokenTable for that
                string tokenName = TokenTable.GetKeyOfValue(tokenPair.Key);
                if (string.IsNullOrEmpty(tokenName))
                {
                    // This definitely isn't great to bail on writing a property
                    Logger.Log(LogLevel.Error, $"Couldn't find token name for {tokenPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                schemaFileContents.Add(CreateNameValuePair(tokenName, tokenPair.Value));
                Logger.Log(LogLevel.Verbose, $"Adding {tokenName} to schema file");
            }
            schemaFileContents.Add(Environment.NewLine);

            // Then the options section
            schemaFileContents.Add(kOptionsTag);
            foreach (KeyValuePair<Options, string> optionPair in OptionValues)
            {
                string optionName = OptionsTable.GetKeyOfValue(optionPair.Key);
                if (string.IsNullOrEmpty(optionName))
                {
                    Logger.Log(LogLevel.Error, $"Couldn't find option name for {optionPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                schemaFileContents.Add(CreateNameValuePair(optionName, optionPair.Value));
            }
            schemaFileContents.Add(Environment.NewLine);

            // Finally we print out the grid
            schemaFileContents.Add(_layoutTagTable.GetKeyOfValue(ElementTags.Grid));
            schemaFileContents.Add(Environment.NewLine);
            Logger.Log(LogLevel.Verbose, $"Generating schema layout grid");

            // Just to save some lookups
            string imageUrlTokenName = LayoutTokenTable.GetKeyOfValue(Tokens.Image);
            string detailedImageUrlTokenName = LayoutTokenTable.GetKeyOfValue(Tokens.AlternateImage);
            string titleTokenName = LayoutTokenTable.GetKeyOfValue(Tokens.Title);
            string standaloneTag = _layoutTagTable.GetKeyOfValue(ElementTags.Standalone);
            string columnTag = _layoutTagTable.GetKeyOfValue(ElementTags.Column);
            string titleTag = _layoutTagTable.GetKeyOfValue(ElementTags.Title);

            foreach (Section section in LayoutSections)
            {
                if (section == null)
                {
                    continue;
                }

                if (section.GetType() == typeof(ImageSection))
                {
                    ImageSection standaloneImageSection = (ImageSection)section;

                    schemaFileContents.Add(standaloneTag);
                    schemaFileContents.Add(CreateNameValuePair(imageUrlTokenName, standaloneImageSection.ImageUrl));
                    schemaFileContents.Add(CreateNameValuePair(detailedImageUrlTokenName, standaloneImageSection.AltImageUrl));

                    Logger.Log(LogLevel.Verbose, "Added ImageSection to layout");
                }
                else if (section.GetType() == typeof(ImageColumnSection))
                {
                    ImageColumnSection columnImageSection = (ImageColumnSection) section;

                    schemaFileContents.Add(columnTag);
                    foreach (ImageSection columnItem in columnImageSection.Sections)
                    {
                        schemaFileContents.Add(CreateNameValuePair(imageUrlTokenName, columnItem.ImageUrl));
                        schemaFileContents.Add(CreateNameValuePair(detailedImageUrlTokenName, columnItem.AltImageUrl));
                    }

                    Logger.Log(LogLevel.Verbose, "Added ImageColumnSection to layout");
                }
                else if (section.GetType() == typeof(TitleSection))
                {
                    TitleSection titleImageSection = (TitleSection)section;

                    schemaFileContents.Add(titleTag);
                    schemaFileContents.Add(CreateNameValuePair(titleTokenName, titleImageSection.TitleText));
                }

                schemaFileContents.Add(Environment.NewLine);
                Logger.Log(LogLevel.Verbose, "Added TitleSection to layout");
            }

            // Now we try and write it all to a file
            string schemaPath = Path.Combine(path, Config.kSchemaFileName);
            try
            {
                File.WriteAllLines(schemaPath, schemaFileContents);
                Logger.Log(LogLevel.Info, $"Schema generated: {schemaPath}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Could not save schema file ({ex.GetType()} exception occured)");
                return false;
            }
        }

        /// <summary>
        /// Performs some basic tests to make sure that all values that should be present in the schema are present
        /// </summary>
        /// <returns>If the schema is valid or not</returns>
        private bool IsValid()
        {
            if (!SchemaValidator.Run(this, out SchemaValidator.ValidationResults results))
            {
                Logger.Log(LogLevel.Error, "Schema validator failed! Some required tests did not pass.");
                Logger.Log(LogLevel.Error, results.ToString());
                return false;
            }
            
            if (results.FailedTests.Count > 0)
            {
                Logger.Log(LogLevel.Warning, "Schema is valid but some optional tests did not pass.");
                Logger.Log(LogLevel.Warning, results.ToString());
            }
            return true;
        }
        
        /// <summary>
        /// Clears the contents and values of the schema
        /// </summary>
        private void Reset()
        {
            TokenValues.Clear();
            OptionValues.Clear();
            LayoutSections.Clear();
            _loaded = false;
            _workingDirectory = string.Empty;
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
            equal &= LayoutSections == other.LayoutSections;
            return equal;
        }

        private bool _disposed;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                Reset();

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
