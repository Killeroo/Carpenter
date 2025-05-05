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
    public class Page : IDisposable, IEquatable<Page>
    {
        // Some cosmetic tags used in the page file 
        private const string kTokensTag = "[TAGS]";
        private const string kOptionsTag = "[OPTIONS]";

        public enum Options
        {
            OutputFilename,
        }

        public enum Tokens
        {
            // Page tokens
            Location,
            Title,
            Month,
            Year,
            Author,
            Camera,
            Thumbnail,
            Description,
            GridTitle,

            // Unused placeholder which denotes where special tokens start in enum
            SpecialTokenSection,

            // Layout specific tokens
            Image,
            AlternateImage,
            
            // Image specific tokens
            ImageWidth,
            ImageHeight,

            // Dynamically generated tokens
            PageUrl
        };

        private enum ElementTags
        {
            Grid,
            Standalone,
            Column,
            Title
        }

        /// <summary>
        /// A table containing all possible tokens that can be found in a PAGE file along with
        /// their corresponding string that is actually used to store their value in the PAGE file
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
        /// A table containing all possible options that can be found in a PAGE file along with
        /// their corresponding string that is actually used to store their value in the PAGE file
        /// </summary>
        public static readonly Dictionary<string, Options> OptionsTable = new()
        {
            { "output_file", Options.OutputFilename }
        };

        /// <summary>
        /// Table that contains all the tokens that can be used in the layout portion of the PAGE file
        /// </summary>
        public static readonly Dictionary<string, Tokens> LayoutTokenTable = new()
        {
            { "%IMAGE", Tokens.Image },
            { "%ALT_IMAGE", Tokens.AlternateImage },
            { "%GRID_TITLE", Tokens.GridTitle },
        };

        /// <summary>
        /// List of tags that are optional and don't have to be explictly specified in the page 
        /// </summary>
        public static readonly HashSet<Tokens> OptionalTokens = new()
        {
            Tokens.Description,
            Tokens.GridTitle
            // Tokens.PageUrl
        };

        /// <summary>
        /// Table containing all the tags that are used to represent different layout sections in the PAGE file
        /// </summary>
        private static readonly Dictionary<string, ElementTags> _layoutTagTable = new()
        {
            { "[LAYOUT]", ElementTags.Grid },
            { "[IMAGE_STANDALONE]", ElementTags.Standalone },
            { "[IMAGE_COLUMN]", ElementTags.Column },
            { "[TITLE]", ElementTags.Title },
        };

        /// <summary>
        /// All the tokens and their values that can have been found in the PAGE
        /// </summary>
        public Dictionary<Tokens, string> TokenValues { get; private set; } = new();
        /// <summary>
        /// All the options and their values that have been found in the PAGE
        /// </summary>
        public Dictionary<Options, string> OptionValues { get; private set; } = new();
        /// <summary>
        /// The sections found in the layout portion of the PAGE
        /// </summary>
        public List<Section> LayoutSections { get; set; } = new();

        /// <summary>
        /// Accessors for Token values
        /// </summary>
        public string Location 
        {
            get { return TokenValues.TryGetValue(Tokens.Location, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Location, value); }
        }
        public string Title 
        {
            get { return TokenValues.TryGetValue(Tokens.Title, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Title, value); }
        }
        public string Month 
        {
            get { return TokenValues.TryGetValue(Tokens.Month, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Month, value); }
        }
        public string Year 
        {
            get { return TokenValues.TryGetValue(Tokens.Year, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Year, value); }
        }
        public string Author 
        {
            get { return TokenValues.TryGetValue(Tokens.Author, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Author, value); }
        }
        public string Camera 
        {
            get { return TokenValues.TryGetValue(Tokens.Camera, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Camera, value); }
        }
        public string Thumbnail 
        {
            get { return TokenValues.TryGetValue(Tokens.Thumbnail, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Thumbnail, value); }
        }
        public string Description 
        {
            get { return TokenValues.TryGetValue(Tokens.Description, out string? value) ? value : string.Empty; }
            set { TokenValues.AddOrUpdate(Tokens.Description, value); }
        }

        /// <summary>
        /// Accessors for Option values
        /// </summary>
        public string GeneratedFilename
        {
            get { return OptionValues.TryGetValue(Options.OutputFilename, out string? value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.OutputFilename, value); }
        }

        public string WorkingDirectory() => _workingDirectory;

        
        /// <summary>
        /// The directory containing the page file
        /// </summary>
        private string _workingDirectory = string.Empty;
        
        /// <summary>
        /// Validation results from the last time the page was validated
        /// </summary>
        private PageValidator.ValidationResults _lastRunValidationResults = new();
        
        public Page() { }
        public Page(string path) => TryLoad(path);
        public Page(Page otherPage)
        {
            if (otherPage == null)
            {
                return;
            }

            _workingDirectory = otherPage._workingDirectory;

            TokenValues = new Dictionary<Tokens, string>(otherPage.TokenValues);
            OptionValues = new Dictionary<Options, string>(otherPage.OptionValues);
            LayoutSections = new List<Section>();
            
            // Need to actually populate the ImageSections manually to avoid copying a reference
            foreach (Section section in otherPage.LayoutSections)
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
        /// Attempts to load the page from a file in the given path.
        /// </summary>
        /// <param name="path"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <exception cref="IndexOutOfRangeException"></exception>
        /// <exception cref="PageParsingException"></exception>
        /// <returns>If the page was sucesfully loaded or not</returns>
        public void TryLoad(string path)
        {
            // Read the file
            string[] pageFileContents = File.ReadAllLines(path);
            _workingDirectory = Path.GetDirectoryName(path);

            // Sanity check size
            if (pageFileContents.Length == 0)
            {
                Logger.Log(LogLevel.Error, $"Page file empty!");
                throw new PageParsingException("Attempted to load empty page file");
            }

            // Version check
            if (pageFileContents[0].Contains(Config.kVersionOption) == false || pageFileContents[0].Contains('=') == false)
            {
                Logger.Log(LogLevel.Error, "Could not read page file version");
                throw new PageParsingException("Could not read page file version");
            }
            string versionValue = pageFileContents[0].GetTokenOrOptionValue();
            float version = 0.0f;
            try
            {
                version = float.Parse(versionValue);
            }
            catch (Exception e) 
            {
                Logger.Log(LogLevel.Error, $"Could not parse page version string (\'{versionValue}\') ({e.GetType()} exception occured)");
                throw new PageParsingException($"Could not parse page version string (\'{versionValue}\')", e);
            }
            if (version != Config.kVersion)
            {
                Logger.Log(LogLevel.Error, $"Incompabitable page version, found v{version} but can only parse v{Config.kVersion}. Please update.");
                throw new PageParsingException("Incompabitable page version");
            }

            // Parse page tokens in file
            for (int i = 0; i < pageFileContents.Length; i++)
            {
                string line = pageFileContents[i];

                foreach (KeyValuePair<string, Tokens> token in TokenTable)
                {
                    if (line.Contains(token.Key) && token.Value < Tokens.SpecialTokenSection)
                    {
                        TokenValues[TokenTable[token.Key]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Page token {TokenTable[token.Key]}={TokenValues[TokenTable[token.Key]]}");
                        break;
                    }
                }
            }

            // Parse options in file
            for (int i = 0; i < pageFileContents.Length; i++)
            {
                string line = pageFileContents[i];

                foreach (string option in OptionsTable.Keys)
                {
                    if (line.Contains(option))
                    {
                        OptionValues[OptionsTable[option]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Page option {OptionsTable[option]}={OptionValues[OptionsTable[option]]}");
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
            int photoSectionStartIndex = pageFileContents.FindIndexWhichContainsValue(gridTag);
            if (photoSectionStartIndex < 0)
            {
                Logger.Log(LogLevel.Error, $"Could not find image layout section with tag \"{gridTag}\" while parsing page. Add it to the page and try again.");
                throw new PageParsingException("Could not find image layout section in page");
            }

            // Ok lets parse everything to the end of the file and construct our image grid
            int currentSectionIndex = -1;
            string imageUrl = string.Empty;
            string altImageUrl = string.Empty;
            string imageTitle = string.Empty;
            for (int i = photoSectionStartIndex; i < pageFileContents.Length; i++)
            {
                string line = pageFileContents[i];

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
                                Logger.Log(LogLevel.Error, "Could not parse tag in page layout section");
                                throw new PageParsingException("Could not parse tag in page layout section");
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
                                Logger.Log(LogLevel.Error, $"Could not parse token ({token}) in photo grid");
                                throw new PageParsingException($"Could not parse token ({token}) in page layout");
                        }

                        if (imageUrl != string.Empty)
                        {
                            ImageSection imageSectionToAdd = new();
                            imageSectionToAdd.ImageUrl = imageUrl;
                            
                            // See if the next line contains an alt image definition.
                            if (i != pageFileContents.Length - 1 && pageFileContents[i + 1].Contains(LayoutTokenTable.GetKeyOfValue(Tokens.AlternateImage)))
                            {
                                imageSectionToAdd.AltImageUrl = pageFileContents[i++].Split('=').Last();
                            }
                            
                            if (LayoutSections[currentSectionIndex].GetType().Equals(typeof(ImageSection)))
                            {
                                var standaloneSection = LayoutSections[currentSectionIndex] as ImageSection;
                                standaloneSection.ImageUrl = imageSectionToAdd.ImageUrl;
                                standaloneSection.AltImageUrl = imageSectionToAdd.AltImageUrl;

                                Logger.Log(LogLevel.Verbose,
                                    $"Added single image section to photo grid (image_url={imageSectionToAdd.ImageUrl} al_image_url={imageSectionToAdd.AltImageUrl})");
                            }
                            else if (LayoutSections[currentSectionIndex].GetType().Equals(typeof(ImageColumnSection)))
                            {
                                // If we are dealing with a column section
                                var columnSection = LayoutSections[currentSectionIndex] as ImageColumnSection;
                                columnSection.Sections.Add(imageSectionToAdd);

                                Logger.Log(LogLevel.Verbose,
                                    $"Added image to column section (image_url={imageSectionToAdd.ImageUrl} alt_image_url={imageSectionToAdd.AltImageUrl})");
                            }
                        }
                        else if (imageTitle != string.Empty && LayoutSections[currentSectionIndex].GetType().Equals(typeof(TitleSection)))
                        {
                            var titleSection = LayoutSections[currentSectionIndex] as TitleSection;
                            titleSection.TitleText = imageTitle;

                            Logger.Log(LogLevel.Verbose, $"Added image title section to photo grid (title={imageTitle})");
                        }

                        // Blank the urls again
                        imageUrl = string.Empty;
                        altImageUrl = string.Empty;
                        imageTitle = string.Empty;
                    }
                }
            }
            
            Logger.Log(LogLevel.Verbose, $"Page loaded (\"{path}\")");
        }

        /// <summary>
        /// Attempts to save the page to a file at the given path
        /// </summary>
        /// <returns>If saving the page was successfully written to a file or not</returns>
        public bool TrySave(string path)
        {
            // Shorthand to create a page parameter and it's value. Just means that if we need to modify the format of the page we can do it in one place.
            string CreateNameValuePair(string name, string value)
            {
                return string.Format("{0}={1}", name, value);
            }

            // Contents of our page file
            List<string> pageFileContent = new List<string>();

            // First add the version
            pageFileContent.Add(CreateNameValuePair(Config.kVersionOption, Config.kVersion.ToString("0.0")));
            pageFileContent.Add(Environment.NewLine);

            // Next add the tokens (Except the class ids)
            pageFileContent.Add(kTokensTag);
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
                pageFileContent.Add(CreateNameValuePair(tokenName, tokenPair.Value));
                Logger.Log(LogLevel.Verbose, $"Adding {tokenName} to page file");
            }
            pageFileContent.Add(Environment.NewLine);

            // Then the options section
            pageFileContent.Add(kOptionsTag);
            foreach (KeyValuePair<Options, string> optionPair in OptionValues)
            {
                string optionName = OptionsTable.GetKeyOfValue(optionPair.Key);
                if (string.IsNullOrEmpty(optionName))
                {
                    Logger.Log(LogLevel.Error, $"Couldn't find option name for {optionPair.Key.ToString()}. This isn't great...");
                    continue;
                }
                pageFileContent.Add(CreateNameValuePair(optionName, optionPair.Value));
            }
            pageFileContent.Add(Environment.NewLine);

            // Finally we print out the grid
            pageFileContent.Add(_layoutTagTable.GetKeyOfValue(ElementTags.Grid));
            pageFileContent.Add(Environment.NewLine);
            Logger.Log(LogLevel.Verbose, $"Generating page layout grid");

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

                    pageFileContent.Add(standaloneTag);
                    pageFileContent.Add(CreateNameValuePair(imageUrlTokenName, standaloneImageSection.ImageUrl));
                    if (!string.IsNullOrEmpty(standaloneImageSection.AltImageUrl))
                        pageFileContent.Add(CreateNameValuePair(detailedImageUrlTokenName, standaloneImageSection.AltImageUrl));

                    Logger.Log(LogLevel.Verbose, "Added ImageSection to layout");
                }
                else if (section.GetType() == typeof(ImageColumnSection))
                {
                    ImageColumnSection columnImageSection = (ImageColumnSection) section;

                    pageFileContent.Add(columnTag);
                    foreach (ImageSection columnItem in columnImageSection.Sections)
                    {
                        pageFileContent.Add(CreateNameValuePair(imageUrlTokenName, columnItem.ImageUrl));
                        if (!string.IsNullOrEmpty(columnItem.AltImageUrl))
                            pageFileContent.Add(CreateNameValuePair(detailedImageUrlTokenName, columnItem.AltImageUrl));
                    }

                    Logger.Log(LogLevel.Verbose, "Added ImageColumnSection to layout");
                }
                else if (section.GetType() == typeof(TitleSection))
                {
                    TitleSection titleImageSection = (TitleSection)section;

                    pageFileContent.Add(titleTag);
                    pageFileContent.Add(CreateNameValuePair(titleTokenName, titleImageSection.TitleText));
                }

                pageFileContent.Add(Environment.NewLine);
                Logger.Log(LogLevel.Verbose, "Added TitleSection to layout");
            }

            // Now we try and write it all to a file
            string pagePath = Path.Combine(path, Config.kPageFileName);
            try
            {
                File.WriteAllLines(pagePath, pageFileContent);
                Logger.Log(LogLevel.Info, $"Page generated: {pagePath}");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Could not save page file ({ex.GetType()} exception occured)");
                return false;
            }
        }

        /// <summary>
        /// Performs some basic tests to make sure that all values that should be present in the page are present
        /// </summary>
        /// <returns>If the page is valid or not</returns>
        private bool IsValid()
        {
            return PageValidator.Run(this, out _lastRunValidationResults);
        }
        
        /// <summary>
        /// Clears the contents and values of the page
        /// </summary>
        private void Reset()
        {
            TokenValues.Clear();
            OptionValues.Clear();
            LayoutSections.Clear();
            _workingDirectory = string.Empty;
        }

        public bool Equals(Page? other)
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
