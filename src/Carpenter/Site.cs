using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Carpenter
{
    /// <summary>
    /// Defines a website or part of a website, stores common options that all SCHEMAs in the child directories of the Site's root directory (Aka Site file location) will use during page generation
    /// </summary>
    public class Site
    {
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
            get { return _optionValues.TryGetValue(Options.Url, out string value) ? value : string.Empty; } 
            set { _optionValues.AddOrUpdate(Options.Url, value); }
        }
        public string TemplatePath 
        {
            get { return _optionValues.TryGetValue(Options.TemplatePath, out string value) ? value : string.Empty; }
            set { _optionValues.AddOrUpdate(Options.TemplatePath, value); }
        }
        public string GridClass 
        {
            get { return _optionValues.TryGetValue(Options.GridClass, out string value) ? value : string.Empty; }
            set { _optionValues.AddOrUpdate(Options.GridClass, value); }
        }
        public string ColumnClass 
        {
            get { return _optionValues.TryGetValue(Options.ColumnClass, out string value) ? value : string.Empty; }
            set { _optionValues.AddOrUpdate(Options.ColumnClass, value); }
        }
        public string ImageClass 
        {
            get { return _optionValues.TryGetValue(Options.ImageClass, out string value) ? value : string.Empty; }
            set { _optionValues.AddOrUpdate(Options.ImageClass, value); }
        }
        public string TitleClass 
        {
            get { return _optionValues.TryGetValue(Options.TitleClass, out string value) ? value : string.Empty; }
            set { _optionValues.AddOrUpdate(Options.TitleClass, value); }
        }

        /// <summary>
        /// Contains all the current site option values
        /// </summary>
        private Dictionary<Options, string> _optionValues = new();

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
                Logger.Log(LogLevel.Error, $"Could not read site file ({ex.GetType()} exception occured)");
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
                        _optionValues[OptionsTable[optionTag]] = line.GetTokenOrOptionValue();
                        Logger.Log(LogLevel.Verbose, $"Site option {OptionsTable[optionTag]}={_optionValues[OptionsTable[optionTag]]}");
                        break;
                    }
                }
            }

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
