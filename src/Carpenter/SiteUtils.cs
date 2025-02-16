using JpegMetadataExtractor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Carpenter
{
    public static class SiteUtils
    {
        /// <summary>
        /// Validates all Schemes found at the given path (searches for schemas inside each sub directory) returning the results for each found schema.
        /// </summary>
        /// <param name="path">The root path that contains all schemas, schemas are searched for in directories within this root path.</param>
        /// <param name="results">The validation results for each found schema, ordered by the path to the schema and it's results</param>
        public static void ValidateAllSchemas(
            string path, 
            out List<(string path, SchemaValidator.ValidationResults results)> results,
            Action<bool /** Valid */, string /** Directory Name */, int /** NumProcessed */, int /** Total */> onSchemaValidation)
        {
            results = new List<(string path, SchemaValidator.ValidationResults results)>();
            if (!Directory.Exists(path))
            {
                return;
            }

            string[] directories = Directory.GetDirectories(path);
            for (int index = 0; index < directories.Length; index++)
            {
                string pathToSchema = Path.Combine(path, Path.GetFileName(directories[index]), Config.kSchemaFileName);
                if (!File.Exists(pathToSchema))
                {
                    continue;
                }

                SchemaValidator.Run(new Schema(pathToSchema), out SchemaValidator.ValidationResults schemaResults);
                onSchemaValidation?.Invoke(schemaResults.FailedTests.Count == 0, Path.GetFileName(directories[index]), index, directories.Length);
                results.Add((directories[index], schemaResults));
            }
        }

        /// <summary>
        /// Generates html pages for all schemas found in the root path.
        /// </summary>
        /// <param name="rootPath">Path to site file </param>
        /// <param name="onDirectoryGenerated">Called each a schema is processed (sucessfully or not)</param>
        public static void GenerateAllPagesInSite(
            string rootPath, 
            Action<bool /** Successfully Generated */, string /** Directory Name */, int /** NumProcessed */, int /** Total */> onDirectoryGenerated)
        {
            if (!Directory.Exists(rootPath))
            {
                return;
            }

            Site site = new();
            if (!site.TryLoad(rootPath))
            {
                return;
            }

            // The cache for the JpegParser is not threadsafe, so we have to turn it off
            JpegParser.UseInternalCache = false;

            const int kMaxThreadCount = 10;
            Thread[] threads = new Thread[kMaxThreadCount];
            Object lockObject = new();
            string[] directories = Directory.GetDirectories(rootPath);
            for (int i = 0; i < directories.Length; i++)
            {
                string pathToSchema = Path.Combine(rootPath, Path.GetFileName(directories[i]), Config.kSchemaFileName);
                if (!File.Exists(pathToSchema))
                {
                    continue;
                }
                
                bool schemaProcessed = false;
                while (!schemaProcessed)
                {
                    for (int index = 0; index < kMaxThreadCount; index++)
                    {
                        if (threads[index] == null || !threads[index].IsAlive)
                        {
                            threads[index] = new (() =>
                            {
                                string currentDirectoryPath = Path.GetDirectoryName(pathToSchema);
                                using Schema localSchema = new(pathToSchema);
                                Template template = new(site.TemplatePath);
                                bool wasGeneratedSuccessfully = template.GenerateHtmlForSchema(localSchema, site, currentDirectoryPath);

                                lock (lockObject)
                                {
                                    onDirectoryGenerated?.Invoke(
                                        wasGeneratedSuccessfully, 
                                        currentDirectoryPath,
                                        i,
                                        directories.Length);
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
                        Thread.Sleep(1000);
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

        public static List<string> GetAllUnusedImages(string path)
        {
            List<string> unusedImagePaths = new();
            if (!Directory.Exists(path))
            {
                return unusedImagePaths;
            }

            string[] localDirectories = Directory.GetDirectories(path);
            for (int i = 0; i < localDirectories.Length; i++)
            {
                string localPath = Path.Combine(path, Path.GetFileName(localDirectories[i]));
                string localSchemaPath = Path.Combine(localPath, Config.kSchemaFileName);
                if (File.Exists(localSchemaPath))
                {
                    // try and load the schema in the directory
                    string currentDirectoryPath = Path.GetDirectoryName(localSchemaPath);
                    using Schema localSchema = new(localSchemaPath);

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
                    foreach (string imagePath in Directory.GetFiles(localPath, "*.jpg"))
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

        public static int RemoveAllUnusedImages(string rootPath)
        {
            int count = 0;
            foreach (string imagePath in GetAllUnusedImages(rootPath))
            {
                File.Delete(imagePath);
                count++;
            }

            return count;
        }
    }
}
