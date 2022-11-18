//https://stackoverflow.com/questions/24643408/how-to-do-on-the-fly-image-compression-in-c
//https://developer.mozilla.org/en-US/docs/Web/CSS/object-fit
//https://stackoverflow.com/questions/19414856/how-can-i-make-all-images-of-different-height-and-width-the-same-via-css

using System.Drawing.Imaging;
using System.Drawing;

// Load the schema file
string pathToSchemaFile = "SCHEMA";
string[] schemaFileContents;
try
{
    schemaFileContents = File.ReadAllLines(pathToSchemaFile);
    DebugLog($"Schema file loaded ({pathToSchemaFile})");
}
catch (Exception e)
{
    DebugError($"[{e.GetType()}] Could not read schema file - {e.Message}");
    return;
}

// Parse schema
enum SchemaTokens
{
    BaseUrl,
    PageUrl,
    Location,
    Title,
    Month,
    Year,
    Author,
    Camera,
    ClassIdImageGrid,
    ClassIdImageColumn,
    ClassIdImageElement,
    Image,
    DetailedImage
};

Dictionary<SchemaTokens, string> tokenDictionary = new()
{
    { SchemaTokens.BaseUrl, "%BASE_URL" },
};

for (int i = 0; i < schemaFileContents.Length; i++)
{
    DebugLog(schemaFileContents[i]);
}

// Load the template file
// TODO: We should do this first
string pathToTemplateFile = "template.html";
string[] templateFileContents;
try
{
    templateFileContents = File.ReadAllLines(pathToTemplateFile);
    DebugLog($"Template file loaded ({pathToTemplateFile})");
}
catch (Exception e)
{
    DebugError($"[{e.GetType()}] Could not read template file - {e.Message}");
    return;
}

// Parse the template file
for (int i = 0; i < templateFileContents.Length; i++)
{
    DebugLog(templateFileContents[i]);
}
for (int i = 0; i < schemaFileContents.Length; i++)
{
    DebugLog(schemaFileContents[i]);
}
return;


string directory = Directory.GetCurrentDirectory();
string[] images = Directory.GetFiles(directory, "*.jpg");

foreach (string jpeg in images)
{
    using (Image originalImage = Image.FromFile(jpeg))
    {
        
    }
}


void DebugLog(string message)
{
    Console.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff")}] {message}");
    Console.ResetColor();
}

void DebugError(string message)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine($"[{DateTime.Now.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss'.'ffff")}] {message}");
    Console.ResetColor();
}