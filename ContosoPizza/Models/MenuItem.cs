namespace ContosoPizza.Models;

// Eine Pizza-Sorte in der Speisekarte, gruppiert über alle verfügbaren
// Grössen hinweg (jede Grösse ist im Katalog eine eigene Pizza-Zeile mit
// eigenem Preis, siehe Models/Pizza.cs).
public class MenuItem
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsGlutenFree { get; set; }
    public List<Pizza> Variants { get; set; } = new();

    public static string EmojiFor(string name)
    {
        var lower = name.ToLowerInvariant();
        if (lower.Contains("margherita")) return "🍅";
        if (lower.Contains("salami") || lower.Contains("diavola")) return "🌶️";
        if (lower.Contains("funghi") || lower.Contains("pilz")) return "🍄";
        if (lower.Contains("formaggi") || lower.Contains("käse")) return "🧀";
        if (lower.Contains("vegetaria") || lower.Contains("gemüse")) return "🥦";
        if (lower.Contains("hawaii")) return "🍍";
        return "🍕";
    }
}
