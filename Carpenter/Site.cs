using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using JpegMetadataExtractor;

namespace Carpenter
{
    /// <summary>
    /// Defines a website or part of a website, stores common options that all SCHEMAs in the child directories of the Site's root directory (Aka Site file location) will use during page generation
    /// </summary>
    public class Site
    {
        private const string kOptionsTag = "[OPTIONS]";
        private const string kClassesTag = "[CLASS_IDS]";

        /// <summary>
        /// All the options that a Site file can contain
        /// </summary>
        public enum Options
        {
            Url,
            TemplatePath,
            GridClass,
            ColumnClass,
            ImageClass,
            TitleClass
        }

        /// <summary>
        /// A set of all the option tag strings for the site with their corresponding enum values
        /// </summary>
        public static readonly Dictionary<string, Options> OptionsTable = new()
        {
            { "site_url", Options.Url },
            { "template_path", Options.TemplatePath },
            { "grid_class", Options.GridClass },
            { "column_class", Options.ColumnClass },
            { "image_class", Options.ImageClass },
            { "title_class", Options.TitleClass }
        };

        /// <summary>
        /// Contains all the current site option values
        /// </summary>
        public Dictionary<Options, string> OptionValues = new();
        
        /// <summary>
        /// Contains all tags specified in the Site config file
        /// </summary>
        public Dictionary<string, List<string>> Tags = new();

        /// <summary>
        /// The path to the site config file 
        /// </summary>
        private string _configFilePath = string.Empty;

        /// <summary>
        /// Denotes if the Site file was loaded correctly
        /// </summary>
        private bool _loaded = false;

        /// <summary>
        /// Accessors for Site options
        /// </summary>
        public string Url {
            get { return OptionValues.TryGetValue(Options.Url, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.Url, value); }
        }
        public string TemplatePath {
            get {
                if (OptionValues.TryGetValue(Options.TemplatePath, out string value))
                {
                    string templatePath = Path.GetDirectoryName(value);
                    if (templatePath == string.Empty)
                    {
                        // Template is in root directory, append site root so it makes sense to anything
                        // that tries to fetch the template location
                        return Path.Combine(GetRootDir(), value);
                    }
                    return value;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { OptionValues.AddOrUpdate(Options.TemplatePath, value); }
        }
        public string GridClass {
            get { return OptionValues.TryGetValue(Options.GridClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.GridClass, value); }
        }
        public string ColumnClass {
            get { return OptionValues.TryGetValue(Options.ColumnClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.ColumnClass, value); }
        }
        public string ImageClass {
            get { return OptionValues.TryGetValue(Options.ImageClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.ImageClass, value); }
        }
        public string TitleClass {
            get { return OptionValues.TryGetValue(Options.TitleClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.TitleClass, value); }
        }


        /// <summary>
        /// Was the Site file was successfully read and loaded
        /// </summary>
        /// <returns></returns>
        public bool IsLoaded() => _loaded;

        public Site() { }
        public Site(string rootPath) => TryLoad(rootPath);

        /// <summary>
        /// Gets the current root path of the Site
        /// </summary>
        /// <returns></returns>
        public string GetRootDir()
        {
            return Path.GetDirectoryName(_configFilePath);
        }

        /// <summary>
        /// Returns the path to the schema file relative to the root directory of the site.
        /// </summary>
        public string GetSchemaRelativePath(Schema schema)
        {
            if (_loaded == false || _configFilePath == string.Empty)
            {
                return string.Empty;
            }

            return schema.WorkingDirectory().Replace(_configFilePath, String.Empty);
        }

        /// <summary>
        /// Try and load a site at the given path
        /// </summary>
        /// <param name="pathToSiteFile"></param>
        /// <exception cref="KeyNotFoundException"></exception>
        /// <returns>If the site was successfully loaded or not</returns>
        public bool TryLoad(string path)
        {
            _configFilePath = Path.Combine(path, Config.kSiteFileName);
            _loaded = false;

            // Try and load the site config file
            string[] fileContents = new string[0];
            try
            {
                fileContents = File.ReadAllLines(_configFilePath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Could not read site file ({ex.GetType()} occured)");
                return false;
            }

            // Parse options
            {
                for (int i = 0; i < fileContents.Length; i++)
                {
                    string line = fileContents[i];
                    foreach (string optionTag in OptionsTable.Keys)
                    {
                        if (line.Contains(optionTag))
                        {
                            OptionValues[OptionsTable[optionTag]] = line.GetTokenOrOptionValue();
                            Logger.Log(LogLevel.Verbose,
                                $"Site option {OptionsTable[optionTag]}={OptionValues[OptionsTable[optionTag]]}");
                            break;
                        }
                    }
                }
            }

            // We have to strip forward slashes from any urls so we can process them consistently 
            OptionValues[Options.Url] = OptionValues[Options.Url].StripForwardSlashes();
            
            // Parse tags
            {
                bool FindPatternInLine(string line, string pattern, out string tag)
                {
                    Match tagMatch = Regex.Match(line, pattern);
                    tag = tagMatch.Success ? tagMatch.Value : string.Empty;
                    return tagMatch.Success;
                }
                
                for (int i = 0; i < fileContents.Length; i++)
                {
                    if (FindPatternInLine(fileContents[i], Config.kSiteConfigTagPattern, out string foundTag))
                    {
                        if (Tags.TryAdd(foundTag, new List<string>()))
                        {
                            while (i + 1 < fileContents.Length && !FindPatternInLine(fileContents[i + 1], Config.kSiteConfigSectionPattern, out string _) && i < fileContents.Length)
                            {
                                i++;
                                Tags[foundTag].Add(fileContents[i]);
                            }
                        }
                    }
                }
            }

            if (IsValid())
            {
                Logger.Log(LogLevel.Verbose, $"Site parsed ({_configFilePath})");
                _loaded = true;
                return true;
            }
            else
            {
                Logger.Log(LogLevel.Error, $"Site failed sanity check");
                Reset();
                return false;
            }
        }

        /// <summary>
        /// Attempts to save the current site to a file.
        /// </summary>
        /// <returns>Whether the site was successfully saved or not</returns>
        public bool TrySave(string path)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Checks that the site file contains all the tokens and values that it should contain
        /// </summary>
        /// <returns>If the site contents are valid or not</returns>
        private bool IsValid()
        {
            foreach (Options option in Enum.GetValues(typeof(Options)))
            {
                if (OptionValues.ContainsKey(option) == false)
                {
                    Logger.Log(LogLevel.Error, $"Sanity check failure! Option {option.ToString()} was not found in site's option values.");
                    return false;
                }
            }
            if (string.IsNullOrEmpty(_configFilePath))
            {
                Logger.Log(LogLevel.Error, $"Sanity check failure! Site does not contain a valid file path");
                return false;
            }
            return true;
        }

        /// <summary>
        /// Clears the contents and values of the site file
        /// </summary>
        private void Reset()
        {
            OptionValues.Clear();
            _configFilePath = string.Empty;
            _loaded = false;
        }

        /// <summary>
        /// Returns a list of all schemas in the child directories of this site
        /// </summary>
        /// <returns></returns>
        public List<Schema> GetSchemas()
        {
            string siteRootPath = GetRootDir();
            if (!_loaded || !Directory.Exists(siteRootPath))
            {
                return new List<Schema>();
            }

            List<Schema> foundSchemas = new();
            foreach (string path in GetPathsToSchemas())
            {
                foundSchemas.Add(new Schema(path));
            }

            return foundSchemas;
        }

        /// <summary>
        /// Returns a list of all directory paths that contain files in the site
        /// </summary>
        public List<string> GetPathsToSchemas()
        {
            string siteRootPath = GetRootDir();
            if (!_loaded || !Directory.Exists(siteRootPath))
            {
                return new List<string>();
            }
            
            List<string> paths = new();
            string[] dirs = Directory.GetDirectories(siteRootPath, "*", SearchOption.AllDirectories);
            foreach (string subDir in dirs)
            {
                string pathToSchema = Path.Combine(subDir, Config.kSchemaFileName);
                if (File.Exists(pathToSchema))
                {
                    paths.Add(pathToSchema);
                }
            }
            
            return paths;
        }

        /// <summary>
        /// Returns a list of all schemas in the child directories of this site, ordered by the date specified in the schema file
        /// </summary>
        /// <returns></returns>
        public List<Schema> GetSchemasOrderedByDate()
        {
            Dictionary<string, int> MonthStringToInt = new()
            {
                { "january", 1 },
                { "february", 2 },
                { "march", 3 },
                { "april", 4 },
                { "may", 5 },
                { "june", 6 },
                { "july", 7 },
                { "august", 8 },
                { "september", 9 },
                { "october", 10 },
                { "november", 11 },
                { "december", 12 }
            };

            // Order schemas by date
            List<Schema> schemasOrderedByDate;
            Dictionary<DateTime, Schema> schemasWithDate = new();
            foreach (Schema schema in GetSchemas())
            {
                DateTime date = new(
                    Convert.ToInt32(schema.TokenValues[Schema.Tokens.Year]),
                    MonthStringToInt[schema.TokenValues[Schema.Tokens.Month].ToLower()],
                    1);

                while (schemasWithDate.ContainsKey(date))
                {
                    date = date.AddDays(1);
                }
                schemasWithDate.Add(date, schema);
            }
            return schemasWithDate.OrderByDescending(x => x.Key).Select(y => y.Value).ToList();
        }
        
        /// <summary>
        /// Validates all Schemes found at the given path (searches for schemas inside each sub directory) returning the results for each found schema.
        /// </summary>
        /// <param name="path">The root path that contains all schemas, schemas are searched for in directories within this root path.</param>
        /// <param name="results">The validation results for each found schema, ordered by the path to the schema and it's results</param>
        public void ValidateAllSchemas(
            out List<(string path, SchemaValidator.ValidationResults results)> results,
            Action<bool /** Valid */, string /** Directory Name */, int /** NumProcessed */, int /** Total */> onSchemaValidation)
        {
            results = new List<(string path, SchemaValidator.ValidationResults results)>();
            List<Schema> schemasToValidate = GetSchemas();
            if (!_loaded || schemasToValidate.Count == 0)
            {
                return;
            }
            
            int index = 0;
            foreach (Schema schema in GetSchemas())
            {
                SchemaValidator.Run(schema, out SchemaValidator.ValidationResults schemaResults);
                onSchemaValidation?.Invoke(schemaResults.FailedTests.Count == 0, Path.GetFileName(schema.WorkingDirectory()), index++, schemasToValidate.Count);
                results.Add((schema.WorkingDirectory(), schemaResults));
            }
        }

        /// <summary>
        /// Generates html pages for all schemas found in the root path.
        /// </summary>
        /// <param name="rootPath">Path to site file </param>
        /// <param name="onDirectoryGenerated">Called each a schema is processed (sucessfully or not)</param>
        public void GenerateAllPagesInSite(
            Action<bool /** Successfully Generated */, string /** Directory Name */, int /** NumProcessed */, int /** Total */>? onDirectoryGenerated)
        {
            string siteRootPath = GetRootDir();
            if (!_loaded || Directory.Exists(siteRootPath))
            {
                return;
            }

            // The cache for the JpegParser is not threadsafe, so we have to turn it off
            JpegParser.UseInternalCache = false;

            const int kMaxThreadCount = 10;
            Thread[] threads = new Thread[kMaxThreadCount];
            Object lockObject = new();
            List<string> schemaPaths = GetPathsToSchemas();
            int processed = 0;
            foreach (string schemaPath in schemaPaths)
            {
                bool schemaProcessed = false;
                while (!schemaProcessed)
                {
                    for (int index = 0; index < kMaxThreadCount; index++)
                    {
                        if (threads[index] == null || !threads[index].IsAlive)
                        {
                            threads[index] = new (() =>
                            {
                                string currentDirectoryPath = Path.GetDirectoryName(schemaPath);
                                using Schema localSchema = new(schemaPath);
                                Template template = new(TemplatePath);
                                bool wasGeneratedSuccessfully = template.GenerateHtmlForSchema(localSchema, this, currentDirectoryPath);

                                lock (lockObject)
                                {
                                    onDirectoryGenerated?.Invoke(
                                        wasGeneratedSuccessfully, 
                                        currentDirectoryPath,
                                        processed++,
                                        schemaPaths.Count);
                                }
                            });
                            threads[index].IsBackground = true;
                            threads[index].Start();
                            schemaProcessed = true;
                            break;
                        }
                    }

                    if (!schemaProcessed)
                    {
                        Thread.Sleep(100);
                    }
                }
            }

            // Wait for threads to finish
            bool threadStillRunning = true;
            while (threadStillRunning)
            {
                threadStillRunning = false;
                foreach (Thread thread in threads)
                {
                    threadStillRunning |= thread != null && thread.IsAlive;
                }
                Thread.Sleep(200);
            }

            return;
        }

        /// <summary>
        /// Returns the paths to all images in the given path that are not referenced by any schemas
        /// </summary>
        /// <param name="path">Path to root of site</param>
        public List<string> GetAllUnusedImages()
        {
            List<string> unusedImagePaths = new();
            string siteRootPath = GetRootDir();
            if (!_loaded || Directory.Exists(siteRootPath))
            {
                return unusedImagePaths;
            }

            string[] localDirectories = Directory.GetDirectories(siteRootPath);
            foreach (string path in GetPathsToSchemas())
            {
                if (File.Exists(path))
                {
                    // try and load the schema in the directory
                    using Schema localSchema = new(path);

                    // Construct a list of all referenced images in the schema so we can
                    // work out what files aren't referenced
                    List<string> referencedImages = new List<string>();
                    foreach (Section section in localSchema.LayoutSections)
                    {
                        if (section is ImageColumnSection)
                        {
                            ImageColumnSection columnSection = section as ImageColumnSection;
                            foreach (ImageSection innerImage in columnSection.Sections)
                            {
                                referencedImages.Add(innerImage.ImageUrl);
                                if (!string.IsNullOrEmpty(innerImage.AltImageUrl))
                                {
                                    referencedImages.Add(innerImage.AltImageUrl);
                                }
                            }
                        }
                        else if (section is ImageSection)
                        {
                            ImageSection standaloneSection = section as ImageSection;
                            referencedImages.Add(standaloneSection.ImageUrl);
                            if (!string.IsNullOrEmpty(standaloneSection.AltImageUrl))
                            {
                                referencedImages.Add(standaloneSection.AltImageUrl);
                            }
                        }
                    }

                    // Loop through and find any files that aren't referenced in the schema 
                    foreach (string imagePath in Directory.GetFiles(path, "*.jpg"))
                    {
                        string imageName = Path.GetFileName(imagePath);
                        if (referencedImages.Contains(imageName) == false)
                        {
                            unusedImagePaths.Add(imagePath);
                        }
                    }
                }
            }

            return unusedImagePaths;
        }

        /// <summary>
        /// Remove all images that are not referenced by schemas in the site
        /// </summary>
        /// <param name="rootPath">Root path of site</param>
        /// <returns>Number of images that were removed</returns>
        public int RemoveAllUnusedImages()
        {
            int count = 0;
            foreach (string imagePath in GetAllUnusedImages())
            {
                File.Delete(imagePath);
                count++;
            }

            return count;
        }

        /// <summary>
        /// Generates index pages for all directories that contains schemas in child directories
        /// </summary>
        public void GenerateIndexPages()
        {
            string siteRootPath = GetRootDir();
            if (!_loaded || !Directory.Exists(siteRootPath))
            {
                return;
            }

            if (!File.Exists(TemplatePath))
            {
                return;
            }
            Template template = new(TemplatePath);
            
            // An index file is basically any path that contains multiple schemas in it's child directories
            Dictionary<string, List<Schema>> foundIndexDirectories = new();
            foreach (Schema schema in GetSchemasOrderedByDate())
            {
                if (schema != null)
                {
                    string schemaParentDir = Path.GetDirectoryName(schema.WorkingDirectory()).Replace(siteRootPath, "");
                    if (foundIndexDirectories.ContainsKey(schemaParentDir)) {
                        foundIndexDirectories[schemaParentDir].Add(schema);
                    } else {
                        foundIndexDirectories.Add(schemaParentDir, new List<Schema> { schema });
                    }
                }
            }

            foreach (KeyValuePair<string, List<Schema>> indexDir in foundIndexDirectories)
            {
                template.GenerateIndex(indexDir.Key, indexDir.Value, this);
            }

        }
    }
}
