using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using JpegMetadataExtractor;

using static Carpenter.Page;

namespace Carpenter
{
    /// [Matthew Carney]
    /// Things I'd like to improve
    /// TODO: - Proper recursion (so we can deal properly with nested tag)
    /// TODO: - Removing duplicate code between GenerateIndex and GeneratePage
    /// TODO: - General performance and allocations
    
    /// <summary>
    /// Generates html for pages (config files that represent pages in a website) and directory pages (link to other pages that share a common parent directory)
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
            { typeof(ImageColumnSection), "layout:image-column" }
        };

        /// <summary>
        /// The number of image elements that will be added to the generated html before the lazy loading parameter is added 
        /// (Added so the first x images will load immediately when the webpage is loaded in a browser, in theory)
        /// </summary>
        private const int kNumImagesWithoutLazyLoading = 4;
        
        /// <summary>
        /// Generates HTML code for a page directory file. A directory file is basically a page that links to other pages which are
        /// contained in child directories.
        /// Unlike page files, directory files have no configuration and are generated at runtime if the option is enabled in the SITE file.
        /// </summary>
        public static void BuildHtmlForPageDirectory(string relativePathToDir, List<Page>? pages, Site? site)
        {
            if (site == null || pages == null)
            {
                throw new ArgumentNullException();
            }

            if (pages.Count == 0)
            {
                throw new ArgumentException("List of provided pages is empty.");
            }
            
            List<Tag> tags = new();
            List<string> generatedContent = new(File.ReadAllLines(site.TemplatePath));
            Dictionary<Tokens, string> tokenValues = new(pages.First().TokenValues); // Use the token values from the first page to fill in the index fields
            Logger.Log(LogLevel.Verbose, $"Generating Directory for \"{relativePathToDir}\"...");
            do
            {
                // Find all tags
                tags = FindTags(generatedContent);
                
                foreach (Tag tag in tags.Where(x => x.Type == "index").OrderByDescending(x => x.ArrayIndex))
                {
                    if (!site.Tags.TryGetValue(tag.Id, out List<string> tagSection))
                    {
                        continue;
                    }

                    int offset = tag.ArrayIndex + 1;
                    string padding = generatedContent[tag.ArrayIndex].Split("<!--").First();
                    if (tag.Id.Contains("foreach:"))
                    {
                        List<string> generateSections = new();
                        foreach (Page page in pages)
                        {
                            Logger.Log(LogLevel.Verbose, $"Adding \"{page.Title}\"...");
                            generateSections.AddRange(GeneratePagePreviewSection(page, site, relativePathToDir, tagSection));
                        }
                        tagSection = generateSections;
                    }

                    foreach (string line in tagSection)
                    {
                        generatedContent.Insert(offset, padding + line);
                        offset++;
                    }
                }
                
                // Clear all tag placeholders
                foreach (Tag tag in tags.OrderByDescending(x => x.ArrayIndex))
                {
                    generatedContent.RemoveAt(tag.ArrayIndex);
                }

                // Populate all tokens
                for (int index = 0; index < generatedContent.Count; index++)
                {
                    foreach (KeyValuePair<string, Tokens> token in Page.TokenTable)
                    {
                        if (tokenValues.Keys.Contains(token.Value))
                        {
                            generatedContent[index] =
                                generatedContent[index].Replace(token.Key, tokenValues[token.Value]);
                        }
                    }
                }
            } while (tags.Count > 0);

            generatedContent.Insert(0, string.Format(Config.kGeneratedComment, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));
            Logger.Log(LogLevel.Info, $"Generated Directory for {pages.Count} pages @ \"{relativePathToDir}\"");
            File.WriteAllLines(Path.Combine(site.GetRootDir() + relativePathToDir, "index.html"), generatedContent);
        }

        /// <summary>
        /// Generates the html for a page file. A Page files is a config file that contains contents and info that will be
        /// generated into a final output html file which can then be uploaded and used on a site.
        /// </summary>
        public static void BuildHtmlForPage(Page? page, Site? site, bool isLocalPreview = false)
        {
            if (site == null || page == null)
            {
                throw new ArgumentNullException();
            }
            
            List<Tag> tags = new();
            Dictionary<Tokens, string> modifiedPageTokens = new(page.TokenValues);
            modifiedPageTokens.AddOrUpdate(Tokens.PageUrl, string.Format("{0}{1}", site.Url, site.GetPageRelativePath(page).Replace("\\", "/")));
            List<string> generatedContent = new(File.ReadAllLines(site.TemplatePath));
            do
            {
                // Find all tags
                tags = FindTags(generatedContent);
                
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
                    string padding = generatedContent[tag.ArrayIndex].Split("<!--").First();
                    if (tag.Id == "page:layout")
                    {
                        tagContents = GenerateLayoutSection(page, site);
                    }
                    
                    tag.Length = padding.Length;
                    foreach (string line in tagContents)
                    {
                        generatedContent.Insert(offset, padding + line);
                        offset++;
                    }
                    
                    tagsOrderedByIndex[index] = tag;
                }
                
                // Clear all tag placeholders
                foreach (Tag tag in FindTags(generatedContent).OrderByDescending(x => x.ArrayIndex))
                {
                    generatedContent.RemoveAt(tag.ArrayIndex + tag.Length);
                }

                // Populate all tokens
                for (int index = 0; index < generatedContent.Count; index++)
                {
                    foreach (KeyValuePair<string, Tokens> token in Page.TokenTable)
                    {
                        if (modifiedPageTokens.Keys.Contains(token.Value))
                        {
                            generatedContent[index] =
                                generatedContent[index].Replace(token.Key, modifiedPageTokens[token.Value]);
                        }
                        else
                        {
                            // Remove the placeholder tag if there was no value for it
                            generatedContent[index] =
                                generatedContent[index].Replace(token.Key, string.Empty);
                        }
                    }
                }
            } while (tags.Count > 0);
            
            // For local previews we go through and remove any page urls so that things can be viewed locally
            if (isLocalPreview)
            {
                string pageUrl = modifiedPageTokens[Tokens.PageUrl] + "/"; // This is a bit of a hack to also remove any trailing slashes in the address
                for (int index = 0; index < generatedContent.Count; index++)
                {
                    string line = generatedContent[index];
                    if (string.IsNullOrEmpty(line))
                    {
                        continue;
                    }

                    // Replace page url
                    line = line.Replace(pageUrl, "");
                    generatedContent[index] = line;
                }
            }
            
            generatedContent.Insert(0, string.Format(Config.kGeneratedComment, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture)));
            Logger.Log(LogLevel.Info, $"Generated HTML for page \"{page.Title}\"");
            File.WriteAllLines(Path.Combine(page.WorkingDirectory(), string.Format("index{0}.html", isLocalPreview ? "_Preview" : "")), generatedContent);
        }

        /// <summary>
        /// Generates HTML for the layout section of a page file. The layout section is the thing that contains the actual content of the page.
        /// </summary>
        /// <param name="page"></param>
        /// <param name="site"></param>
        /// <returns></returns>
        private static List<string> GenerateLayoutSection(Page page, Site site)
        {
            // Generate the layout section!
            List<string> output = new();
            Dictionary<Tokens, string> tokenValues = new();
            RawImageMetadata rawData = new();
            foreach (Section section in page.LayoutSections)
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
                    rawData = JpegParser.GetRawMetadata(Path.Combine(page.WorkingDirectory(), asImageSection.ImageUrl));
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
                        if (string.IsNullOrEmpty(innerImageSection.ImageUrl))
                        {
                            continue;
                        }
                        
                        List<string> imageSectionCopy = new(imageSection);
                        tokenValues.Clear();
                        
                        // Populate the generated contents with image data
                        rawData = JpegParser.GetRawMetadata(Path.Combine(page.WorkingDirectory(), innerImageSection.ImageUrl));
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
        
        /// <summary>
        /// This method replaces placeholder token strings with their equivalent token values in a block of HTML.
        /// Tokens are a way of each page can specify different values that can be used in a page.
        /// </summary>
        private static void ResolveTokens(Dictionary<Tokens, string> tokenValues, ref List<string> section)
        {
            for (int index = 0; index < section.Count; index++)
            {
                foreach (KeyValuePair<string, Tokens> token in Page.TokenTable)
                {
                    if (tokenValues.TryGetValue(token.Value, out string? value))
                    {
                        section[index] = section[index].Replace(token.Key, value);
                    }
                }
            }    
        }
        
        /// <summary>
        /// Finds any Tag strings in a given section of HTML. This placeholder tag will then be replaced with the corresponding
        /// Tag contents from the SITE file.
        /// </summary>
        private static List<Tag> FindTags(List<string> section)
        {
            List<Tag> results = new();
            for (int index = 0; index < section.Count; index++)
            {
                Match tagMatch = Regex.Match(section[index], Config.kTagInHtmlRegexPattern);
                if (tagMatch.Success)
                {
                    Tag foundTag = new() { ArrayIndex = index, Id = tagMatch.Value };
                    tagMatch = Regex.Match(section[index], "(?<=:)(.*?)(?=:)");
                    if (tagMatch.Success) {
                        foundTag.Type = tagMatch.Groups[0].Value;
                    }
                    results.Add(foundTag); 
                    //Logger.Log(LogLevel.Verbose, $"Found tag {content[index].StripWhitespaces()} @ {index}");
                }
            }

            return results;
        }

        /// <summary>
        /// Generates html which previews a given Page. Used when generating an index page which links to multiple child pages.
        /// </summary>
        private static List<string> GeneratePagePreviewSection(Page page, Site site, string relativePath, List<string> contents)
        {
            List<string> generatedContents = new();
            if (page == null || site == null || contents == null || contents.Count == 0)
            {
                return generatedContents;
            }

            // Patch in the correct path to the page based on it's local path relative to where the site file is
            Dictionary<Page.Tokens, string> modifiedPageTokens = new(page.TokenValues);
            modifiedPageTokens[Page.Tokens.PageUrl] = string.Format("{0}{1}/{2}",
                site.Url,
                relativePath.Replace(Path.DirectorySeparatorChar, '/'),
                Path.GetFileName(page.WorkingDirectory()));

            // Get the dimensions for the thumbnail image
            (int width, int height) thumbnailDimensions = new(0, 0);
            if (page.Thumbnail != string.Empty)
            {
                string thumbnailFilename = page.Thumbnail;
                foreach (string imageFile in Directory.EnumerateFiles(page.WorkingDirectory(),
                             "*" + Path.GetExtension(thumbnailFilename),
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

                // Process each line and replace them with values from the page
                foreach (KeyValuePair<string, Page.Tokens> tokenEntry in Page.TokenTable)
                {
                    if (modifiedPageTokens.ContainsKey(tokenEntry.Value) &&
                        !Page.OptionalTokens.Contains(tokenEntry.Value))
                    {
                        processedLine = processedLine.Replace(tokenEntry.Key,
                            modifiedPageTokens[tokenEntry.Value]);
                    }
                }

                generatedContents.Add(processedLine);
            }

            return generatedContents;
        }
    }
}
