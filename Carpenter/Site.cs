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
    /// Defines a website or part of a website, stores common options that all PAGEs in the child directories of the Site's root directory (Aka Site file location) will use during page generation
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
        }

        /// <summary>
        /// A set of all the option tag strings for the site with their corresponding enum values
        /// </summary>
        public static readonly Dictionary<string, Options> OptionsTable = new()
        {
            { "site_url", Options.Url },
            { "template_path", Options.TemplatePath }
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
            get { return OptionValues.TryGetValue(Options.Url, out string? value) ? value : string.Empty; }
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
        /// Returns the path to the page file relative to the root directory of the site.
        /// </summary>
        public string GetPageRelativePath(Page page)
        {
            if (_loaded == false || _configFilePath == string.Empty)
            {
                return string.Empty;
            }

            return page.WorkingDirectory().Replace(Path.GetDirectoryName(_configFilePath), String.Empty);
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
        /// Returns a list of all pages in the child directories of this site
        /// </summary>
        /// <returns></returns>
        public List<Page> GetPages()
        {
            string siteRootPath = GetRootDir();
            if (!_loaded || !Directory.Exists(siteRootPath))
            {
                return new List<Page>();
            }

            List<Page> foundPages = new();
            foreach (string path in GetPathsToPages())
            {
                foundPages.Add(new Page(path));
            }

            return foundPages;
        }

        /// <summary>
        /// Returns a list of all directory paths that contain files in the site
        /// </summary>
        public List<string> GetPathsToPages()
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
                string pathToPage = Path.Combine(subDir, Config.kPageFileName);
                if (File.Exists(pathToPage))
                {
                    paths.Add(pathToPage);
                }
            }
            
            return paths;
        }

        /// <summary>
        /// Returns a list of all pages in the child directories of this site, ordered by the date specified in the page file
        /// </summary>
        /// <returns></returns>
        public List<Page> GetPagesOrderedByDate()
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

            // Order pages by date
            Dictionary<DateTime, Page> pagesWithDate = new();
            foreach (Page page in GetPages())
            {
                DateTime date = new(
                    Convert.ToInt32(page.TokenValues[Page.Tokens.Year]),
                    MonthStringToInt[page.TokenValues[Page.Tokens.Month].ToLower()],
                    1);

                while (pagesWithDate.ContainsKey(date))
                {
                    date = date.AddDays(1);
                }
                pagesWithDate.Add(date, page);
            }
            return pagesWithDate.OrderByDescending(x => x.Key).Select(y => y.Value).ToList();
        }
        
        public bool Publish()
        {
            if (!ValidateAllPages(out List<(string path, PageValidator.ValidationResults results)> _)) {
                return false;
            }
            GenerateAllPages();
            GenerateIndexPages();
            RemoveAllUnusedImages();
            return true;
        }
        
        /// <summary>
        /// Validates all Schemes found at the given path (searches for pages inside each sub directory) returning the results for each found page.
        /// </summary>
        /// <param name="path">The root path that contains all pages, pages are searched for in directories within this root path.</param>
        /// <param name="results">The validation results for each found page, ordered by the path to the page and it's results</param>
        public bool ValidateAllPages(
            out List<(string path, PageValidator.ValidationResults results)> results,
            Action<bool /** Valid */, string /** Directory Name */, int /** NumProcessed */, int /** Total */>? onPageValidation = null)
        {
            results = new List<(string path, PageValidator.ValidationResults results)>();
            List<Page> pageToValidate = GetPages();
            if (!_loaded || pageToValidate.Count == 0)
            {
                return false;
            }
            
            int index = 0;
            bool passed = true;
            foreach (Page page in GetPages())
            {
                passed &= PageValidator.Run(page, out PageValidator.ValidationResults pageResults);
                onPageValidation?.Invoke(pageResults.FailedTests.Count == 0, Path.GetFileName(page.WorkingDirectory()), index++, pageToValidate.Count);
                results.Add((page.WorkingDirectory(), pageResults));
            }

            return passed;
        }

        /// <summary>
        /// Generates html pages for all pages found in the root path.
        /// </summary>
        /// <param name="rootPath">Path to site file </param>
        /// <param name="onDirectoryGenerated">Called each a page is processed (sucessfully or not)</param>
        public void GenerateAllPages(
            Action<bool /** Successfully Generated */, string /** Directory Name */, int /** NumProcessed */, int /** Total */>? onDirectoryGenerated = null)
        {
            if (!_loaded)
            {
                return;
            }

            // The cache for the JpegParser is not threadsafe, so we have to turn it off
            JpegParser.UseInternalCache = false;

            const int kMaxThreadCount = 10;
            Thread[] threads = new Thread[kMaxThreadCount];
            Object lockObject = new();
            List<string> pagePaths = GetPathsToPages();
            int processed = 0;
            Logger.Log(LogLevel.Info, $"Generating HTML for {pagePaths.Count} Pages with {kMaxThreadCount} threads...");
            foreach (string pagePath in pagePaths)
            {
                bool pageProcessed = false;
                Logger.Log(LogLevel.Verbose, $"Processing \"{pagePath}\"...");
                while (!pageProcessed)
                {
                    for (int index = 0; index < kMaxThreadCount; index++)
                    {
                        if (threads[index] == null || !threads[index].IsAlive)
                        {
                            threads[index] = new (() =>
                            {
                                string currentDirectoryPath = Path.GetDirectoryName(pagePath);
                                using Page localPage = new(pagePath);
                                HtmlGenerator.BuildHtmlForPage(localPage, this);

                                lock (lockObject)
                                {
                                    onDirectoryGenerated?.Invoke(
                                        true, // TODO: Lol Nah 
                                        currentDirectoryPath,
                                        processed++,
                                        pagePaths.Count);
                                }
                                
                                Logger.Log(LogLevel.Verbose, $"Thread finished for path: {pagePath}");
                            });
                            threads[index].IsBackground = true;
                            threads[index].Start();
                            pageProcessed = true;
                            Logger.Log(LogLevel.Verbose, "Starting generator thread...");
                            break;
                        }
                    }

                    if (!pageProcessed)
                    {
                        Logger.Log(LogLevel.Verbose, "All threads in used. Waiting for one to become available...");
                        Thread.Sleep(100);
                    }
                }
            }

            // Wait for threads to finish
            bool threadStillRunning = true;
            while (threadStillRunning)
            {
                Logger.Log(LogLevel.Verbose, "=============================");
                threadStillRunning = false;
                int index = 0;
                foreach (Thread thread in threads)
                {
                    threadStillRunning |= thread != null && thread.IsAlive;
                    Logger.Log(LogLevel.Verbose, $"Thread {index++} running: {threadStillRunning} (null={thread == null} alive={thread.IsAlive})");
                }
                Thread.Sleep(200);
            }
            
            Logger.Log(LogLevel.Info, "Finished generating HTML for all Pages.");
        }

        /// <summary>
        /// Returns the paths to all images in the given path that are not referenced by any pages
        /// </summary>
        /// <param name="path">Path to root of site</param>
        public List<string> GetAllUnusedImages()
        {
            List<string> unusedImagePaths = new();
            string siteRootPath = GetRootDir();
            if (!_loaded || !Directory.Exists(siteRootPath))
            {
                return unusedImagePaths;
            }

            string[] localDirectories = Directory.GetDirectories(siteRootPath);
            foreach (string path in GetPathsToPages())
            {
                if (File.Exists(path))
                {
                    // try and load the page in the directory
                    using Page localPage = new(path);

                    // Construct a list of all referenced images in the page so we can
                    // work out what files aren't referenced
                    List<string> referencedImages = new List<string>();
                    foreach (Section section in localPage.LayoutSections)
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

                    // Loop through and find any files that aren't referenced in the page 
                    foreach (string imagePath in Directory.GetFiles(Path.GetDirectoryName(path), "*.jpg"))
                    {
                        string imageName = Path.GetFileName(imagePath);
                        if (referencedImages.Contains(imageName) == false)
                        {
                            unusedImagePaths.Add(imagePath);
                            Logger.Log(LogLevel.Verbose, $"Found unreferenced image: {imageName}");
                        }
                    }
                }
            }

            Logger.Log(LogLevel.Verbose, $"Found {unusedImagePaths.Count} unused images");
            return unusedImagePaths;
        }

        /// <summary>
        /// Remove all images that are not referenced by pages in the site
        /// </summary>
        /// <param name="rootPath">Root path of site</param>
        /// <returns>Number of images that were removed</returns>
        public int RemoveAllUnusedImages()
        {
            int count = 0;
            foreach (string imagePath in GetAllUnusedImages())
            {
                try
                {
                    File.Delete(imagePath);
                    Logger.Log(LogLevel.Verbose, $"Deleted image: {imagePath}");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    Logger.Log(LogLevel.Warning, $"Failed to delete image: {imagePath} [{e.GetType()}]");
                }
                count++;
            }

            Logger.Log(LogLevel.Info, $"Removed {count} unused images from Site");
            return count;
        }

        /// <summary>
        /// Generates index pages for all directories that contains pages in child directories
        /// </summary>
        public void GenerateIndexPages()
        {
            string siteRootPath = GetRootDir();
            if (!_loaded || !Directory.Exists(siteRootPath))
            {
                return;
            }
            
            // An index file is basically any path that contains multiple pages in it's child directories
            Dictionary<string, List<Page>> foundIndexDirectories = new();
            foreach (Page page in GetPagesOrderedByDate())
            {
                if (page != null)
                {
                    string pageParentDir = Path.GetDirectoryName(page.WorkingDirectory()).Replace(siteRootPath, "");
                    if (foundIndexDirectories.ContainsKey(pageParentDir)) {
                        foundIndexDirectories[pageParentDir].Add(page);
                    } else {
                        foundIndexDirectories.Add(pageParentDir, new List<Page> { page });
                    }
                }
            }

            foreach (KeyValuePair<string, List<Page>> indexDir in foundIndexDirectories)
            {
                HtmlGenerator.BuildHtmlForPageDirectory(indexDir.Key, indexDir.Value, this);
            }
        }
    }
}
