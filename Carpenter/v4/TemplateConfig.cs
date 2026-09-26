using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Carpenter;

public class TemplateConfig
{
    public static readonly int kCurrentVersion = 1;

    public int Version { get; set; } = kCurrentVersion;
    public string PathToTemplateFile { get; set; } = "";
    public TemplateData Data { get; set; } = new();

    public void ToFile(string path)
    {
        JsonSerializerOptions options = new() { WriteIndented = true };
        string outputJson = JsonSerializer.Serialize(this, options);
        File.WriteAllText(path, outputJson);
    }

    public void FromFile(string path)
    {
        string fileContents = File.ReadAllText(path);
        TemplateConfig? config = JsonSerializer.Deserialize<TemplateConfig>(fileContents);
        if (config != null)
            CopyFrom(config);
    }

    public void CopyFrom(TemplateConfig other)
    {
        Version = other.Version;
        PathToTemplateFile = other.PathToTemplateFile;
        Data.CopyFrom(other.Data);
    }
}

public class TemplateData
{
    public Dictionary<string, string> Values { get; set; } = new();
    public Dictionary<string, List<TemplateData>> Collections { get; set; } = new();

    public void CopyFrom(TemplateData other)
    {
        Values = other.Values;
        Collections = other.Collections;
    }
}