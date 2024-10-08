using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Carpenter.Schema;

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
        /// Accessors for Site options
        /// </summary>
        public string Url 
        { 
            get { return OptionValues.TryGetValue(Options.Url, out string value) ? value : string.Empty; } 
            set { OptionValues.AddOrUpdate(Options.Url, value); }
        }
        public string TemplatePath 
        {
            get 
            {
                if (OptionValues.TryGetValue(Options.TemplatePath, out string value))
                {
                    string templatePath = Path.GetDirectoryName(value);
                    if (templatePath == string.Empty)
                    {
                        // Template is in root directory, append site root so it makes sense to anything
                        // that tries to fetch the template location
                        templatePath = Path.Combine(GetPath(), value);
                    }
                    return templatePath;
                }
                else
                {
                    return string.Empty;
                }
            }
            set { OptionValues.AddOrUpdate(Options.TemplatePath, value); }
        }
        public string GridClass 
        {
            get { return OptionValues.TryGetValue(Options.GridClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.GridClass, value); }
        }
        public string ColumnClass 
        {
            get { return OptionValues.TryGetValue(Options.ColumnClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.ColumnClass, value); }
        }
        public string ImageClass 
        {
            get { return OptionValues.TryGetValue(Options.ImageClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.ImageClass, value); }
        }
        public string TitleClass 
        {
            get { return OptionValues.TryGetValue(Options.TitleClass, out string value) ? value : string.Empty; }
            set { OptionValues.AddOrUpdate(Options.TitleClass, value); }
        }

        /// <summary>
        /// Contains all the current site option values
        /// </summary>
        public Dictionary<Options, string> OptionValues = new();

        /// <summary>
        /// The path to the site file 
        /// </summary>
        private string _filePath = string.Empty;

        /// <summary>
        /// Denotes if the Site file was loaded correctly
        /// </summary>
        private bool _loaded = false;


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
        public string GetPath() 
        { 
            return Path.GetDirectoryName(_filePath); 
        }

        /// <summary>
        /// Try and load a site at the given path
        /// </summary>
        /// <param name="pathToSiteFile"></param>
        /// <returns>If the site was successfully loaded or not</returns>
        public bool TryLoad(string path)
        {
            _filePath = Path.Combine(path, Config.kSiteFileName);
            _loaded = false;

            // Try and load the site config file
            string[] fileContents = new string[0];
            try
            {
                fileContents = File.ReadAllLines(_filePath);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Could not read site file ({ex.GetType()} occured)");
                return false;
            }

            // Parse options
            for (int i = 0; i < fileContents.Length; i++)
            {
                string line = fileContents[i];
                foreach (string optionTag in OptionsTable.Keys)
                {
                    if (line.Contains(optionTag))
                    {
                        OptionValues[OptionsTable[optionTag]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Site option {OptionsTable[optionTag]}={OptionValues[OptionsTable[optionTag]]}");
                        break;
                    }
                }
            }

            // We have to strip forward slashes from any urls so we can process them consistently 
            OptionValues[Options.Url] = OptionValues[Options.Url].StripForwardSlashes();

            _loaded = true;
            return true;
        }

        private bool SanityCheck()
        {
            // TODO: implement plz
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of all schemas in the child directories of this site
        /// </summary>
        /// <returns></returns>
        public List<Schema> GetSchemas()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a list of all schemas in the child directories of this site, ordered by the date specified in the schema file
        /// </summary>
        /// <returns></returns>
        public List<Schema> GetSchemasOrderedByDate()
        {
            throw new NotImplementedException();
        }
    }
}
