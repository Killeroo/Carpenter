using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Carpenter.Schema;

namespace Carpenter
{
    public static class SchemaValidator
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
            public Func<Schema, bool> Test;

            public ValidationTest(string _name, TestImportance _importance, Func<Schema, bool> _test)
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
                    foreach (TestResult testResult in FailedTests)
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
            new ValidationTest("TestAllImagePathsExist", TestImportance.Required, (Schema schema) =>
            {
                Func<ImageSection, bool> DoImageSectionPathsExist = (ImageSection section) =>
                {
                    bool bExists = true;
                    if (!string.IsNullOrEmpty(section.ImageUrl))
                    {
                        bExists &= File.Exists(Path.Combine(schema._workingDirectory, section.ImageUrl));
                    }
                    if (!string.IsNullOrEmpty(section.AltImageUrl))
                    {
                        bExists &= File.Exists(Path.Combine(schema._workingDirectory, section.AltImageUrl));
                    }
                    return bExists;
                };

                if (!Directory.Exists(schema.WorkingDirectory()))
                    return false;

                foreach (Section section in schema.LayoutSections)
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
            new ValidationTest("TestUrlMatchesLocalPath", TestImportance.Optional, (Schema schema) =>
            {
                return schema.WorkingDirectory().Contains(schema.PageUrl);
            }),
            new ValidationTest("TestSchemaHasThumbnailSet", TestImportance.Optional, (Schema schema) =>
            {
                return schema.Thumbnail != string.Empty;
            }),
            new ValidationTest("TestForEmptySectionsInColumns", TestImportance.Optional, (Schema schema) =>
            {
                // This actually doesn't matter because we silently strip out these spaces..
                foreach (Section section in schema.LayoutSections)
                {
                    if (section is ImageColumnSection)
                    {
                        ImageColumnSection columnSection = section as ImageColumnSection;
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
            new ValidationTest("TestAllTokensArePresent",  TestImportance.Required, (Schema schema) =>
            {
                foreach (Tokens token in Enum.GetValues(typeof(Tokens)))
                {
                    if ((token < Tokens.Image) // We shouldn't expect to find grid specific tokens defines in the Token value tables (they are stored in the layout instead)
                        && schema.TokenValues.ContainsKey(token) == false
                        && !Schema.OptionalTokens.Contains(token))
                    {
                        return false;
                    }
                }
                return true;
            }),
            new ValidationTest("TestAllOptionsArePresent", TestImportance.Required, (Schema schema) =>
            {
                foreach (Options option in Enum.GetValues(typeof(Options)))
                {
                    if (schema.OptionValues.ContainsKey(option) == false)
                    {
                        return false;
                    }
                }
                return true;
            }),
            new ValidationTest("TestAllTokensHaveValues", TestImportance.Optional, (Schema schema) =>
            {
                bool testPassed = true;
                foreach (Tokens token in Enum.GetValues(typeof(Tokens)))
                {
                    if (token < Tokens.Image && !Schema.OptionalTokens.Contains(token) && schema.TokenValues.ContainsKey(token))
                    {
                        testPassed &= schema.TokenValues[token] != string.Empty;
                    }
                }
                return testPassed;
            }),
            new ValidationTest("TestAllOptionsHaveValues", TestImportance.Optional, (Schema schema) =>
            {
                bool testPassed = true;
                foreach (Options option in Enum.GetValues(typeof(Options)))
                {
                    if (schema.OptionValues.ContainsKey(option))
                    {
                        testPassed &= schema.OptionValues[option] != string.Empty;
                    }
                }
                return testPassed;
            }),
            new ValidationTest("SchemaContainsLayoutSection", TestImportance.Required, (Schema schema) =>
            {
                if (schema.LayoutSections.Count == 0)
                    return false;
                
                // TODO: Could expand this to check if there are any images present in the layout
                return true;
            })
        };

        public static bool Run(Schema schemaToTest, out ValidationResults results)
        {
            results = new();
            if (schemaToTest == null)
            {
                return false;
            }

            bool bValidationsPassed = true;
            foreach (ValidationTest validation in Tests)
            {
                if (!validation.Test(schemaToTest))
                {
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

            return bValidationsPassed;
        }
    }
}
