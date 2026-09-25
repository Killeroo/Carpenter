// using System.Diagnostics;
// using System.Text;
// using Matt.Tests;
// using Microsoft.CodeAnalysis.CSharp;
// using Microsoft.CodeAnalysis.CSharp.Scripting;
// using Microsoft.CodeAnalysis.Scripting;
//
// namespace Matt.Tests
// {
//     public static class TestUtils
//     {
//         public static void Print()
//         {
//             Console.WriteLine("Hello from TestUtils.Print()");
//         }
//     }
//     
//     public class AnotherTestUtils
//     {
//         public static void Print()
//         {
//             Console.WriteLine("Hello from TestUtils.Print()");
//         }
//     }
// }
//
// namespace TemplatingTest
// {
//     public class ScriptGlobals
//     {
//         // Local objects you want to pass in
//         public int CoreValue { get; set; }
//         public List<string> Items { get; set; } = new();
//     
//         // You can also pass functions/actions
//         public Action<string> Logger { get; set; }
//     }
//     
//     class Program
//     {
//         public static async Task Main(string[] args)
//         {
//
//             // var script = CSharpScript.RunAsync(@"
//             //         class MyClass
//             //         { 
//             //             public void Print() => System.Console.WriteLine(1);
//             //         }");
//             //
//             // await script.ContinueWithAsync("new MyClass().Print();");
//
//             // script.RunSynchronously();
//             
//             // AnotherTestUtils utils = new AnotherTestUtils();
//             // CSharpScript.EvaluateAsync("utils.Print();",
//             //     ScriptOptions.Default, utils);
//             
//             // 1. Initialize your local objects and the globals wrapper
//             var localItems = new List<string> { "Apple", "Banana", "Cherry" };
//             var globals = new ScriptGlobals
//             {
//                 CoreValue = 42,
//                 Items = localItems,
//                 Logger = msg => Console.WriteLine($"[Script Log]: {msg}")
//             };
//
// // 2. Configure the script options (import namespaces and assembly references)
//             var options = ScriptOptions.Default
//                 .WithReferences(typeof(ScriptGlobals).Assembly, typeof(List<string>).Assembly)
//                 .WithImports("System", "System.Collections.Generic", "System.Linq");
//
// // 3. Define the C# code snippet as a string
// // Note how it directly uses 'CoreValue', 'Items', and 'Logger'
//             string code = """
//                               Logger("Evaluating data...");
//                               var filtered = Items.Where(i => i.StartsWith("B")).ToList();
//                               return $"{filtered.FirstOrDefault()} found. Magic number is {CoreValue * 2}.";
//                           """;
//
//             try
//             {
//                 // 4. Run the code, explicitly passing the globals type and instance
//                 var result = await CSharpScript.EvaluateAsync<string>(
//                     code, 
//                     options, 
//                     globals: globals, 
//                     globalsType: typeof(ScriptGlobals)
//                 );
//
//                 Console.WriteLine($"Result: {result}");
//             }
//             catch (CompilationErrorException e)
//             {
//                 Console.WriteLine("Compile errors:");
//                 Console.WriteLine(string.Join(Environment.NewLine, e.Diagnostics));
//             }
//             
//             
//             string code2 = """
//                               Logger("Evaluating data...");
//                               var filtered = 2;
//                               return $"found. Magic number is {CoreValue * filtered}.";
//                           """;
//             
//             
//             Script<string> compiledScript = CSharpScript.Create<string>(code2, options, globalsType: typeof(ScriptGlobals));
//             compiledScript.Compile();
//             
//             var instanceA = new ScriptGlobals { CoreValue = 10 };
//             string resultA = (await compiledScript.RunAsync(instanceA)).ReturnValue;
//
//             var instanceB = new ScriptGlobals { CoreValue = 99 };
//             string resultB = (await compiledScript.RunAsync(instanceB)).ReturnValue;
//         }
//     }
// }
//
//
//
//
//
using System;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace RoslynScriptRunner
{
    // 1. Define a strongly-typed class to act as the global context (host object)
    public class PlayerContext
    {
        public string Name { get; set; }
        public int Health { get; set; }
        public int Score { get; set; }

        public void RewardPoints(int points)
        {
            Score += points;
            Console.WriteLine($"[Method] {Name} gained {points} points. New Score: {Score}");
        }
    }

    public class PageData
    {
        public string Name = "Something";
        public int Pics;
    }

    class Program
    {
        static async Task Main(string[] args)
        {

            //Console.WriteLine(HtmlTemplateUtils.ConvertTemplateFileToCode("TestTemplate.html"));
            var template = HtmlTemplateUtils.Create<PageData>("TestTemplate.html");
            Console.WriteLine(await template.EvaluateAsync(new PageData() { Name = "Hello", Pics = 2 }));
            return;

            // 2. Define the script as a string
            // Properties and methods of the globals object are directly accessible in the script
            string scriptCode = @"
                Health += 20; 
                RewardPoints(10);
                return $""[Script] Processed {Name}: Health={Health}, Score={Score}"";
            ";

            Console.WriteLine("Compiling script...");

            // 3. Compile the script once
            // We pass PlayerContext as the globals type so the compiler knows the available properties/methods
            ScriptOptions options = ScriptOptions.Default.AddImports("System");
            Script<object> compiledScript = CSharpScript.Create<object>(scriptCode, options, globalsType: typeof(PlayerContext));
            
            // Explicitly compile/verify to catch errors early and optimize subsequent runs
            compiledScript.Compile(); 

            Console.WriteLine("Compilation successful.\n");

            // 4. Create different instances of the object
            var player1 = new PlayerContext { Name = "Alice", Health = 80, Score = 100 };
            var player2 = new PlayerContext { Name = "Bob", Health = 45, Score = 250 };
            var player3 = new PlayerContext { Name = "Charlie", Health = 10, Score = 5 };

            // 5. Run the same compiled script against each instance
            Console.WriteLine("--- Running Script for Player 1 ---");
            var result = await compiledScript.RunAsync(globals: player1);
            Console.WriteLine(result.ReturnValue);

            Console.WriteLine("\n--- Running Script for Player 2 ---");
            await compiledScript.RunAsync(globals: player2);

            Console.WriteLine("\n--- Running Script for Player 3 ---");
            await compiledScript.RunAsync(globals: player3);

            // 6. Verify that the original objects were modified
            Console.WriteLine("\n--- Final Object States ---");
            Console.WriteLine($"{player1.Name} - Health: {player1.Health}, Score: {player1.Score}");
            Console.WriteLine($"{player2.Name} - Health: {player2.Health}, Score: {player2.Score}");
            Console.WriteLine($"{player3.Name} - Health: {player3.Health}, Score: {player3.Score}");
        }
    }
}