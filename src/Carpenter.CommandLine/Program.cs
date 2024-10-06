// TODO: Add link previews
// https://andrejgajdos.com/how-to-create-a-link-preview/

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Drawing.Imaging;
using System.Drawing;
using Carpenter;

namespace Carpenter.CommandLine
{
    class Program
    {
        // TODO: Possible arguments:
        // --schema = Specify one schema to process
        // --template = Specify a template file

        private class TimerScope : IDisposable
        {
            public TimeSpan Elapsed => _stopwatch.Elapsed;

            private Stopwatch _stopwatch = new();
            private string _name = string.Empty;

            public TimerScope(string timerName)
            {
                _stopwatch.Restart();
                _name = timerName;
            }

            public void Dispose()
            {
                _stopwatch.Stop();
            }

            public override string ToString() 
            {
                return string.Format("{0} [{1}ms]", _name, Elapsed.Milliseconds);
            }
        }

        static void Main(string[] args)
        {
            string rootDirectory = string.Empty;
            if (args.Length != 0)
            {
                rootDirectory = args[0];
            }
            else
            {
                rootDirectory = Environment.CurrentDirectory;
            }

            rootDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos";
            string schemaDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos\central-london-1";
            //string schemaDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos\test";

            Console.WriteLine($"Carpenter v{Config.kVersion} - Static photo webpage generator");


            Site site = new();
            Template template = new();
            Schema schema = new();

            File.ReadAllLines("C:\\Path\\LilSyncy.exe");
            File.ReadAllLines("C:\\Users\\Kelpie\\Desktop\\WebsiteConversion\\photos\\SITE");
            File.ReadAllLines(Path.Combine(rootDirectory, "SITE"));
            File.ReadAllLines(Path.Combine(schemaDirectory, "SCHEMA"));

            using (TimerScope siteLoadTimer = new("Site.TryLoad"))
            {
                if (site.TryLoad(rootDirectory) == false)
                {
                    Console.WriteLine("Failed to read site");
                    return;
                }
                Console.WriteLine(siteLoadTimer);
            }

            using (TimerScope templateLoadTimer = new("Template.TryLoad"))
            {
                if (template.TryLoad(site.TemplatePath) == false)
                {
                    Console.WriteLine("Failed to read Template");
                    return;
                }
                Console.WriteLine(templateLoadTimer);
            }

            using (TimerScope schemaLoadTimer = new("Schema.TryLoad"))
            {
                if (schema.TryLoad(Path.Combine(schemaDirectory, Config.kSchemaFileName)) == false)
                {
                    Console.WriteLine("Failed to read schema");
                    return;
                }
                Console.WriteLine(schemaLoadTimer);
            }
            Console.WriteLine();

            using (TimerScope schemaPreviewGenerationTimer = new("Template.GeneratePreviewHtmlForSchema"))
            {
                if (template.GeneratePreviewHtmlForSchema(schema, site, schemaDirectory, out string previewFilename) == false)
                {
                    Console.WriteLine("Failed to generate preview");
                    return;
                }
                Console.WriteLine(schemaPreviewGenerationTimer);
            }
            using (TimerScope schemaPreviewGenerationTimer = new("Template.GenerateHtmlForSchema"))
            {
                if (template.GenerateHtmlForSchema(schema, site, schemaDirectory) == false)
                {
                    Console.WriteLine("Failed to generate webpage");
                    return;
                }
                Console.WriteLine(schemaPreviewGenerationTimer);
            }

            //// Load the template we will use for all pages, it should be in our root directory
            //string pathToTemplateFile = Path.Combine(rootDirectory, "template.html");
            //Template template;
            //try
            //{
            //    template = new Template(pathToTemplateFile);
            //}
            //catch (Exception ex)
            //{
            //    Logger.Log(LogLevel.Error, $"Exception occured parsing template ({ex.GetType()}) at {pathToTemplateFile}.");
            //    return;
            //}

            //// Now loop through every folder and generate a webpage from the SCHEMA file present in the directory
            //int count = 0;
            //Stopwatch stopwatch = Stopwatch.StartNew();
            //foreach (string directory in Directory.GetDirectories(rootDirectory))
            //{
            //    string currentSchemaPath = Path.Combine(directory, Config.kSchemaFileName);
            //    if (!File.Exists(currentSchemaPath))
            //    {
            //        Logger.Log(LogLevel.Error, $"Could not find ({Config.kSchemaFileName}) at {directory}, skipping..");
            //        continue;
            //    }
            //    else
            //    {
            //        Logger.Log(LogLevel.Verbose, $"Generating page for directory: " + Path.GetDirectoryName(directory));
            //    }

            //    // Load the schema file
            //    string pathToSchemaFile = Path.Combine(directory, Config.kSchemaFileName);
            //    Schema schema = new Schema();
            //    if (!schema.TryLoad(pathToSchemaFile))
            //    {
            //        Logger.Log(LogLevel.Error, $"Encountered an error parsing schema, skipping..");
            //        continue;
            //    }

            //    // Finally generate the webpage
            //    template.GenerateHtmlForSchema(schema, directory);

            //    count++;
            //}

            //stopwatch.Stop();
            //Logger.Log(LogLevel.Info, $"Website generation completed. {count} pages created in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}


