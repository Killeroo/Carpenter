
namespace TemplatingTest;

public static class Extensions
{
    public static List<int> GetIndexesOf(this string str, string value)
    {
        int currentIndex = 0;
        List<int> indexes = new();
        while (currentIndex != -1) {
            currentIndex = str.IndexOf(value, currentIndex + value.Length, StringComparison.Ordinal);
            if (currentIndex != -1)
                indexes.Add(currentIndex);
        }

        return indexes;
    }
}

