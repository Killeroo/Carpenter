// TODO: Add link previews
// https://andrejgajdos.com/how-to-create-a-link-preview/

using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Carpenter;

namespace Carpenter.CommandLine
{
    class Program
    {

        /**
         * Arguments
         * --site DIR
         * --schema DIR
         *
         * --build
         * --preview
         * --publish
         * --validate
         */

        // TODO: Possible arguments:
        // --schema = Specify one schema to process
        // --template = Specify a template file

        enum Operation
        {
            Preview,
            Build,
            Publish,
            Validate,
        }
        struct CommandLineContext
        {
            public static readonly Dictionary<string, Type> ArgToType = new()
            {
                { typeof(Site).Name.ToLower(), typeof(Site) },
                { typeof(Schema).Name.ToLower(), typeof(Schema) }
            };

            public List<object> FoundObjects;
            public Operation Operation;
            public string FoundPath;

            public CommandLineContext()
            {
                FoundObjects = new();
                Operation = Operation.Publish;
                FoundPath = string.Empty;
            }

            public bool ContainsObject<T>()
            {
                foreach (object obj in FoundObjects)
                {
                    if (obj is T)
                    {
                        return true;
                    }
                }
                return false;
            }

            public bool TryFetchObject<T>(out T? @object)
            {
                foreach (object obj in FoundObjects)
                {
                    if (obj is T objAsType)
                    {
                        @object = objAsType;
                        return true;
                    }
                }

                @object = default(T);
                return true;
            }
        }

        static void Main(string[] args)
        {
            CommandLineContext context = new();
            for (int index = 0; index < args.Length; index++)
            {
                string currArg = args[index];
                if (currArg.StartsWith("--"))
                {
                    string strippedArg = currArg.Substring(2, currArg.Length - 2);
                    if (CommandLineContext.ArgToType.ContainsKey(strippedArg))
                    {
                        if (index + 1 >= args.Length)
                        {
                            throw new ArgumentException($"Could not find path for \"{currArg}\" argument.");
                        }

                        if (!Directory.Exists(args[index + 1]))
                        {
                            throw new AggregateException(
                                $"Path for \"{currArg}\" does not exist. Please specify a valid directory path.");
                        }

                        ConstructorInfo ctor = CommandLineContext.ArgToType[strippedArg]
                            .GetConstructor(new[] { typeof(string) });
                        context.FoundObjects.Add(ctor.Invoke(new object[] { args[++index] }));
                    }
                    else if (Operation.TryParse(strippedArg, false, out Operation outOp))
                    {
                        context.Operation = outOp;
                    }
                }
                else if (string.IsNullOrEmpty(context.FoundPath) && Directory.Exists(args[index]))
                {
                    context.FoundPath = args[index];
                }
            }

            // switch (currArg.Substring(2, currArg.Length))
                    // {
                    //     case "site":
                    //         if (index + 1 < args.Length && Directory.Exists(args[index + 1]))
                    //         {
                    //             context.FoundSite = new Site(args[index + 1]);
                    //         }
                    //         else
                    //         {
                    //             throw new ArgumentException("Could not find argument with Site path");
                    //         }
                    //         break;
                    //     case "schema":
                    //         if (index + 1 < args.Length && Directory.Exists(args[index + 1]))
                    //         {
                    //             context.FoundSchema = new Schema(args[index + 1]);
                    //         }
                    //         else
                    //         {
                    //             throw new ArgumentException("Could not find argument with Schema path");
                    //         }
                    //         break;
                    // }
            //     }
            // }

            if (!context.ContainsObject<Site>())
            {
                // Try and load the site in the current working directory or first found path if one wasn't already loaded
                context.FoundObjects.Add(new Site(string.IsNullOrEmpty(context.FoundPath) ? Environment.CurrentDirectory : context.FoundPath));
            }

            if (context.TryFetchObject(out Site site))
            {
                Console.WriteLine("We have a site!!!");
            }

            switch (context.Operation)
            {
                case Operation.Preview:
                {
                    if (context.ContainsObject<Schema>())
                    {
                        
                    }
                    break;
                }
            }

            // string rootDirectory = string.Empty;
            // if (args.Length != 0)
            // {
            //     rootDirectory = args[0];
            // }
            // else
            // {
            //     rootDirectory = Environment.CurrentDirectory;
            // }
            //
            // // Load the template we will use for all pages, it should be in our root directory
            // string pathToTemplateFile = Path.Combine(rootDirectory, "template.html");
            // HtmlGenerator htmlGenerator;
            // try
            // {
            //     htmlGenerator = new HtmlGenerator(pathToTemplateFile);
            // }
            // catch (Exception ex)
            // {
            //     Logger.Log(LogLevel.Error, $"Exception occured parsing template ({ex.GetType()}) at {pathToTemplateFile}.");
            //     return;
            // }
            //
            // // Now loop through every folder and generate a webpage from the SCHEMA file present in the directory
            // int count = 0;
            // Stopwatch stopwatch = Stopwatch.StartNew();
            // foreach (string directory in Directory.GetDirectories(rootDirectory))
            // {
            //     string currentSchemaPath = Path.Combine(directory, Config.kSchemaFileName);
            //     if (!File.Exists(currentSchemaPath))
            //     {
            //         Logger.Log(LogLevel.Error, $"Could not find ({Config.kSchemaFileName}) at {directory}, skipping..");
            //         continue;
            //     }
            //     else
            //     {
            //         Logger.Log(LogLevel.Verbose, $"Generating page for directory: " + Path.GetDirectoryName(directory));
            //     }
            //
            //     // Load the schema file
            //     
            //     string pathToSchemaFile = Path.Combine(directory, Config.kSchemaFileName);
            //     Schema schema = new Schema();
            //     if (!schema.TryLoad(pathToSchemaFile))
            //     {
            //         Logger.Log(LogLevel.Error, $"Encountered an error parsing schema, skipping..");
            //         continue;
            //     }
            //
            //     // Finally generate the webpage
            //     htmlGenerator.GenerateHtmlForSchema(schema, directory);
            //
            //     count++;
            // }

            // stopwatch.Stop();
            //Logger.Log(LogLevel.Info, $"Website generation completed. {count} pages created in {stopwatch.ElapsedMilliseconds}ms.");
        }
    }
}


