using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace Carpenter
{
    public static class HtmlTemplateUtils
    {
        private const string kTemplateOpen = "<?";
        private const string kTemplateClose = "?>";
        private const char kTemplateOutputChar = '=';

        private const string kCodeStart = "string output = \"\";";
        private const string kCodeEnd = @"return output;";
        private const string kCodeAddToOutput = @"output += ";
        private const string kCodeTerminator = @";";

        public static HtmlTemplate<T> Create<T>(string pathToTemplateFile)
        {
            HtmlTemplate<T> template = new();
            string code = ConvertTemplateFileToCode(pathToTemplateFile);
            template.Compile(code);

            return template;
        }

        public static string ConvertTemplateFileToCode(string path)
        {
            string code = kCodeStart + Environment.NewLine;

            foreach (string rawLine in File.ReadAllLines(path))
            {
                if (rawLine.Contains(kTemplateOpen, StringComparison.Ordinal)
                    && rawLine.Contains(kTemplateClose, StringComparison.Ordinal))
                {
                    List<int> openIndexes = rawLine.GetIndexesOf(kTemplateOpen);
                    List<int> closeIndexes = rawLine.GetIndexesOf(kTemplateClose);
                    Debug.Assert(openIndexes.Count == closeIndexes.Count);

                    int currIndex = 0;
                    for (int index = 0; index < openIndexes.Count; index++)
                    {
                        int startIndex = openIndexes[index];
                        int endIndex = closeIndexes[index];
                        bool outputStatement = rawLine.Length > openIndexes[index] + kTemplateOpen.Length &&
                                               rawLine[startIndex + kTemplateOpen.Length] == kTemplateOutputChar;
                        int offset = kTemplateOpen.Length + (outputStatement ? 1 : 0);

                        string preStatement = rawLine.Substring(currIndex, startIndex - currIndex);
                        string statement = rawLine.Substring(startIndex + offset, (endIndex - startIndex) - offset);
                        currIndex = endIndex + 2;

                        code += kCodeAddToOutput + preStatement.ToLiteral() + kCodeTerminator;
                        code += (outputStatement ? kCodeAddToOutput : "") + statement + kCodeTerminator;

                        if (index == openIndexes.Count - 1)
                        {
                            string remainingLine = rawLine.Substring(currIndex, rawLine.Length - currIndex);
                            code += kCodeAddToOutput + remainingLine.ToLiteral() + kCodeTerminator;
                        }
                    }
                }
                else
                {
                    code += kCodeAddToOutput + rawLine.ToLiteral() + kCodeTerminator;
                }

                code += kCodeAddToOutput + Environment.NewLine.ToLiteral() + kCodeTerminator + Environment.NewLine;
            }

            code += kCodeEnd + Environment.NewLine;
            return code;
        }
    }

    public class HtmlTemplate<T>
    {
        public string Code => _rawCode;
        public string Output => _output;

        private Script<object> _compiledScript;
        private string _rawCode;
        private string _output;

        public void Compile(string code)
        {
            if (_rawCode.IsEmptyOrNull() && code == _rawCode)
            {
                return;
            }

            ScriptOptions options = ScriptOptions.Default.AddImports("System", "System.Text");
            _compiledScript = CSharpScript.Create<object>(code, options, globalsType: typeof(T));
            _compiledScript.Compile();

            _rawCode = code;
        }

        public async Task<string> EvaluateAsync(T config)
        {
            ScriptState<object> result = await _compiledScript.RunAsync(config);
            _output = result.ReturnValue as string;
            return _output;
        }
        public string Evaluate(T config)
        {
            return Task.Run(() => EvaluateAsync(config)).GetAwaiter().GetResult();
        }
    }
}