﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    public class Template
    {
        private class TemplateSection
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

            public TemplateSection(Template template, int index)
            {
                _template = template;
                Parse(index);
            }

            private void Parse(int index)
            {
                string line = _template.FileContents[index];

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
                    EndLine = _template.FileContents[EndIndex];
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
                        Contents[destinationIndex] = _template.FileContents[i];
                        destinationIndex++;
                    }
                }
            }

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

        private const string GeneratedComment = "<!-- Generated by Carpenter, Static Photo Website Generator (https://github.com/Killeroo/Carpenter), at {0} UTC -->";
        private const int NumImagesWithoutLazyLoading = 4;

        public string[] FileContents => _fileContents;

        private TemplateSection _imageGridSection;
        private TemplateSection _imageSection;
        private TemplateSection _imageColumnSection;
        private TemplateSection _imageTitleSection;
        private string[] _fileContents;
        private bool _loaded = false;

        public void Load(string path)
        {
            try
            {
                _fileContents = File.ReadAllLines(path);
                Logger.DebugLog($"Template file loaded (\"{path}\")");
                _loaded = true;
            }
            catch (Exception e)
            {
                Logger.DebugError($"[{e.GetType()}] Could not read template file - {e.Message}");
                return;
            }
        }

        Dictionary<string, (int height, int width)> _schemaImages = new Dictionary<string, (int height, int width)>();
        public void Generate(Schema schema, string outputPath)
        {
            if (!_loaded)
            {
                Logger.DebugError("Cannot generate page without template loaded. Call load() first.");
                return;
            }

            // Store a list of image size so we can use those when generating the img tags
            // HACK: Assumes output directory also contains images
            _schemaImages.Clear();
            Logger.DebugLog($"Caching image sizes in {outputPath}...");
            foreach (string imagePath in Directory.GetFiles(outputPath, "*.jpg"))
            {
                using (System.Drawing.Image image = System.Drawing.Image.FromFile(imagePath))
                {
                    _schemaImages.Add(Path.GetFileName(imagePath), (image.Height, image.Width));
                }
            }

            // TODO: More logs

            // Parse first to understand the template using the schema 
            // We need to do this before we can generate the file
            Parse(schema);

            // Ok next we need to modify the template and remove the sections we have just passed
            // TODO: Don't assume sections are nested and that photo grid is first...
            List<string> templateCopy = _fileContents.RemoveSection(_imageGridSection.StartIndex, _imageGridSection.EndIndex).ToList();

            // Next lets build our photo grid
            string[] photoGridContents = GeneratePhotoGrid(schema);
            Logger.DebugLog($"PhotoGrid generated");

            // Ok add it to the template copy where the image grid was in the template
            templateCopy.InsertRange(_imageGridSection.StartIndex, photoGridContents);

            // We are almost there, go through the copy and replace all tokens with values from the schema
            for (int i = 0; i < templateCopy.Count; i++)
            {
                string line = templateCopy[i];

                if (string.IsNullOrEmpty(line))
                    continue;

                foreach (var token in schema.TokenTable)
                {
                    line = line.Replace(token.Key, schema.TokenValues[token.Value]);
                }

                templateCopy[i] = line;
            }
            Logger.DebugLog($"All tokens in template file replaced");

            // Add generated timestamp
            templateCopy.Insert(0, "");
            templateCopy.Insert(0, string.Format(GeneratedComment, DateTime.Now.ToString("yyyy'-'MM'-'dd' 'HH':'mm''")));

            // Great now save the file out
            try
            {
                // TODO: Add file name to schema and working directory
                string generatedPath = Path.Combine(outputPath, schema.OptionValues[Schema.Option.OutputFilename]);
                File.WriteAllLines(generatedPath, templateCopy);
                Logger.DebugLog($"File generated: {generatedPath}");
            }
            catch (Exception e)
            {
                Logger.DebugError($"[{e.GetType()}] Could not create generated file - {e.Message}");
            }

        }

        // TODO: Tab consistently for nested elements
        private int _processedImages = 0;
        private string[] GeneratePhotoGrid(Schema schema)
        {
            List<string> photoGridContents = new List<string>();
            _processedImages = 0;

            photoGridContents.Add(_imageGridSection.StartLine);
            foreach (ImageSection section in schema.ImageSections)
            {
                if (section.GetType() == typeof(StandaloneImageSection))
                {
                    CreateImageElement(schema, section as StandaloneImageSection, ref photoGridContents);

                    _processedImages++;
                }
                else if (section.GetType() == typeof(ColumnImageSection))
                {
                    ColumnImageSection columnSection = section as ColumnImageSection;

                    photoGridContents.Add(_imageColumnSection.StartLine);

                    foreach (StandaloneImageSection imageSection in columnSection.Sections)
                    {
                        CreateImageElement(schema, imageSection, ref photoGridContents);
                    }

                    photoGridContents.Add(_imageColumnSection.EndLine);

                    _processedImages++;
                }
                else if (section.GetType() == typeof(TitleImageSection))
                {
                    CreateTitleElement(schema, section as TitleImageSection, ref photoGridContents);
                }

            }
            photoGridContents.Add(_imageGridSection.EndLine);

            return photoGridContents.ToArray();

        }

        private void CreateImageElement(Schema schema, StandaloneImageSection section, ref List<string> outputContent)
        {
            // Load the template html we are using for the image
            string[] imageTemplate = _imageSection.Contents;

            // Iterate through it and replace it with the contents of the StandaloneImageSection
            for (int i = 0; i < imageTemplate.Length; i++)
            {
                string line = imageTemplate[i];

                // Replace all image tokens with the values in the StandaloneImageSection
                foreach (var imageTokenEntry in schema.ImageTokenTable)
                {
                    switch (imageTokenEntry.Value)
                    {
                        case Schema.Token.Image:
                            line = line.Replace(imageTokenEntry.Key, section.PreviewImage);
                            break;

                        case Schema.Token.DetailedImage:
                            line = line.Replace(imageTokenEntry.Key, section.DetailedImage);
                            break;
                    }
                }

                // Remove lazy loading attribute for first few images
                if (_processedImages < NumImagesWithoutLazyLoading)
                {
                    line = line.Replace("loading=\"lazy\"", "");
                }

                line = line.Replace("%HEIGHT", _schemaImages[section.PreviewImage].height.ToString());
                line = line.Replace("%WIDTH", _schemaImages[section.PreviewImage].width.ToString());

                // Add the modified line into the outputted html
                outputContent.Add(line);
            }
        }

        private void CreateTitleElement(Schema schema, TitleImageSection section, ref List<string> outputContent)
        {
            // Load the template html we are using for the title
            string[] titleTemplate = _imageTitleSection.Contents;

            // Iterate through it and replace it with the contents of the StandaloneImageSection
            for (int i = 0; i < titleTemplate.Length; i++)
            {
                string line = titleTemplate[i];

                // Replace all image tokens with the values in the StandaloneImageSection
                foreach (var imageTokenEntry in schema.ImageTokenTable)
                {
                    if (imageTokenEntry.Value == Schema.Token.ImageTitle)
                    {
                        line = line.Replace(imageTokenEntry.Key, section.TitleText);
                    }
                }

                // Add the modified line into the outputted html
                outputContent.Add(line);
            }
        }

        /// <summary>
        /// We need to parse the template using the schema to understand some basics about how things
        /// are laid out
        /// </summary>
        /// <param name="schema"></param>
        private void Parse(Schema schema)
        {
            // Reset everything so we don't end up having data from another schema
            _imageSection = null;
            _imageColumnSection = null;
            _imageGridSection = null;
            _imageTitleSection = null;

            // Loop through the file contents to find each relevant section of the template
            // (The schema contains the class identifiers that represent where everything should go)
            for (int i = 0; i < _fileContents.Length; i++)
            {
                string line = _fileContents[i];

                if (line.Contains($"class=\"{schema.TokenValues[Schema.Token.ClassIdImageGrid]}\""))
                {
                    // First we need to find of the template that corresponds to our photo grid
                    _imageGridSection = new TemplateSection(this, i);
                    Logger.DebugLog($"Found ImageGrid element (id={schema.TokenValues[Schema.Token.ClassIdImageGrid]})");
                }
                else if (line.Contains($"class=\"{schema.TokenValues[Schema.Token.ClassIdImageElement]}\""))
                {
                    // Next we need to find the second of the template that makes up the element for our image
                    _imageSection = new TemplateSection(this, i);
                    Logger.DebugLog($"Found ImageSection element (id={schema.TokenValues[Schema.Token.ClassIdImageElement]})");
                }
                else if (line.Contains($"class=\"{schema.TokenValues[Schema.Token.ClassIdImageColumn]}\""))
                {
                    // Next we need to know what what our column sections are
                    _imageColumnSection = new TemplateSection(this, i);
                    Logger.DebugLog($"Found ImageColumn element (id={schema.TokenValues[Schema.Token.ClassIdImageColumn]})");
                }
                else if (schema.TokenValues.ContainsKey(Schema.Token.ClassIdImageTitle) && line.Contains($"class=\"{schema.TokenValues[Schema.Token.ClassIdImageTitle]}\""))
                {
                    // Next we need to know what what our column sections are
                    _imageTitleSection = new TemplateSection(this, i);
                    Logger.DebugLog($"Found ImageTitle element (id={schema.TokenValues[Schema.Token.ClassIdImageTitle]})");
                }
            }

            if (_imageSection == null || _imageColumnSection == null || _imageGridSection == null || _imageTitleSection == null)
            {
                Logger.DebugError("Did not parse all sections of template file");
            }
            else
            {
                Logger.DebugLog($"Template file parsed using Schema, all elements found");
            }
        }

        private int ParseTillElementEnds(int startIndex, string elementIdentifier)
        {
            // Keep track of each element we encounter, what element it was an where we found it
            // so that we avoid thinking that out element has ended when it was infact the terminator
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

        private string ParseElement(string line, int index)
        {
            // Strip starting whitespaces and convert to character array
            line = line.TrimStart(' ');
            line = line.TrimStart('\t');
            char[] characters = line.ToCharArray();

            // Check the first character is valid
            if (characters[0] != '<')
            {
                Logger.DebugError($"Line {index + 1} of template file doesn't start with '<', invalid html. check template file and try again.");
                return string.Empty;
                //throw new argumentexception($"line {index + 1} of template file doesn't start with '<', invalid html. check template file and try again.");
            }

            // Parse all leading characters to crudley get the element string (till we hit a whitespace)
            string elementType = "";
            int maxCount = 50;
            int count = 0;
            foreach (var @char in characters)
            {
                if (count > maxCount)
                {
                    Logger.DebugError($"Element too long (50+ characters), bailing. Final element string={elementType}");
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