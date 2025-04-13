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

        enum Operation
        {
            Preview,
            Build,
            Publish,
            Validate
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
                return false;
            }
        }

        static void Main(string[] args)
        {
            Logger.SetLogLevel(LogLevel.Info);
            
            CommandLineContext context = new();
            try
            {
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
                                throw new ArgumentException(
                                    $"Path for \"{currArg}\" does not exist. Please specify a valid directory path.");
                            }

                            ConstructorInfo? ctor = CommandLineContext.ArgToType[strippedArg].GetConstructor(new[] { typeof(string) });
                            if (ctor != null)
                            {
                                context.FoundObjects.Add(ctor.Invoke(new object[] { args[++index] }));
                            }
                        }
                        else if (Operation.TryParse(strippedArg, true, out Operation outOp))
                        {
                            context.Operation = outOp;
                        }
                    }
                    else if (string.IsNullOrEmpty(context.FoundPath) && Directory.Exists(args[index]))
                    {
                        context.FoundPath = args[index];
                    }
                }
            }
            catch (ArgumentException e)
            {
                Console.WriteLine(e);
                throw;
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

            if (!context.TryFetchObject(out Site? site) || site == null)
            {
                ErrorAndExit("Could not find a valid site. Please check arguments and try again.");
            }

            switch (context.Operation)
            {
                case Operation.Preview:
                {
                    if (context.TryFetchObject(out Schema schema))
                    {
                        HtmlGenerator.BuildHtmlForSchema(schema, site, true);
                    }
                    else
                    {
                        ErrorAndExit("Cannot generate preview without a schema. Please specify one with '--schema <path>' and try again.");
                    }
                    
                    break;
                }
                case Operation.Build:
                {
                    if (context.TryFetchObject(out Schema schema))
                    {
                        HtmlGenerator.BuildHtmlForSchema(schema, site);
                    }
                    else
                    {
                        site.GenerateAllSchemas();
                    }
                    break;
                }
                case Operation.Publish:
                {
                    if (site.Publish())
                    {
                        Logger.Log(LogLevel.Info, "Site published.");
                    }
                    else
                    {
                        ErrorAndExit("Could not publish site.");
                    }
                    break;
                }
                case Operation.Validate:
                {
                    if (context.TryFetchObject(out Schema schema))
                    {
                        SchemaValidator.Run(schema, out SchemaValidator.ValidationResults results);
                        Logger.Log(LogLevel.Info, $"Validation results:\n{results.ToString()}");
                    }
                    else
                    {
                        if (site.ValidateAllSchemas(out List<(string path, SchemaValidator.ValidationResults results)> siteResults))
                        {
                            Logger.Log(LogLevel.Info, "Site validated.");
                        }

                        foreach ((string path, SchemaValidator.ValidationResults results) in siteResults)
                        {
                            Console.WriteLine($"Results for {path}");
                            Console.WriteLine("====================");
                            Console.WriteLine(results.ToString());
                            Console.WriteLine("====================");
                        }
                    }
                    break;
                }
            }

            Logger.Flush();
            // stopwatch.Stop();
            //Logger.Log(LogLevel.Info, $"Website generation completed. {count} pages created in {stopwatch.ElapsedMilliseconds}ms.");
        }

        private static void ErrorAndExit(string message)
        {
            Logger.Log(LogLevel.Error, message);
            Environment.Exit(1);
        }
    }
}


