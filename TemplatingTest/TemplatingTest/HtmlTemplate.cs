using System.Diagnostics;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Scripting;

namespace TemplatingTest;

public class HtmlTemplate<T>
{
    public static HtmlTemplate<T> Create(string pathToTemplateFile)
    {
        HtmlTemplate<T> template = new();
        
        
        template.Compile();
        
    }
    
    private Script<object> _compiledScripts;
    
    private string _rawCode;
    private string _output;
    
    public string Code => _rawCode;
    public string Output => _output;
    
    public void Compile(string code)
    {
        _rawCode = code;
        
    }
    
    public async string EvaluateAsync(T config)
    {
        return _compiledScripts.RunAsync();
    }
    
    private string GetCode()
    {
        
    }
    
    private void Run(T obj)
    {
        
    }
    
    
    
    const string kTemplateOpen = "<?";
    const string kTemplateOpenOutput = "<?=";
    const string kTemplateClose = "?>";
        
    // TODO: Cleanup
    const string kPreamble = @"string output = "";";
    const string kLineStart = "output += ";
    const string kLineEnd = @";"; // \n
    const string kStatementStart_NoOutput = "; ";
    const string kStatementStart_Output = "; output += ";
    const string kTerminator = ";";

    public void Load(string path)
    {
        foreach (string line in File.ReadAllLines("TestTemplate.html")) {
            string generatedLine = "";

            int statementStartIndex = line.IndexOf(kTemplateOpen, StringComparison.Ordinal);
            int statementEndIndex = line.IndexOf(kTemplateClose, StringComparison.Ordinal);
            if (statementStartIndex != -1 && statementEndIndex != -1) {
                List<int> openIndexes = line.GetIndexesOf(kTemplateOpen);
                List<int> closeIndexes = line.GetIndexesOf(kTemplateClose);
                Debug.Assert(openIndexes.Count == closeIndexes.Count);

                int currIndex = 0;
                for (int index = 0; index < openIndexes.Count; index++) {
                    int startIndex = openIndexes[index];
                    int endIndex = closeIndexes[index];
                    bool outputStatement = line.Length > openIndexes[index] + kTemplateOpen.Length &&
                                           line[startIndex + kTemplateOpen.Length] == '=';
                    int offset = kTemplateOpen.Length + (outputStatement ? 1 : 0);

                    string preStatement = line.Substring(currIndex, startIndex - currIndex); //index
                    string statement = line.Substring(startIndex + offset, (endIndex - startIndex) - offset);
                    currIndex = endIndex + 2;

                    generatedLine += kLineStart;
                    generatedLine += SymbolDisplay.FormatLiteral(preStatement, true);
                    generatedLine += (outputStatement ? kStatementStart_Output : kStatementStart_NoOutput) + statement +
                                     kTerminator;
                    // Console.WriteLine(outputStatement);
                    // Console.WriteLine($"pre " + preStatement);
                    // Console.WriteLine($"cur " + statement);
                    //
                    if (index == openIndexes.Count - 1) {
                        generatedLine += kLineStart;
                        generatedLine +=
                            SymbolDisplay.FormatLiteral(line.Substring(currIndex, line.Length - currIndex), true);
                        generatedLine += kLineEnd;
                    }
                }
            }
            else {
                generatedLine += kLineStart + SymbolDisplay.FormatLiteral(line, true) + kLineEnd;
            }


            generatedLine = generatedLine.Insert(generatedLine.Length - 2,
                SymbolDisplay.FormatLiteral(Environment.NewLine, false));
        }
    }
}