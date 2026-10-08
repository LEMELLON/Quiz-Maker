using System.Text.RegularExpressions;

namespace QuizMakerEngine.Services;

public static class TextHelpers
{
    public static string CleanExtractedText(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return string.Empty;
        text = Regex.Replace(text, @"(\w)-\s*\n\s*(\w)", "$1$2");
        text = Regex.Replace(text, @"(\w)-\s+(\w)", "$1$2");
        return Regex.Replace(text, @"\s+", " ").Trim();
    }

    public static string ToClampPrompt(string sentence, string targetTerm)
    {
        if (string.IsNullOrWhiteSpace(sentence)) return string.Empty;
        if (string.IsNullOrWhiteSpace(targetTerm)) return sentence;

        string cleanTerm = Regex.Escape(targetTerm.Trim());
        string pattern = $@"^(?:the|an?)\s+{cleanTerm}\b\s*|^{cleanTerm}\b\s*";
        var match = Regex.Match(sentence.Trim(), pattern, RegexOptions.IgnoreCase);

        return match.Success ? "___ " + sentence.Trim()[match.Length..] : "___ " + sentence.Trim();
    }

    public static string NormalizeTermKey(string term)
    {
        if (string.IsNullOrWhiteSpace(term)) return string.Empty;
        var stripped = Regex.Replace(term, @"[.,;:!?""'`\(\)\[\]{}\-–—…/\\]", "");
        return Regex.Replace(stripped, @"\s+", " ").Trim().ToLowerInvariant();
    }
}