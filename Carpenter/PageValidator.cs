using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Carpenter.Page;

namespace Carpenter
{
    public static class PageValidator
    {
        public enum TestImportance
        {
            Required,
            Optional
        }

        internal struct ValidationTest
        {
            public string Name;
            public TestImportance Importance;
            public Func<Page, bool> Test;

            public ValidationTest(string _name, TestImportance _importance, Func<Page, bool> _test)
            {
                Name = _name;
                Test = _test;
                Importance = _importance;
            }
        }

        public struct TestResult
        {
            public string Name;
            public TestImportance Importance;

            public TestResult(string _name, TestImportance _importance)
            {
                Name = _name;
                Importance = _importance;
            }
        }

        public struct ValidationResults
        {
            public List<TestResult> FailedTests = new();
            public List<TestResult> PassedTests = new();

            public ValidationResults() { }

            public override string ToString()
            {
                string outputString = string.Format("{0}/{1} tests passed. {2}",
                    PassedTests.Count,
                    PassedTests.Count + FailedTests.Count,
                    FailedTests.Count > 0 ? "The following tests failed: " : "");

                if (FailedTests.Count > 0)
                {
                    foreach (TestResult testResult in FailedTests.OrderBy(x => x.Importance))
                    {
                        outputString += Environment.NewLine;
                        outputString += string.Format("- [{0}] {1}", testResult.Importance, testResult.Name);
                    }
                }
                return outputString;
            }
        }

        private static List<ValidationTest> Tests = new()
        {
            new ValidationTest("TestAllImagePathsExist", TestImportance.Required, (Page page) =>
            {
                Func<ImageSection, bool> DoImageSectionPathsExist = (ImageSection section) =>
                {
                    bool bExists = true;
                    if (!string.IsNullOrEmpty(section.ImageUrl))
                    {
                        bExists &= File.Exists(Path.Combine(page.WorkingDirectory(), section.ImageUrl));
                    }
                    if (!string.IsNullOrEmpty(section.AltImageUrl))
                    {
                        bExists &= File.Exists(Path.Combine(page.WorkingDirectory(), section.AltImageUrl));
                    }
                    return bExists;
                };

                if (!Directory.Exists(page.WorkingDirectory()))
                    return false;

                foreach (Section section in page.LayoutSections)
                {
                    if (section is ImageSection)
                    {
                        if (!DoImageSectionPathsExist(section as ImageSection))
                        {
                            return false;
                        }
                    }
                    if (section is ImageColumnSection)
                    {
                        ImageColumnSection columnSection = section as ImageColumnSection;
                        foreach (ImageSection image in columnSection.Sections)
                        {
                            if (!DoImageSectionPathsExist(image))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }),
            new ValidationTest("TestPageHasThumbnailSet", TestImportance.Optional, (Page page) =>
            {
                return page.Thumbnail != string.Empty;
            }),
            new ValidationTest("TestForEmptySectionsInColumns", TestImportance.Optional, (Page page) =>
            {
                // This actually doesn't matter because we silently strip out these spaces..
                foreach (Section section in page.LayoutSections)
                {
                    if (section is ImageColumnSection columnSection)
                    {
                        foreach (ImageSection image in columnSection.Sections)
                        {
                            if (string.IsNullOrWhiteSpace(image.ImageUrl) && string.IsNullOrWhiteSpace(image.AltImageUrl))
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }),
            new ValidationTest("TestAllTokensArePresent",  TestImportance.Required, (Page page) =>
            {
                foreach (Tokens token in Enum.GetValues(typeof(Tokens)))
                {
                    if ((token < Tokens.SpecialTokenSection) // We shouldn't expect to find grid specific tokens defines in the Token value tables (they are stored in the layout instead)
                        && page.TokenValues.ContainsKey(token) == false
                        && !Page.OptionalTokens.Contains(token))
                    {
                        return false;
                    }
                }
                return true;
            }),
            new ValidationTest("TestAllOptionsArePresent", TestImportance.Required, (Page page) =>
            {
                foreach (Options option in Enum.GetValues(typeof(Options)))
                {
                    if (page.OptionValues.ContainsKey(option) == false)
                    {
                        return false;
                    }
                }
                return true;
            }),
            new ValidationTest("TestAllTokensHaveValues", TestImportance.Optional, (Page page) =>
            {
                bool testPassed = true;
                foreach (Tokens token in Enum.GetValues(typeof(Tokens)))
                {
                    if (token < Tokens.Image 
                        && !Page.OptionalTokens.Contains(token) 
                        && page.TokenValues.ContainsKey(token))
                    {
                        testPassed &= page.TokenValues[token] != string.Empty;
                    }
                }
                return testPassed;
            }),
            new ValidationTest("TestAllOptionsHaveValues", TestImportance.Optional, (Page page) =>
            {
                bool testPassed = true;
                foreach (Options option in Enum.GetValues(typeof(Options)))
                {
                    if (page.OptionValues.ContainsKey(option))
                    {
                        testPassed &= page.OptionValues[option] != string.Empty;
                    }
                }
                return testPassed;
            }),
            new ValidationTest("PageContainsLayoutSection", TestImportance.Required, (Page page) =>
            {
                if (page.LayoutSections.Count == 0)
                    return false;
                
                // TODO: Could expand this to check if there are any images present in the layout
                return true;
            })
        };

        public static bool Run(Page? pageToTest, out ValidationResults results)
        {
            results = new();
            if (pageToTest == null)
            {
                return false;
            }

            Logger.Log(LogLevel.Verbose, $"Running Validation Tests for Page \"{pageToTest.Title}\"...");
            bool bValidationsPassed = true;
            bool bTestsFailed = false;
            foreach (ValidationTest validation in Tests)
            {
                bool testPassed = validation.Test(pageToTest);
                string passString = testPassed ? "PASSED" : "FAILED";
                Logger.Log(LogLevel.Verbose, $"Test \"{validation.Name}\" ({validation.Importance}): {passString}");
                
                if (!testPassed)
                {
                    bTestsFailed = true;
                    results.FailedTests.Add(new (validation.Name, validation.Importance));
                    if (validation.Importance == TestImportance.Required)
                    {
                        bValidationsPassed = false;
                    }
                }
                else
                {
                    results.PassedTests.Add(new(validation.Name, validation.Importance));
                }
            }
            Logger.Log(!bValidationsPassed ? LogLevel.Error : bTestsFailed ? LogLevel.Warning : LogLevel.Info,
                $"Validation completed for \"{pageToTest.Title}\": {results.ToString()}");

            return bValidationsPassed;
        }
    }
}
