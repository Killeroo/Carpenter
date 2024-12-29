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
using System.Runtime.CompilerServices;

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
            public string Name => _name;

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
            //// TODO: Point at example project
            //string rootDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos";
            //string schemaDirectory = @"C:\Users\Kelpie\Desktop\WebsiteConversion\photos\donegal-3";
            string rootDirectory = @"G:\My Drive\Website\matthewcarney.info\photos";
            string schemaDirectory = @"G:\My Drive\Website\matthewcarney.info\photos\donegal-4";
            string tempPath = Path.Combine(Path.GetTempPath(), "Carpenter", "Benchmark");
            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }

            Console.WriteLine($"Carpenter v{Config.kVersion} - Static photo webpage generator");

            Logger.EnableLevel(LogLevel.Info, false);

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
                WriteTimerScopeToConsole(siteLoadTimer);
            }

            using (TimerScope templateLoadTimer = new("Template.TryLoad"))
            {
                if (template.TryLoad(site.TemplatePath) == false)
                {
                    Console.WriteLine("Failed to read Template");
                    return;
                }
                WriteTimerScopeToConsole(templateLoadTimer);
            }

            using (TimerScope schemaLoadTimer = new("Schema.TryLoad"))
            {
                if (schema.TryLoad(Path.Combine(schemaDirectory, Config.kSchemaFileName)) == false)
                {
                    Console.WriteLine("Failed to read schema");
                    return;
                }
                WriteTimerScopeToConsole(schemaLoadTimer);
            }

            using (TimerScope schemaPreviewGenerationTimer = new("Template.GeneratePreviewHtmlForSchema"))
            {
                if (template.GeneratePreviewHtmlForSchema(schema, site, schemaDirectory, out string previewFilename) == false)
                {
                    Console.WriteLine("Failed to generate preview");
                    return;
                }
                WriteTimerScopeToConsole(schemaPreviewGenerationTimer);
            }
            using (TimerScope schemaPreviewGenerationTimer = new("Template.GenerateHtmlForSchema"))
            {
                if (template.GenerateHtmlForSchema(schema, site, schemaDirectory) == false)
                {
                    Console.WriteLine("Failed to generate webpage");
                    return;
                }
                WriteTimerScopeToConsole(schemaPreviewGenerationTimer);
            }
            using (TimerScope schemaPreviewGenerationTimer = new("Schema.TrySave"))
            {
                if (schema.TrySave(tempPath) == false)
                {
                    Console.WriteLine("Failed to save schema");
                    return;
                }
                WriteTimerScopeToConsole(schemaPreviewGenerationTimer);
            }

        }

        static List<Tuple<int, ConsoleColor>> TimeRanges = new()
            {
                new (10, ConsoleColor.Green),
                new (50, ConsoleColor.Yellow),
                new (100, ConsoleColor.Red),
            };

        static void WriteTimerScopeToConsole(TimerScope scopeToWrite)
        {
            ConsoleColor Color = ConsoleColor.Magenta;
            foreach (var Range in TimeRanges)
            {
                if (scopeToWrite.Elapsed.Milliseconds < Range.Item1)
                {
                    Color = Range.Item2;
                    break;
                }
            }

            ConsoleWrite(Color, $"[{scopeToWrite.Elapsed.Milliseconds}ms] ");
            ConsoleWriteLine(ConsoleColor.DarkGray, scopeToWrite.Name);
        }

        static void ConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor previousColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = previousColor;
        }

        static void ConsoleWriteLine(ConsoleColor color, string text)
        {
            ConsoleWrite(color, text + Environment.NewLine);
        }

    }
}


