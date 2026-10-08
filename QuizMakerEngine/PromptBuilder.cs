using QuizMakerEngine.Models;

namespace QuizMakerEngine.Services;

public static class PromptBuilder
{
    private static readonly Dictionary<int, string> TierFocus = new()
    {
        [1] = "This is PASS 1 of a 3-pass scan — TIER 1 (top-tier essentials). Scan the ENTIRE document and identify only the small handful of most important, high-level terms (3-10 total).",
        [2] = "This is PASS 2 of a 3-pass scan — TIER 2 (bullet-by-bullet detail). Evaluate EVERY substantive bullet point. Classify each as 'new' (novel term) or 'extension' (adds detail to an existing term).",
        [3] = "This is PASS 3 of a 3-pass scan — TIER 3 (final sweep). Pick up remaining minor details, asides, or facts missed by Passes 1 and 2."
    };

    public static string BuildSystemPrompt(int tier, List<string>? existingTerms = null)
    {
        string definitionTypeSchema = tier == 2 ? "\"definitionType\": \"new\" or \"extension\",\n" : "";
        string exclusionBlock = "";

        if (existingTerms != null && existingTerms.Count > 0)
        {
            exclusionBlock = $"\n\nCRITICAL: Avoid re-minting new definitions for already processed terms:\n" +
                             string.Join("\n", existingTerms.Select(t => $"- {t}"));
        }

        return $@"You are a quiz-writing assistant embedded in a document-to-quiz tool.
{TierFocus[tier]}

Return ONLY a single valid JSON object in this exact shape:
{{
  ""collections"": [
    {{
      ""targetTerm"": ""key term"",
      ""sourcePage"": 1,
      ""questionSnippet"": ""exact sentence verbatim from source"",
      ""answerSnippet"": ""exact term verbatim from source"",
      {definitionTypeSchema}""variants"": {{
        ""clamp"": {{
          ""sentence"": ""TargetTerm is the definition..."",
          ""choices"": [""TargetTerm"", ""Wrong1"", ""Wrong2"", ""Wrong3""],
          ""correctAnswerIndex"": 0
        }}
      }}
    }}
  ]
}}
Rules:
- Exact verbatim substrings for snippets.
- Clamp sentence must have target term as the leading subject.
- Return ONLY valid JSON, no markdown code blocks.{exclusionBlock}";
    }

    public static string BuildUserPagesPrompt(List<DocumentPage> pages)
    {
        return string.Join("\n\n", pages.Select(p => $"--- Page {p.PageNumber} ---\n{p.Text.Trim()}"));
    }
}