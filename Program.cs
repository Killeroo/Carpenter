//https://stackoverflow.com/questions/24643408/how-to-do-on-the-fly-image-compression-in-c
//https://developer.mozilla.org/en-US/docs/Web/CSS/object-fit
//https://stackoverflow.com/questions/19414856/how-can-i-make-all-images-of-different-height-and-width-the-same-via-css

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace PhotoWebpageGenerator
{
    static class Logger
    {
        public static void DebugLog(string message)
        {
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff")}] {message}");
            Console.ResetColor();
        }

        public static void DebugError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff")}] {message}");
            Console.ResetColor();
        }
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
        Image,
        DetailedImage
    };

    public enum SchemaImageTags
    {
        Grid,
        Standalone,
        Column
    }

    public abstract class ImageSection
    {

    }

    public class StandaloneImageSection : ImageSection
    {
        public string Image;
        public string DetailedImage;
    }

    public class ColumnImageSection : ImageSection
    {
        public List<StandaloneImageSection> Sections = new();
    }

    public class Schema
    {
        public readonly Dictionary<string, SchemaTokens> TokenTable = new()
        {
            {"%BASE_URL", SchemaTokens.BaseUrl},
            {"%PAGE_URL", SchemaTokens.PageUrl},
            {"%LOCATION", SchemaTokens.Location},
            {"%TITLE", SchemaTokens.Title},
            {"%MONTH", SchemaTokens.Month},
            {"%YEAR", SchemaTokens.Year},
            {"%CAMERA", SchemaTokens.Camera},
            {"image_grid", SchemaTokens.ClassIdImageGrid},
            {"image_column", SchemaTokens.ClassIdImageColumn},
            {"image_element", SchemaTokens.ClassIdImageElement}
        };

        public readonly Dictionary<string, SchemaTokens> ImageTokenTable = new()
        {
            { "%IMAGE_URL", SchemaTokens.Image },
            { "%DETAILED_IMAGE_URL", SchemaTokens.DetailedImage },
        };

        private readonly Dictionary<string, SchemaImageTags> _tagTable = new()
        {
            {"[IMAGE_LAYOUT]", SchemaImageTags.Grid},
            {"[IMAGES_STANDALONE]", SchemaImageTags.Standalone},
            {"[IMAGES_COLUMN]", SchemaImageTags.Column},
        };

        public Dictionary<SchemaTokens, string> TokenValues = new();
        public List<ImageSection> ImageSections = new();

        public void Load(string path)
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
                return;
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
                return;
            }

            // Ok lets parse everything to the end of the file and construct our image grid
            int currentSectionIndex = -1;
            string imageUrl = string.Empty;
            string detailedImageUrl = string.Empty;
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

                            case SchemaImageTags.Grid:
                                // Ignore this as this is probably the start of our photo grid
                                // TODO: We should worry if we encounter more than one of these
                                break;

                            default:
                                Logger.DebugError("Could not parse tag in photo grid");
                                return;
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

                            default:
                                // TODO: Stronger identification for bad formatted tags
                                Logger.DebugError($"Could not parse token ({token}) in photo grid");
                                return;
                        }

                        // TODO: Make this check more robust so we definitely have 2 sections before adding to section (Maybe parse ahead in array?)
                        // TODO: Don't assume this will be ordered image_url then detailed_image_url
                        if (imageUrl != string.Empty && detailedImageUrl != string.Empty)
                        {
                            if (ImageSections[currentSectionIndex].GetType().Equals(typeof(StandaloneImageSection)))
                            {
                                // If we are dealing with a standalone section
                                var standaloneSection = ImageSections[currentSectionIndex] as StandaloneImageSection;
                                standaloneSection.Image = imageUrl;
                                standaloneSection.DetailedImage = detailedImageUrl;

                                Logger.DebugLog($"Added single image section to photo grid (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }
                            else if (ImageSections[currentSectionIndex].GetType().Equals(typeof(ColumnImageSection)))
                            {
                                // If we are dealing with a column section
                                var columnSection = ImageSections[currentSectionIndex] as ColumnImageSection;
                                columnSection.Sections.Add(new StandaloneImageSection
                                {
                                    Image = imageUrl,
                                    DetailedImage = detailedImageUrl,
                                });

                                Logger.DebugLog($"Added image to column section (image_url={imageUrl} detailed_image_url={imageUrl})");
                            }

                            // Blank the urls again
                            imageUrl = string.Empty;
                            detailedImageUrl = string.Empty;
                        }
                    }
                }
            }

            Logger.DebugLog($"Schema file parsed (\"{path}\")");
        }
    }

    public static class Extensions
    {
        public static string CopyTill(this string str, char till)
        {
            char[] chars = str.ToCharArray();

            string copy = string.Empty;
            int index = 0;
            while (chars[index] != till)
            {
                copy += chars[index];
                index++;
            }

            return copy;
        }

        public static int FindClosestKey(this Dictionary<int, string> dict, int value)
        {
            int closestKey = int.MaxValue;
            foreach (int key in dict.Keys)
            {
                int diff = Math.Abs(value - key);
                if (diff < closestKey)
                {
                    closestKey = key;
                }
            }

            return closestKey;
        }

        public static string[] RemoveSection(this string[] array, int start, int end)
        {
            string[] result = new string[array.Length - (end - start)];

            int destinationCount = 0;
            for (int i = 0; i < array.Length; i++)
            {
                if (i >= start && i <= end)
                {
                    continue;
                }
                result[destinationCount] = array[i];
                destinationCount++;
            }

            return result;
        }

        public static int FindIndexWhichContainsValue(this string[] array, string value)
        {
            for (int i = 0; i < array?.Length; i++)
            {
                string element = array[i];

                if (element.Contains(value))
                {
                    return i;
                }
            }

            return -1;
        }

    }

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

                Length = (EndIndex - StartIndex) + 1;

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

        public string[] FileContents => _fileContents;

        private TemplateSection _imageGridSection;
        private TemplateSection _imageSection;
        private TemplateSection _imageColumnSection;
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

        public void Generate(Schema schema)
        {
            if (!_loaded)
            {
                Logger.DebugError("Cannot generate page without template loaded. Call load() first.");
                return;
            }

            // Parse first to understand the template using the schema 
            // We need to do this before we can generate the file
            Parse(schema);

            // Ok next we need to modify the template and remove the sections we have just passed
            // TODO: Don't assume sections are nested and that photo grid is first...
            string[] templateCopy = _fileContents.RemoveSection(_imageGridSection.StartIndex, _imageGridSection.EndIndex);

            // Next lets build our photo grid
            string[] photoGridElements = GeneratePhotoGrid(schema);

        }

        private string[] GeneratePhotoGrid(Schema schema)
        {
            List<string> photoGridContents = new List<string>();

            photoGridContents.Add(_imageColumnSection.StartLine);

            foreach (ImageSection section in schema.ImageSections)
            {
                if (section.GetType() == typeof(StandaloneImageSection))
                {
                    CreateImageElement(schema, section as StandaloneImageSection, ref photoGridContents);
                }
            }

            photoGridContents.Add(_imageGridSection.EndLine);
            return photoGridContents.ToArray();

        }

        private void CreateImageElement(Schema schema, StandaloneImageSection section, ref List<string> contents)
        {
            //string[] imageElement = _imageSection.Contents;
            //for (int i = 0; i < imageElement.Length; i++)
            //{
            //    string valueToInsert = string.Empty;
            //    foreach (var imageTokenEntry in schema.ImageTokenTable)
            //    {
            //        switch (imageTokenEntry.Value)
            //        {
            //            case SchemaTokens.Image:

            //                break;
            //        }
            //    }
            //    imageElement[i] = imageElement[i].Replace(schema.tok)
            //}
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

            // Loop through the file contents to find each relevant section of the template
            // (The schema contains the class identifiers that represent where everything should go)
            for (int i = 0; i < _fileContents.Length; i++)
            {
                string line = _fileContents[i];

                if (line.Contains($"class=\"{schema.TokenValues[SchemaTokens.ClassIdImageGrid]}\""))
                {
                    // First we need to find of the template that corresponds to our photo grid
                    _imageGridSection = new TemplateSection(this, i);
                } 
                else if (line.Contains($"class=\"{schema.TokenValues[SchemaTokens.ClassIdImageElement]}\""))
                {
                    // Next we need to find the second of the template that makes up the element for our image
                    _imageSection = new TemplateSection(this, i);
                }
                else if (line.Contains($"class=\"{schema.TokenValues[SchemaTokens.ClassIdImageColumn]}\""))
                {
                    // Next we need to know what what our column sections are
                    _imageColumnSection = new TemplateSection(this, i);
                }
            }

            if (_imageSection == null || _imageColumnSection == null || _imageGridSection == null)
            {
                Logger.DebugError("Did not parse all sections of template file");
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
                }
                else
                {
                    count++;
                }

                if (Char.IsWhiteSpace(@char) || @char.Equals('>'))
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


    class Program
    {
        static void Main(string[] args)
        {
            // Load the schema file
            string pathToSchemaFile = "SCHEMA";
            Schema schema = new Schema();
            schema.Load(pathToSchemaFile);

            string pathToTemplateFile = "template.html";
            Template template = new Template();
            template.Load(pathToTemplateFile);
            template.Generate(schema);
        }


    }
}


