﻿using System;
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
using JpegMetadataExtractor;

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
            string rootDirectory = @"G:\My Drive\Website\photos.matthewcarney.net\";
            string schemaDirectory = @"G:\My Drive\Website\photos.matthewcarney.net\other\archive\donegal-3";
            string tempPath = Path.Combine(Path.GetTempPath(), "Carpenter", "Benchmark");
            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }

            Console.WriteLine($"Carpenter v{Config.kVersion} - Static photo webpage generator");

            Logger.SetLogLevel(LogLevel.Info);

            Site site = new();
            Schema schema = new();

            Stopwatch stopwatch = Stopwatch.StartNew();
            {
                if (site.TryLoad(rootDirectory) == false)
                {
                    Console.WriteLine("Failed to read site");
                    return;
                }
            }
            WriteTimerToConsole(stopwatch, "Site.TryLoad");
            
            stopwatch = Stopwatch.StartNew();
            {
                site.GenerateIndexPages();
            }
            WriteTimerToConsole(stopwatch, "Site.GenerateIndexPages");
            
            stopwatch = Stopwatch.StartNew();
            {
                site.GenerateAllSchemas((_, _, processed, total) => {Console.Write("Generating pages... [{0}/{1}]\r", processed, total);});
            }
            WriteTimerToConsole(stopwatch, "Site.GenerateAllPagesInSite");
            
            stopwatch = Stopwatch.StartNew();
            {
                if (schema.TryLoad(Path.Combine(schemaDirectory, Config.kSchemaFileName)) == false)
                {
                    Console.WriteLine("Failed to read schema");
                    return;
                }
            }
            WriteTimerToConsole(stopwatch, "Schema.TryLoad");

            JpegParser.UseInternalCache = true;
            JpegParser.CacheSize = 100;
            JpegParser.ClearCache();
            stopwatch = Stopwatch.StartNew();
            {
                HtmlGenerator.BuildHtmlForSchema(schema, site);
            }
            WriteTimerToConsole(stopwatch, "Template.GeneratePage");
            
            stopwatch = Stopwatch.StartNew();
            {
                HtmlGenerator.BuildHtmlForSchema(schema, site);
            }
            WriteTimerToConsole(stopwatch, "Template.GeneratePage (w/ cached image data)");
            
            stopwatch = Stopwatch.StartNew();
            {
                if (schema.TrySave(tempPath) == false)
                {
                    Console.WriteLine("Failed to save schema");
                    return;
                }
            }
            WriteTimerToConsole(stopwatch, "Schema.TrySave");

            stopwatch = Stopwatch.StartNew();
            {
                if (SchemaValidator.Run(schema, out SchemaValidator.ValidationResults results) == false)
                {
                    Console.WriteLine("Validation failed!");
                }
                //if (results.FailedTests.Count > 0)
                //{
                //    Console.WriteLine(results);
                //}
            }
            WriteTimerToConsole(stopwatch, "SchemaValidator.Run");
        }

        static List<Tuple<int, ConsoleColor>> TimeRanges = new()
            {
                new (10, ConsoleColor.Green),
                new (50, ConsoleColor.Yellow),
                new (100, ConsoleColor.Red)
            };

        static void WriteTimerToConsole(Stopwatch stopwatch, string name)
        {
            ConsoleColor Color = ConsoleColor.Magenta;
            foreach (var Range in TimeRanges)
            {
                if (stopwatch.ElapsedMilliseconds < Range.Item1)
                {
                    Color = Range.Item2;
                    break;
                }
            }

            ConsoleWrite(Color, $"[{stopwatch.ElapsedMilliseconds}ms] ");
            ConsoleWriteLine(ConsoleColor.DarkGray, name);
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


