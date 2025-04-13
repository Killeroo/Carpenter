using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using JpegMetadataExtractor;
using static Carpenter.Schema;

namespace Carpenter
{
    /// [Matthew Carney]
    /// Things I'd like to improve
    /// TODO: - Better naming and comments
    /// TODO: - Proper recursion (so we can deal properly with nested tag)
    /// TODO: - Removing duplicate code between GenerateIndex and GeneratePage
    /// TODO: - General performance and allocations
    
    /// <summary>
    /// Generates html for a different parts of the end site (schema and the index files that list them all
    /// </summary>
    public static class HtmlGenerator
    {
        /// <summary>
        /// Placeholder tag, marks a place in the template which will get replaced with a section of text specified
        /// in the Site config file. 
        /// </summary>
        private struct Tag
        {
            /// <summary>
            /// Tag identifier, just the raw string that the tag uses in the template file
            /// </summary>
            public string Id;
            
            /// <summary>
            /// The parent type of tag, a common string used by multiple tags. Used for generating specific types of pages.
            /// For example, Tags with Type 'Index' will only be used when generating index pages, 'Page' tags will only be used
            /// when generating normal pages
            /// </summary>
            public string Type;
            
            /// <summary>
            /// The index where the tag is located in the file contents array.
            /// </summary>
            public int ArrayIndex;

            /// <summary>
            /// The length of the tag's contents after it has been inserted into the template file.
            /// </summary>
            public int Length;
        }
        
        private static readonly Dictionary<Type, string> LayoutTypeToTagName = new()
        {
            { typeof(TitleSection), "layout:title" },
            { typeof(ImageSection), "layout:image-standalone" },
            { typeof(ImageColumnSection), "layout:image-column" },
        };

        /// <summary>
        /// The number of image elements that will be added to the generated html before the lazy loading parameter is added 
        /// (Added so the first x images will load immediately when the webpage is loaded in a browser, in theory)
        /// </summary>
        private const int kNumImagesWithoutLazyLoading = 4;

        /// <exception cref="ArgumentNullException">Thrown when any provided object is null</exception>
        /// <exception cref="ArgumentException">Thrown if a provided argument is not setup correctly</exception>
        /// <exception cref="IOException">Thrown when trying to read a template file or write a generated file</exception>
        public static void BuildHtmlForIndexDirectory(string relativePathToDir, List<Schema>? schemas, Site? site)
        {
            if (site == null || schemas == null)
            {
                throw new ArgumentNullException();
            }

            if (schemas.Count == 0)
            {
                throw new ArgumentException("List of provided schemas is empty.");
            }
            
            List<Tag> tags = new();
            List<string> generatedContents = new(File.ReadAllLines(site.TemplatePath));
            Dictionary<Tokens, string> tokenValues = new(schemas.First().TokenValues); // Use the token values from the first page to fill in the index fields
            Logger.Log(LogLevel.Verbose, $"Generating Index for \"{relativePathToDir}\"...");
            do
            {
                // Find all tags
                tags = FindTags(generatedContents);
                
                foreach (Tag tag in tags.Where(x => x.Type == "index").OrderByDescending(x => x.ArrayIndex))
                {
                    if (!site.Tags.TryGetValue(tag.Id, out List<string> tagSection))
                    {
                        continue;
                    }

                    int offset = tag.ArrayIndex + 1;
                    string padding = generatedContents[tag.ArrayIndex].Split("<!--").First();
                    if (tag.Id.Contains("foreach:"))
                    {
                        List<string> generateSections = new();
                        foreach (Schema schema in schemas)
                        {
                            Logger.Log(LogLevel.Verbose, $"Adding \"{schema.Title}\"...");
                            generateSections.AddRange(GenerateSchemaSection(schema, site, relativePathToDir, tagSection));
                        }
                        tagSection = generateSections;
                    }

                    foreach (string line in tagSection)
                    {
                        generatedContents.Insert(offset, padding + line);
                        offset++;
                    }
                }
                
                // Clear all tag placeholders
                foreach (Tag tag in tags.OrderByDescending(x => x.ArrayIndex))
                {
                    generatedContents.RemoveAt(tag.ArrayIndex);
                }

                // Populate all tokens
                for (int index = 0; index < generatedContents.Count; index++)
                {
                    foreach (KeyValuePair<string, Tokens> token in Schema.TokenTable)
                    {
                        if (tokenValues.Keys.Contains(token.Value))
                        {
                            generatedContents[index] =
                                generatedContents[index].Replace(token.Key, tokenValues[token.Value]);
                        }
                    }
                }
            } while (tags.Count > 0);

            Logger.Log(LogLevel.Info, $"Generated Index Page for {schemas.Count} schemas @ \"{relativePathToDir}\"");
            File.WriteAllLines(Path.Combine(site.GetRootDir() + relativePathToDir, "index.html"), generatedContents);
        }

        public static void BuildHtmlForSchema(Schema? schema, Site? site, bool isLocalPreview = false)
        {
            if (site == null || schema == null)
            {
                throw new ArgumentNullException();
            }
            
            List<Tag> tags = new();
            Dictionary<Tokens, string> modifiedSchemaTokens = new(schema.TokenValues);
            modifiedSchemaTokens.AddOrUpdate(Tokens.PageUrl, string.Format("{0}{1}", site.Url, site.GetSchemaRelativePath(schema).Replace("\\", "/")));
            List<string> generatedContents = new(File.ReadAllLines(site.TemplatePath));
            do
            {
                // Find all tags
                tags = FindTags(generatedContents);
                
                List<Tag> tagsOrderedByIndex = tags.Where(x => x.Type == "page").OrderByDescending(x => x.ArrayIndex).ToList();
                for (int index = 0; index < tagsOrderedByIndex.Count; index++)
                {
                    Tag tag = tagsOrderedByIndex[index];
                    if (!site.Tags.TryGetValue(tag.Id, out List<string> tagContents)
                        && tag.Id != "page:layout")
                    {
                        continue;
                    }

                    int offset = tag.ArrayIndex + 1;
                    string padding = generatedContents[tag.ArrayIndex].Split("<!--").First();
                    if (tag.Id == "page:layout")
                    {
                        tagContents = GenerateLayoutSection(schema, site);
                    }
                    
                    tag.Length = padding.Length;
                    foreach (string line in tagContents)
                    {
                        generatedContents.Insert(offset, padding + line);
                        offset++;
                    }
                    
                    tagsOrderedByIndex[index] = tag;
                }
                
                // Clear all tag placeholders
                foreach (Tag tag in FindTags(generatedContents).OrderByDescending(x => x.ArrayIndex))
                {
                    generatedContents.RemoveAt(tag.ArrayIndex + tag.Length);
                }

                // Populate all tokens
                for (int index = 0; index < generatedContents.Count; index++)
                {
                    foreach (KeyValuePair<string, Tokens> token in Schema.TokenTable)
                    {
                        if (modifiedSchemaTokens.Keys.Contains(token.Value))
                        {
                            generatedContents[index] =
                                generatedContents[index].Replace(token.Key, modifiedSchemaTokens[token.Value]);
                        }
                    }
                }
            } while (tags.Count > 0);
            
            // For local previews we go through and remove any page urls so that things can be viewed locally
            if (isLocalPreview)
            {
                string pageUrl = modifiedSchemaTokens[Tokens.PageUrl] + "/"; // This is a bit of a hack to also remove any trailing slashes in the address
                for (int index = 0; index < generatedContents.Count; index++)
                {
                    string line = generatedContents[index];
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // Replace page url
                    line = line.Replace(pageUrl, "");
                    generatedContents[index] = line;
                }
            }

            Logger.Log(LogLevel.Info, $"Generated HTML for schema \"{schema.Title}\"");
            File.WriteAllLines(Path.Combine(schema.WorkingDirectory(), string.Format("index{0}.html", isLocalPreview ? "_Preview" : "")), generatedContents);
        }

        private static List<string> GenerateLayoutSection(Schema schema, Site site)
        {
            // Generate the layout section!
            List<string> output = new();
            Dictionary<Tokens, string> tokenValues = new();
            RawImageMetadata rawData = new();
            foreach (Section section in schema.LayoutSections)
            {
                if (!site.Tags.TryGetValue(LayoutTypeToTagName[section.GetType()], out List<string> sectionContents))
                {
                    Debug.Assert(false); // We want to know when this fails;
                    continue;
                }
                
                tokenValues.Clear();
                List<string> sectionContentsCopy = new(sectionContents); // TODO: Hate this, remove the amount of duplicates
                if (section is TitleSection asTitleSection)
                {
                    tokenValues.Add(Tokens.GridTitle, asTitleSection.TitleText);
                    ResolveTokens(tokenValues, ref sectionContentsCopy);
                }
                else if (section is ImageSection asImageSection)
                {
                    rawData = JpegParser.GetRawMetadata(Path.Combine(schema.WorkingDirectory(), asImageSection.ImageUrl));
                    tokenValues.Add(Tokens.ImageWidth, rawData.FrameData.Width.ToString());
                    tokenValues.Add(Tokens.ImageHeight, rawData.FrameData.Height.ToString());
                    tokenValues.Add(Tokens.Image, asImageSection.ImageUrl);
                    tokenValues.Add(Tokens.AlternateImage, asImageSection.AltImageUrl);
                    ResolveTokens(tokenValues, ref sectionContentsCopy);
                } 
                else if (section is ImageColumnSection asImageColumnSection)
                {
                    List<Tag> sectionTags = FindTags(sectionContentsCopy);
                    sectionTags = sectionTags.Where(x => x.Type.Contains("foreach")).ToList();
                    Debug.Assert(sectionTags.Count > 0);

                    // We only care about the first foreach tag we find.
                    // Ideally we want to be able to resolve any tag recursively but that is for later.
                    if (!site.Tags.TryGetValue(LayoutTypeToTagName[typeof(ImageSection)], out List<string> imageSection))
                    {
                        continue;
                    }
                    int innerOffset = sectionTags.First().ArrayIndex + 1;
                    string padding = sectionContentsCopy[sectionTags.First().ArrayIndex].Split("<!--").First();
                    foreach (ImageSection innerImageSection in asImageColumnSection.Sections)
                    {
                        List<string> imageSectionCopy = new(imageSection);
                        tokenValues.Clear();
                        
                        // Populate the generated contents with iamge data
                        rawData = JpegParser.GetRawMetadata(Path.Combine(schema.WorkingDirectory(), innerImageSection.ImageUrl));
                        tokenValues.Add(Tokens.ImageWidth, rawData.FrameData.Width.ToString());
                        tokenValues.Add(Tokens.ImageHeight, rawData.FrameData.Height.ToString());
                        tokenValues.Add(Tokens.Image, innerImageSection.ImageUrl);
                        tokenValues.Add(Tokens.AlternateImage, innerImageSection.AltImageUrl); // TODO: Don't assume this is the same as non alt image
                        ResolveTokens(tokenValues, ref imageSectionCopy);
                        
                        foreach (string line in imageSectionCopy)
                        {
                            sectionContentsCopy.Insert(innerOffset, padding + line);
                            innerOffset++;
                        }
                        
                        // Technically we should remove any tags here but because we aren't doing this using proper recursion
                        // we just leave it and let the main look in GeneratePage clean up the unused tags
                    }
                }
                
                output.AddRange(sectionContentsCopy);
            }
            
            return output;
        }
        
        // TODO: Rename everything here, too much 'content'
        private static void ResolveTokens(Dictionary<Tokens, string> tokenValues,
            ref List<string> content)
        {
            for (int index = 0; index < content.Count; index++)
            {
                foreach (KeyValuePair<string, Tokens> token in Schema.TokenTable)
                {
                    if (tokenValues.TryGetValue(token.Value, out string? value))
                    {
                        content[index] = content[index].Replace(token.Key, value);
                    }
                }
            }    
        }
        
        private static List<Tag> FindTags(List<string> content)
        {
            List<Tag> results = new();
            for (int index = 0; index < content.Count; index++)
            {
                Match tagMatch = Regex.Match(content[index], Config.kTagInHtmlRegexPattern);
                if (tagMatch.Success)
                {
                    Tag foundTag = new() { ArrayIndex = index, Id = tagMatch.Value };
                    tagMatch = Regex.Match(content[index], "(?<=:)(.*?)(?=:)");
                    if (tagMatch.Success) {
                        foundTag.Type = tagMatch.Groups[0].Value;
                    }
                    results.Add(foundTag); 
                    //Logger.Log(LogLevel.Verbose, $"Found tag {content[index].StripWhitespaces()} @ {index}");
                }
            }

            return results;
        }

        private static List<string> GenerateSchemaSection(Schema schema, Site site, string relativePath, List<string> contents)
        {
            List<string> generatedContents = new();
            if (schema == null || site == null || contents == null || contents.Count == 0)
            {
                return generatedContents;
            }

            // Patch in the correct path to the schema based on it's local path relative to where the site file is
            Dictionary<Schema.Tokens, string> modifiedSchemaTokens = new(schema.TokenValues);
            modifiedSchemaTokens[Schema.Tokens.PageUrl] = string.Format("{0}{1}/{2}", 
                site.Url,
                relativePath.Replace("\\", "/"),
                site.GetSchemaRelativePath(schema));

            // Get the dimensions for the thumbnail image
            (int width, int height) thumbnailDimensions = new(0, 0);
            if (schema.Thumbnail != string.Empty)
            {
                string thumbnailFilename = schema.Thumbnail;
                foreach (string imageFile in Directory.EnumerateFiles(schema.WorkingDirectory(),
                             string.Format("*.{0}", Path.GetExtension(thumbnailFilename)),
                             SearchOption.AllDirectories))
                {
                    if (Path.GetFileName(imageFile) == thumbnailFilename)
                    {
                        ImageMetadata thumbnailData = JpegParser.GetMetadata(imageFile);
                        thumbnailDimensions = new(thumbnailData.Width, thumbnailData.Height);
                        break;
                    }
                }
            }

            foreach (string line in contents)
            {
                string processedLine = line;

                // We still need to manually find and replace the image and width height tags
                if ((thumbnailDimensions.width != 0 && thumbnailDimensions.height != 0)
                    && (line.Contains(Config.kTemplateImageWidthToken) ||
                        line.Contains(Config.kTemplateImageHeightToken)))
                {
                    processedLine = processedLine.Replace(Config.kTemplateImageWidthToken,
                        thumbnailDimensions.width.ToString());
                    processedLine = processedLine.Replace(Config.kTemplateImageHeightToken,
                        thumbnailDimensions.height.ToString());
                }

                // Process each line and replace them with values from the schema
                foreach (KeyValuePair<string, Schema.Tokens> tokenEntry in Schema.TokenTable)
                {
                    if (modifiedSchemaTokens.ContainsKey(tokenEntry.Value) &&
                        !Schema.OptionalTokens.Contains(tokenEntry.Value))
                    {
                        processedLine = processedLine.Replace(tokenEntry.Key,
                            modifiedSchemaTokens[tokenEntry.Value]);
                    }
                }

                generatedContents.Add(processedLine);
            }

            return generatedContents;
        }
    }
}
