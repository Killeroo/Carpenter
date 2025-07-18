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
            string rootDir = @"C:\Projects\Carpenter\example-site\";
            string pageDir = @"C:\Projects\Carpenter\example-site\places\ireland\sheephaven-bay-1";
            string tempPath = Path.Combine(Path.GetTempPath(), "Carpenter", "Benchmark");
            if (Directory.Exists(tempPath) == false)
            {
                Directory.CreateDirectory(tempPath);
            }

            Console.WriteLine($"Carpenter v{Config.kVersion} - Static photo webpage generator");
            Logger.SetLogLevel(LogLevel.Info);

            Site site = new();
            Page page = new();

            Stopwatch stopwatch = Stopwatch.StartNew();
            {
                if (site.TryLoad(rootDir) == false)
                {
                    Console.WriteLine("Failed to read site");
                    return;
                }
            }
            WriteTimerToConsole(stopwatch, "Site.TryLoad");
            
            // Copy over a placeholder images for each image in the site.
            // We don't store these locally because they are a lot of data but we can't process the site without them..
            const string kPlaceholderImageName = "placeholder_image.jpg";
            string placeholderImagePath = Path.Combine(rootDir, kPlaceholderImageName);
            if (File.Exists(placeholderImagePath))
            {
                var something = GetSiteImagePaths(site);
                foreach (string path in something)
                {
                    File.Copy(placeholderImagePath, path, true);
                }
            }
            Console.WriteLine("Placeholder images copied.");
            
            stopwatch = Stopwatch.StartNew();
            {
                site.GenerateIndexPages();
            }
            WriteTimerToConsole(stopwatch, "Site.GenerateIndexPages");
            
            stopwatch = Stopwatch.StartNew();
            {
                site.GenerateAllPages((_, _, processed, total) => {Console.Write("Generating pages... [{0}/{1}]\r", processed, total);});
            }
            WriteTimerToConsole(stopwatch, "Site.GenerateAllPagesInSite");
            
            stopwatch = Stopwatch.StartNew();
            {
                page.TryLoad(Path.Combine(pageDir, Config.kPageFileName));
            }
            WriteTimerToConsole(stopwatch, "Page.TryLoad");

            JpegParser.UseInternalCache = true;
            JpegParser.CacheSize = 100;
            JpegParser.ClearCache();
            stopwatch = Stopwatch.StartNew();
            {
                HtmlGenerator.BuildHtmlForPage(page, site);
            }
            WriteTimerToConsole(stopwatch, "Template.GeneratePage");
            
            stopwatch = Stopwatch.StartNew();
            {
                HtmlGenerator.BuildHtmlForPage(page, site);
            }
            WriteTimerToConsole(stopwatch, "Template.GeneratePage (w/ cached image data)");
            
            stopwatch = Stopwatch.StartNew();
            {
                if (page.TrySave(tempPath) == false)
                {
                    Console.WriteLine("Failed to save page");
                    return;
                }
            }
            WriteTimerToConsole(stopwatch, "Page.TrySave");

            stopwatch = Stopwatch.StartNew();
            {
                if (PageValidator.Run(page, out PageValidator.ValidationResults results) == false)
                {
                    Console.WriteLine("Validation failed!");
                }
                //if (results.FailedTests.Count > 0)
                //{
                //    Console.WriteLine(results);
                //}
            }
            WriteTimerToConsole(stopwatch, "PageValidator.Run");
            
            // Remove the placeholder images when we are done
            foreach (string path in GetSiteImagePaths(site))
            {
                File.Delete(path);
            }
            Console.WriteLine("Placeholder images removed.");
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

        static List<string> GetSiteImagePaths(Site site)
        {
            List<string> imagePaths = new();

            void StoreImagePath(string pagePath, ImageSection section)
            {
                imagePaths.Add(Path.Combine(pagePath, section.ImageUrl));
                if (!string.IsNullOrEmpty(section.AltImageUrl) && section.ImageUrl != section.AltImageUrl)
                {
                    imagePaths.Add(Path.Combine(pagePath, section.AltImageUrl));
                }
            }
            
            foreach (Page page in site.GetPages())
            {
                foreach (Section section in page.LayoutSections)
                {
                    if (section is ImageColumnSection asColumnSection)
                    {
                        foreach (ImageSection imageSection in asColumnSection.Sections)
                        {
                            StoreImagePath(page.WorkingDirectory(), imageSection);
                        }
                    }
                    else if (section is ImageSection asImageSection)
                    {
                        StoreImagePath(page.WorkingDirectory(), asImageSection);
                    }
                }
            }

            return imagePaths;
        }
        
    }
}


