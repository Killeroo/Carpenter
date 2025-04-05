using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
        /// Placeholder tag, marks a place in the template which will get replaced with a section of text specified
        /// in the Site config file. 
        /// </summary>
        private struct Tag
        {
            /// <summary>
            /// Tag identifier, just the raw string that the tag uses in the template file
            /// </summary>
            public string Id;
            
            /// <summary>
            /// The parent type of tag, a common string used by multiple tags. Used for generating specific types of pages.
            /// For example, Tags with Type 'Index' will only be used when generating index pages, 'Page' tags will only be used
            /// when generating normal pages
            /// </summary>
            public string Type;
            
            /// <summary>
            /// The index where the tag is located in the file contents array.
            /// </summary>
            public int ArrayIndex;
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
        private List<string> _fileContents;

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
                _fileContents = File.ReadAllLines(path).ToList();
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
            modifiedTokenValues.AddOrUpdate(Schema.Tokens.PageUrl, string.Format("{0}/{1}", site.Url, site.GetSchemaRelativePath(schema)));

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
        
        private List<Tag> FindTags(List<string> content)
        {
            List<Tag> results = new();
            for (int index = 0; index < content.Count; index++)
            {
                Match tagMatch = Regex.Match(content[index], Config.kTagInHtmlRegexPattern);
                if (tagMatch.Success)
                {
                    Tag foundTag = new() { ArrayIndex = index, Id = tagMatch.Value };
                    tagMatch = Regex.Match(content[index], "(?<=:)(.*?)(?=:)");
                    if (tagMatch.Success) {
                        foundTag.Type = tagMatch.Groups[0].Value;
                    }
                    results.Add(foundTag); 
                    Logger.Log(LogLevel.Info, $"Found tag {content[index].StripWhitespaces()} @ {index}");
                }
            }

            return results;
        }
        
        public void GeneratePage(Schema schema, Site site, string outputPath)
        {
            if (site == null || schema == null)
            {
                return;
            }
            
            List<Tag> tags = new();
            List<string> generatedContents = new(_fileContents);
            Dictionary<Tokens, string> tokenValues = new();
            do
            {
                // Find all tags
                tags = FindTags(generatedContents);
                
                foreach (Tag tag in tags.Where(x => x.Type == "page").OrderByDescending(x => x.ArrayIndex))
                {
                    if (!site.Tags.TryGetValue(tag.Id, out List<string> tagContents)) // TODO: Skip for layout section
                    {
                        continue;
                    }

                    int offset = tag.ArrayIndex + 1;
                    string padding = generatedContents[tag.ArrayIndex].Split("<!--").First();
                    if (tag.Id == "page:layout")
                    {
                        Dictionary<Type, string> LayoutTypeToTagName = new()
                        {
                            { typeof(TitleSection), "layout:title" },
                            { typeof(ImageSection), "layout:image-standalone" },
                            { typeof(ImageColumnSection), "layout:image-column" },
                        };
                        
                        // Generate the layout section!
                        foreach (Section section in schema.LayoutSections)
                        {
                            if (!site.Tags.TryGetValue(LayoutTypeToTagName[section.GetType()], out List<string> sectionContents))
                            {
                                Debug.Assert(false); // We want to know when this fails;
                                continue;
                            }
                            
                            tokenValues.Clear();
                            List<string> sectionContentsCopy = new(sectionContents); // TODO: Hate this, remove the amount of duplicates
                            if (section is TitleSection asTitleSection)
                            {
                                tokenValues.Add(Tokens.GridTitle, asTitleSection.TitleText);
                                ResolveTokens(tokenValues, ref sectionContentsCopy);
                            }
                            else if (section is ImageSection asImageSection)
                            {
                                tokenValues.Add(Tokens.Image, asImageSection.ImageUrl);
                                tokenValues.Add(Tokens.AlternateImage, asImageSection.AltImageUrl);
                                ResolveTokens(tokenValues, ref sectionContentsCopy);
                            } 
                            else if (section is ImageColumnSection asImageColumnSection)
                            {
                                List<Tag> sectionTags = FindTags(sectionContentsCopy);
                                sectionTags = sectionTags.Where(x => x.Type.Contains("foreach")).ToList();
                                Debug.Assert(sectionTags.Count > 0);
                                
                                // We only care about the first foreach tag we find.
                                // Ideally we want to be able to resolve any tag recursively but that is for later.
                                Debug.Assert(site.Tags.TryGetValue(LayoutTypeToTagName[typeof(ImageSection)], out List<string> imageSection));
                                int innerOffset = sectionTags.First().ArrayIndex + 1;
                                foreach (ImageSection innerImageSection in asImageColumnSection.Sections)
                                {
                                    List<string> imageSectionCopy = new(imageSection);
                                    tokenValues.Clear();
                                    tokenValues.Add(Tokens.Image, innerImageSection.ImageUrl);
                                    tokenValues.Add(Tokens.AlternateImage, innerImageSection.AltImageUrl);
                                    ResolveTokens(tokenValues, ref imageSectionCopy);
                                    sectionContentsCopy.InsertRange(innerOffset, imageSectionCopy);
                                    innerOffset += imageSectionCopy.Count;
                                }
                            }
                            
                            tagContents.AddRange(sectionContentsCopy);
                        }
                        
                    }
                    
                    foreach (string line in tagContents)
                    {
                        generatedContents.Insert(offset, padding + line);
                        offset++;
                    }
                }
                
                // Clear all tag placeholders
                foreach (Tag tag in tags.OrderByDescending(x => x.ArrayIndex))
                {
                    generatedContents.RemoveAt(tag.ArrayIndex);
                }

                // Populate all tokens
                for (int index = 0; index < generatedContents.Count; index++)
                {
                    foreach (KeyValuePair<string, Tokens> token in Schema.TokenTable)
                    {
                        if (schema.TokenValues.Keys.Contains(token.Value))
                        {
                            generatedContents[index] =
                                generatedContents[index].Replace(token.Key, schema.TokenValues[token.Value]);
                        }
                    }
                }
            } while (tags.Count > 0);

            File.WriteAllLines(Path.Combine(site.GetRootDir(), "test-index.html"), generatedContents);
        }
        
        // TODO: Rename everything here, too much 'content'
        void ResolveTokens(Dictionary<Schema.Tokens, string> tokenValues,
            ref List<string> content)
        {
            for (int index = 0; index < content.Count; index++)
            {
                foreach (KeyValuePair<string, Tokens> token in Schema.TokenTable)
                {
                    if (tokenValues.TryGetValue(token.Value, out string? value))
                    {
                        content[index] = content[index].Replace(token.Key, value);
                    }
                }
            }    
        }
        
        // TODO: Add a different template for indexes
        // TODO: Expand to normal page generation
        // TODO: Add headers
        public void GenerateIndex(string relativePath, List<Schema> schemas, Site site)
        {
            if (site == null || schemas.Count == 0)
            {
                return;
            }
            
            List<Tag> tags = new();
            List<string> generatedContents = new(_fileContents);
            Dictionary<Schema.Tokens, string> tokenValues = new(schemas.First().TokenValues); // Use the token values from the first page to fill in the index fields
            do
            {
                // Find all tags
                tags = FindTags(generatedContents);
                
                foreach (Tag tag in tags.Where(x => x.Type == "index").OrderByDescending(x => x.ArrayIndex))
                {
                    if (!site.Tags.TryGetValue(tag.Id, out List<string> tagSection))
                    {
                        continue;
                    }

                    int offset = tag.ArrayIndex + 1;
                    string padding = generatedContents[tag.ArrayIndex].Split("<!--").First();
                    if (tag.Id.Contains("foreach:"))
                    {
                        List<string> generateSections = new();
                        foreach (Schema schema in schemas)
                        {
                            generateSections.AddRange(GenerateSchemaSection(schema, site, relativePath, tagSection));
                        }
                        tagSection = generateSections;
                    }

                    foreach (string line in tagSection)
                    {
                        generatedContents.Insert(offset, padding + line);
                        offset++;
                    }
                }
                
                // Clear all tag placeholders
                foreach (Tag tag in tags.OrderByDescending(x => x.ArrayIndex))
                {
                    generatedContents.RemoveAt(tag.ArrayIndex);
                }

                // Populate all tokens
                for (int index = 0; index < generatedContents.Count; index++)
                {
                    foreach (KeyValuePair<string, Tokens> token in Schema.TokenTable)
                    {
                        if (tokenValues.Keys.Contains(token.Value))
                        {
                            generatedContents[index] =
                                generatedContents[index].Replace(token.Key, tokenValues[token.Value]);
                        }
                    }
                }
            } while (tags.Count > 0);

            File.WriteAllLines(Path.Combine(site.GetRootDir() + relativePath, "index.html"), generatedContents);
        }

        private List<string> GenerateSchemaSection(Schema schema, Site site, string relativePath, List<string> contents)
        {
            List<string> generatedContents = new();
            if (schema == null || site == null || contents == null || contents.Count == 0)
            {
                return generatedContents;
            }

            // Patch in the correct path to the schema based on it's local path relative to where the site file is
            Dictionary<Schema.Tokens, string> modifiedSchemaTokens = new(schema.TokenValues);
            modifiedSchemaTokens[Schema.Tokens.PageUrl] = string.Format("{0}{1}/{2}", 
                site.Url,
                relativePath.Replace("\\", "/"),
                site.GetSchemaRelativePath(schema));

            // Get the dimensions for the thumbnail image
            (int width, int height) thumbnailDimensions = new(0, 0);
            if (schema.Thumbnail != string.Empty)
            {
                string thumbnailFilename = schema.Thumbnail;
                foreach (string imageFile in Directory.EnumerateFiles(schema.WorkingDirectory(),
                             string.Format("*.{0}", Path.GetExtension(thumbnailFilename)),
                             SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(imageFile) == thumbnailFilename)
                    {
                        ImageMetadata thumbnailData = JpegParser.GetMetadata(imageFile);
                        thumbnailDimensions = new(thumbnailData.Width, thumbnailData.Height);
                        break;
                    }
                }
            }

            foreach (string line in contents)
            {
                string processedLine = line;

                // We still need to manually find and replace the image and width height tags
                if ((thumbnailDimensions.width != 0 && thumbnailDimensions.height != 0)
                    && (line.Contains(Config.kTemplateImageWidthToken) ||
                        line.Contains(Config.kTemplateImageHeightToken)))
                {
                    processedLine = processedLine.Replace(Config.kTemplateImageWidthToken,
                        thumbnailDimensions.width.ToString());
                    processedLine = processedLine.Replace(Config.kTemplateImageHeightToken,
                        thumbnailDimensions.height.ToString());
                }

                // Process each line and replace them with values from the schema
                foreach (KeyValuePair<string, Schema.Tokens> tokenEntry in Schema.TokenTable)
                {
                    if (modifiedSchemaTokens.ContainsKey(tokenEntry.Value) &&
                        !Schema.OptionalTokens.Contains(tokenEntry.Value))
                    {
                        processedLine = processedLine.Replace(tokenEntry.Key,
                            modifiedSchemaTokens[tokenEntry.Value]);
                    }
                }

                generatedContents.Add(processedLine);
            }

            return generatedContents;
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
            for (int i = 0; i < _fileContents.Count; i++)
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

            for (int i = startIndex; i < _fileContents.Count; i++)
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
