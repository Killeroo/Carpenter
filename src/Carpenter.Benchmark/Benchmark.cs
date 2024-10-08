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
using System.ComponentModel;

using Carpenter;

namespace Carpenter.Tests
{
    /// <summary>
    /// A super simple console application that tests loading a simple Carpenter site.
    /// Used for benchmarking and sanity checking changes.
    /// </summary>
    class Benchmark
    {
        /// <summary>
        /// A simple class that allows us to convieniently time code that it wraps around
        /// </summary>
        /// <remarks>
        /// Probably not super accurate, but accurate enough
        /// </remarks>
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
            // TODO: Point at example project
            string rootDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos";
            string schemaDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos\central-london-1";

            Console.WriteLine($"Carpenter v{Config.kVersion} - Static photo webpage generator");

            Site site = new();
            Template template = new();
            Schema schema = new();

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

            // TODO: Next try saving!

        }
    }
}


