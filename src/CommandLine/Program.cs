﻿// TODO: Add link previews
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
        private const string VersionString = "v2.0";

        private const string TemplateFilename = "template.html";
        private const string SchemaFilename = "SCHEMA";

        // TODO: Possible arguments:
        // --schema = Specify one schema to process
        // --template = Specify a template file

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

            Console.WriteLine($"Carpenter {VersionString} - Static photo webpage generator");

            // Load the template we will use for all pages, it should be in our root directory
            string pathToTemplateFile = Path.Combine(rootDirectory, "template.html");
            Template template;
            try
            {
                template = new Template(pathToTemplateFile);
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, $"Exception occured parsing template ({ex.GetType()}) at {pathToTemplateFile}.");
                return;
            }

            // Now loop through every folder and generate a webpage from the SCHEMA file present in the directory
            int count = 0;
            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (string directory in Directory.GetDirectories(rootDirectory))
            {
                string currentSchemaPath = Path.Combine(directory, SchemaFilename);
                if (!File.Exists(currentSchemaPath))
                {
                    Logger.Log(LogLevel.Error, $"Could not find ({SchemaFilename}) at {directory}, skipping..");
                    continue;
                }
                else
                {
                    Logger.Log(LogLevel.Verbose, $"Generating page for directory: " + Path.GetDirectoryName(directory));
                }

                // Load the schema file
                string pathToSchemaFile = Path.Combine(directory, "SCHEMA");
                Schema schema = new Schema();
                if (!schema.Load(pathToSchemaFile))
                {
                    Logger.Log(LogLevel.Error, $"Encountered an error parsing schema, skipping..");
                    continue;
                }

                // Finally generate the webpage
                template.Generate(schema, directory);

                count++;
            }

            stopwatch.Stop();
            Logger.Log(LogLevel.Info, $"Website generation completed. {count} pages created in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}


