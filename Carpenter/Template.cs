using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JpegMetadataExtractor;
using static Carpenter.Schema;

namespace Carpenter
{
    /// <summary>
    /// Represents a piece of loaded html that contains Token values that will be replaced with values from 
    /// schema and site files to create a finished webpage.
    /// </summary>
    public class Template
    {
        /// <summary>
        /// A section of html from the raw template html that is used for a specific schema layout section (eg image, title, grid or column)
        /// </summary>
        private class HtmlSnippet
        {
            public string Element;
            public string?[] Contents;
            public string StartLine;
            public string EndLine;
            public int Length;

            // Start and end indexes in the original template array
            public int StartIndex;
            public int EndIndex;

            private readonly Template _template;

            public HtmlSnippet(Template template, int index)
            {
                _template = template;
                Parse(index);
            }

            /// <summary>
            /// Parse the html section at the given index within the owning template's html
            /// </summary>
            /// <param name="index"></param>
            private void Parse(int index)
            {
                string line = _template._fileContents[index];

                // Ok great what element is it
                Element = _template.ParseElement(line, index);

                if (line.Contains(@"/>") || line.Contains(@"</"))
                {
                    // Replace the end of the element, we are going to need it to span
                    // over multiple lines
                    line = line.Replace(@"/>", @">");

                    // TODO: Properly strip </div> on the same line

                    // Special case where the element was self contained,
                    // so the end element would be on the next line
                    EndIndex = index + 1;
                    EndLine = $"{line.CopyTill('<')}</{Element}>";
                }
                else
                {
                    // Find the index where the element ends
                    EndIndex = _template.ParseTillElementEnds(index + 1, Element);
                    EndLine = _template._fileContents[EndIndex];
                }

                // Save the line so we can use it in generation later
                StartIndex = index;
                StartLine = line;

                Length = EndIndex - StartIndex + 1;

                // Now lets copy over what is between the start and end elements (if we can)
                if (Length > 1)
                {
                    Contents = new string[Length];
                    int destinationIndex = 0;
                    for (int i = StartIndex; i <= EndIndex; i++)
                    {
                        Contents[destinationIndex] = _template._fileContents[i];
                        destinationIndex++;
                    }
                }
            }

            /// <summary>
            /// Dumps the current html section to standard output
            /// </summary>
            private void Dump()
            {
                Console.WriteLine($"Element={Element}");
                Console.WriteLine($"StartLine={StartLine}");
                Console.WriteLine($"EndLine={EndLine}");
                Console.WriteLine($"Length={Length}");
                Console.WriteLine($"StartIndex={StartIndex}");
                Console.WriteLine($"EndIndex={EndIndex}");

                if (Contents != null)
                {
                    foreach (string line in Contents)
                    {
                        Console.WriteLine(line);
                    }
                }
            }
        }

        /// <summary>
        /// The number of image elements that will be added to the generated html before the lazy loading parameter is added 
        /// (Added so the first x images will load immediately when the webpage is loaded in a browser, in theory)
        /// </summary>
        private const int kNumImagesWithoutLazyLoading = 4;

        /// <summary>
        /// The html snippets from the template file that will be used for specific Schema sections
        /// </summary>
        private HtmlSnippet? _gridSection;
        private HtmlSnippet? _imageSection;
        private HtmlSnippet? _imageColumnSection;
        private HtmlSnippet? _titleSection;

        /// <summary>
        /// The raw contents of the loaded template html file
        /// </summary>
        private string[] _fileContents;

        /// <summary>
        /// Was the template successfully loaded from a file
        /// </summary>
        private bool _loaded = false;

        public bool IsLoaded => _loaded;

        public Template() 
        {
            // Setup Jpeg parser library 
            //JpegParser.UseInternalCache = true;
            //JpegParser.CacheSize = 1;
        }
        public Template(string path) : this() => TryLoad(path);

        /// <summary>
        /// Try and load the template from a html file at the given path
        /// </summary>
        /// <returns>Was the template successfully loaded or not</returns>
        public bool TryLoad(string path)
        {
            try
            {
                _fileContents = File.ReadAllLines(path);
                FindTags();
                Logger.Log(LogLevel.Verbose, $"Template file loaded (\"{path}\")");

                _loaded = true;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Error loading template file ({ex.GetType()} exception occurred)");
                return false;
            }
        }

        internal struct Tag
        {
            private string Name;
            private string Index;
        }

        private Dictionary<int, string> _tags = new();
        private void FindTags()
        {
            for (int index = 0; index < _fileContents.Length; index++)
            {
                if (Regex.Match(_fileContents[index], Config.kTagRegexPattern).Success)
                {
                    _tags.Add(index, _fileContents[index].StripWhitespaces());
                    Logger.Log(LogLevel.Warning, $"Found tag {_fileContents[index].StripWhitespaces()} @ {index}");
                }
            }
        }
        
        /// <summary>
        /// Generate a html preview for a given schema, a preview can be opened locally and will link to local files
        /// </summary>
        /// <returns>If the page was successfully generated or not</returns>
        public bool GeneratePreviewHtmlForSchema(Schema schema, Site site, string outputPath, out string previewFilename)
        {
            string generatedFilename = schema.GeneratedFilename == string.Empty ? Config.kDefaultGeneratedFilename : schema.GeneratedFilename;
            previewFilename = string.Format("{0}{1}.html", Path.GetFileNameWithoutExtension(generatedFilename), Config.kGeneratedPreviewPostfix);
            return GenerateHtml(schema, site, outputPath, previewFilename, true);
        }

        /// <summary>
        /// Generates a html webpage for a given schema.
        /// </summary>
        /// <returns>If the webpage was successfully generated or not</returns>
        public bool GenerateHtmlForSchema(Schema schema, Site site, string outputPath)
        {
            string generatedFilename = schema.GeneratedFilename == string.Empty ? Config.kDefaultGeneratedFilename : schema.GeneratedFilename;
            return GenerateHtml(schema, site, outputPath, generatedFilename, false);
        }

        /// <summary>
        /// Internal method for creating a webpage from a schema, site and template contents
        /// </summary>
        /// <returns>If the webpage was successfully generated or not</returns>
        /// // TODO: Throw exception
        Dictionary<string, (int height, int width)> _schemaImages = new();
        private bool GenerateHtml(Schema schema, Site site, string outputPath, string outputFilename, bool isPreview)
        {
            if (!_loaded)
            {
                Logger.Log(LogLevel.Error, "Cannot generate page without template loaded. Call load() first.");
                return false;
            }

            if (schema == null || schema.IsLoaded() == false)
            {
                Logger.Log(LogLevel.Error, "Schema object not loaded, cannot generate page.");
                return false;
            }

            if (site == null || site.IsLoaded() == false)
            {
                Logger.Log(LogLevel.Error, "Site object not loaded, cannot generate page.");
                return false;
            }

            // Store a list of image size so we can use those when generating the img tags
            // HACK: Assumes output directory also contains images
            _schemaImages.Clear();
            ImageMetadata metadata;
            Logger.Log(LogLevel.Verbose, $"Caching image sizes in {outputPath}..."); // TODO: Increase cache size so it can hold all images in the directory
            foreach (string imagePath in Directory.GetFiles(outputPath, "*.jpg"))
            {
                // Fetch image dimensions
                int width = 0, height = 0;

                // TEMP: Comment out so we can diagnose and harden the jpgparser
                //try
                {
                    metadata = JpegParser.GetMetadata(imagePath);
                    width = metadata.Width;
                    height = metadata.Height;
                }
                //catch (Exception)
                //{
                //    // If we encounter an error then we fall back on reading the metadata using C#'s GDI Image interface
                //    // (it's slower and more memory intensive)
                //    System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath);
                //    width = image.Width;
                //    height = image.Height;
                //    image.Dispose();
                //}

                _schemaImages.Add(Path.GetFileName(imagePath), (height, width));
            }

            // Parse first to understand the template using the site config 
            // We need to do this before we can generate the file
            // TODO: Skip this if we previously parsed the site 
            Parse(site);

            // Ok next we need to modify the template and remove the sections we have just passed
            // TODO: Don't assume sections are nested and that photo grid is first...
            List<string> templateCopy = _fileContents.RemoveSection(_gridSection.StartIndex, _gridSection.EndIndex).ToList();

            // Next lets build our photo grid
            string[] photoGridContents = GenerateHtmlGridFromSchema(schema);
            Logger.Log(LogLevel.Verbose, $"PhotoGrid generated");

            // Ok add it to the template copy where the image grid was in the template
            templateCopy.InsertRange(_gridSection.StartIndex, photoGridContents);

            // We want to copy and modify the schema token values to add the base site url to be used when adding the page url
            Dictionary<Schema.Tokens, string> modifiedTokenValues = new(schema.TokenValues);
            modifiedTokenValues[Schema.Tokens.PageUrl] = string.Format("{0}/{1}", site.Url, modifiedTokenValues[Schema.Tokens.PageUrl]);

            // We are almost there, go through the copy and replace all tokens with values from the schema
            for (int i = 0; i < templateCopy.Count; i++)
            {
                string line = templateCopy[i];

                if (string.IsNullOrEmpty(line))
                    continue;

                foreach (var token in Schema.TokenTable)
                {
                    if (modifiedTokenValues.Keys.Contains(token.Value))
                    {
                        line = line.Replace(token.Key, modifiedTokenValues[token.Value]);
                    }
                }

                // Remove any token placeholders 
                foreach (var token in Schema.TokenTable)
                {
                    line = line.Replace(token.Key, string.Empty);
                }

                templateCopy[i] = line;
            }
            Logger.Log(LogLevel.Verbose, $"All tokens in template file replaced");

            // Add generated timestamp
            templateCopy.Insert(0, "");
            templateCopy.Insert(0, string.Format(Config.kGeneratedComment, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm''")));

            // Remove urls so images on page render correctly in preview mode
            if (isPreview)
            {
                string pageUrl = modifiedTokenValues[Schema.Tokens.PageUrl] + "/"; // This is a bit of a hack to also remove any trailing slashes in the address
                for (int index = 0; index < templateCopy.Count; index++)
                {
                    string line = templateCopy[index];
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // Replace page url
                    line = line.Replace(pageUrl, "");
                    templateCopy[index] = line;
                }
            }

            // Great now save the file out
            try
            {
                string generatedFilePath = Path.Combine(outputPath, outputFilename);
                File.WriteAllLines(generatedFilePath, templateCopy);
                Logger.Log(LogLevel.Info, $"File generated: {generatedFilePath}");

                return true;
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, $"[{e.GetType()}] Could not create generated file - {e.Message}");
                return false;
            }
        }

        /// <summary>
        /// Generates a html grid from a given schema
        /// </summary>
        /// <returns>The outputted html grid</returns>
        private int _processedImages = 0;
        private string[] GenerateHtmlGridFromSchema(Schema schema)
        {
            List<string> photoGridContents = new List<string>();
            _processedImages = 0;

            photoGridContents.Add(_gridSection.StartLine);
            foreach (Section section in schema.LayoutSections)
            {
                // TODO: Tab consistently for nested elements
                if (section.GetType() == typeof(ImageSection))
                {
                    CreateImageElement(schema, section as ImageSection, ref photoGridContents);
                    _processedImages++;
                }
                else if (section.GetType() == typeof(ImageColumnSection))
                {
                    ImageColumnSection columnSection = section as ImageColumnSection;

                    photoGridContents.Add(_imageColumnSection.StartLine);
                    foreach (ImageSection imageSection in columnSection.Sections)
                    {
                        CreateImageElement(schema, imageSection, ref photoGridContents);
                    }
                    photoGridContents.Add(_imageColumnSection.EndLine);
                    _processedImages++;
                }
                else if (section.GetType() == typeof(TitleSection))
                {
                    CreateTitleElement(schema, section as TitleSection, ref photoGridContents);
                }

            }
            photoGridContents.Add(_gridSection.EndLine);

            return photoGridContents.ToArray();
        }

        /// <summary>
        /// Creates a html snippet for a given image section
        /// </summary>
        private void CreateImageElement(Schema schema, ImageSection section, ref List<string> outputContent)
        {
            // Load the template html we are using for the image
            string[] imageTemplate = _imageSection.Contents;

            // Iterate through it and replace it with the contents of the StandaloneImageSection
            for (int i = 0; i < imageTemplate.Length; i++)
            {
                string line = imageTemplate[i];

                // Replace all image tokens with the values in the StandaloneImageSection
                foreach (var imageTokenEntry in Schema.LayoutTokenTable)
                {
                    switch (imageTokenEntry.Value)
                    {
                        case Schema.Tokens.Image:
                            line = line.Replace(imageTokenEntry.Key, section.ImageUrl);
                            break;

                        case Schema.Tokens.AlternateImage:
                            line = line.Replace(imageTokenEntry.Key, section.AltImageUrl);
                            break;
                    }
                }

                // Remove lazy loading attribute for first few images
                if (_processedImages < kNumImagesWithoutLazyLoading)
                {
                    line = line.Replace("loading=\"lazy\"", "");
                }

                if (_schemaImages.ContainsKey(section.ImageUrl))
                {
                    line = line.Replace(Config.kTemplateImageHeightToken, _schemaImages[section.ImageUrl].height.ToString());
                    line = line.Replace(Config.kTemplateImageWidthToken, _schemaImages[section.ImageUrl].width.ToString());
                }
                else
                {
                    Logger.Log(LogLevel.Warning, $"Could not find referenced image ({section.ImageUrl}) in directory, generated file might not render properly");
                }

                // Add the modified line into the outputted html
                outputContent.Add(line);
            }
        }

        /// <summary>
        /// Create a title html element from a given title section
        /// </summary>
        private void CreateTitleElement(Schema schema, TitleSection section, ref List<string> outputContent)
        {
            // Load the template html we are using for the title
            string[] titleTemplate = _titleSection.Contents;

            // Iterate through it and replace it with the contents of the StandaloneImageSection
            for (int i = 0; i < titleTemplate.Length; i++)
            {
                string line = titleTemplate[i];

                // Replace all image tokens with the values in the StandaloneImageSection
                foreach (var imageTokenEntry in Schema.LayoutTokenTable)
                {
                    if (imageTokenEntry.Value == Schema.Tokens.GridTitle)
                    {
                        line = line.Replace(imageTokenEntry.Key, section.TitleText);
                    }
                }

                // Add the modified line into the outputted html
                outputContent.Add(line);
            }
        }

        
        
        public void GeneratePage(Schema page)
        {
            
        }
        
        private void GenerateIndex(List<Schema> schemas)
        {
            
        }

        /// <summary>
        /// We need to parse the template using the site data to understand some basics about how things
        /// will be laid out and what elements in the template are what
        /// </summary>
        /// TODO: Throw exceptions
        private void Parse(Site site)
        {
            // Reset everything so we don't end up having data from another schema
            _imageSection = null;
            _imageColumnSection = null;
            _gridSection = null;
            _titleSection = null;

            // Loop through the file contents to find each relevant section of the template
            // (The schema contains the class identifiers that represent where everything should go)
            for (int i = 0; i < _fileContents.Length; i++)
            {
                string line = _fileContents[i];

                if ((line.Contains($"class=") || line.Contains($"id=")) && line.Contains(site.GridClass))
                {
                    // First we need to find of the template that corresponds to our photo grid
                    _gridSection = new HtmlSnippet(this, i);
                    Logger.Log(LogLevel.Verbose, $"Found ImageGrid element (id={site.GridClass})");
                }
                else if ((line.Contains($"class=") || line.Contains($"id=")) && line.Contains(site.ImageClass))
                {
                    // Next we need to find the second of the template that makes up the element for our image
                    _imageSection = new HtmlSnippet(this, i);
                    Logger.Log(LogLevel.Verbose, $"Found ImageSection element (id={site.ImageClass})");
                }
                else if ((line.Contains($"class=") || line.Contains($"id=")) && line.Contains(site.ColumnClass))
                {
                    _imageColumnSection = new HtmlSnippet(this, i);
                    Logger.Log(LogLevel.Verbose, $"Found ImageColumn element (id={site.ColumnClass})");
                }
                else if ((line.Contains($"class=") || line.Contains($"id=")) && line.Contains(site.TitleClass))
                {
                    _titleSection = new HtmlSnippet(this, i);
                    Logger.Log(LogLevel.Verbose, $"Found ImageTitle element (id={site.TitleClass})");
                }
            }

            if (_imageSection == null || _imageColumnSection == null || _gridSection == null || _titleSection == null)
            {
                Logger.Log(LogLevel.Error, "Did not parse all sections of template file");
                Debug.Assert(true); // TODO: Not great really
            }
            else
            {
                Logger.Log(LogLevel.Verbose, $"Template file parsed using Schema, all elements found");
            }
        }

        /// <summary>
        /// Parses the template contents till the closing tag for a specific element is found.
        /// </summary>
        /// <returns>The index in the file contents where the element ends</returns>
        private int ParseTillElementEnds(int startIndex, string elementIdentifier)
        {
            // Keep track of each element we encounter, what element it was and where we found it
            // so that we avoid thinking that our element has ended when it was infact the terminator
            // for a nested element
            Dictionary<int, string> duplicateElementTracker = new Dictionary<int, string>();

            for (int i = startIndex; i < _fileContents.Length; i++)
            {
                string line = _fileContents[i];

                if (line.Contains("</"))
                {
                    string element = ParseElement(line, i);

                    // Ok does this element match the one we are looking for
                    // and have we come accross no other nested elements of the same type
                    if (element.Equals(elementIdentifier))
                    {
                        if (!duplicateElementTracker.Values.Contains(elementIdentifier))
                        {
                            // We found the end, return the index we are at
                            return i;
                        }
                        else
                        {
                            // Ok so this belongs to a nested element
                            // Find the occurance that is closest to our current index
                            // and remove it, that is obviously the element that is closing
                            int closestKey = duplicateElementTracker.FindClosestKey(i);
                            duplicateElementTracker.Remove(closestKey);
                            continue;
                        }
                    }
                }

                // if line contains an element with the same name
                // as the one we are looking for 
                // we need to track it
                if (line.Contains('<') && !line.Contains("/>"))
                {
                    string element = ParseElement(line, i);
                    if (element.Equals(elementIdentifier))
                    {
                        // track it
                        duplicateElementTracker.Add(i, element);
                    }
                }
            }

            // We didn't find it 
            return -1;
        }

        /// <summary>
        /// Parse and return the html element/tag found on a given line in the template file
        /// </summary>
        private string ParseElement(string line, int index)
        {
            // Strip starting whitespaces and convert to character array
            line = line.TrimStart(' ');
            line = line.TrimStart('\t');
            char[] characters = line.ToCharArray();

            // Check the first character is valid
            if (characters[0] != '<')
            {
                Logger.Log(LogLevel.Error, $"Line {index + 1} of template file doesn't start with '<', invalid html. check template file and try again.");
                return string.Empty;
                //throw new argumentexception($"line {index + 1} of template file doesn't start with '<', invalid html. check template file and try again.");
            }

            // Parse all leading characters to crudley get the element string (till we hit a whitespace)
            const int kMaxElementCharCount = 50;
            string elementType = "";
            int count = 0;
            foreach (var @char in characters)
            {
                if (count > kMaxElementCharCount)
                {
                    Logger.Log(LogLevel.Error, $"Element too long (50+ characters), bailing. Final element string={elementType}");
                    break;
                }
                else
                {
                    count++;
                }

                if (char.IsWhiteSpace(@char) || @char.Equals('>'))
                {
                    // Remove terminator start cone
                    elementType = elementType.Replace(@"</", "");
                    elementType = elementType.Replace(">", "");

                    // Remove the start cone from the element name
                    return elementType.TrimStart('<');
                }
                else
                {
                    elementType += @char;
                }
            }

            return string.Empty;
        }
    }
}
