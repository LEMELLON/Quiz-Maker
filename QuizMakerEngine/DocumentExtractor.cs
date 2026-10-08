using System.IO.Compression;
using System.Xml.Linq;
using QuizMakerEngine.Models;
using UglyToad.PdfPig;

namespace QuizMakerEngine.Services;

public class DocumentExtractor
{
    public static List<DocumentPage> ExtractPdf(string filePath)
    {
        var pages = new List<DocumentPage>();
        using var document = PdfDocument.Open(filePath);

        foreach (var page in document.GetPages())
        {
            pages.Add(new DocumentPage
            {
                PageNumber = page.Number,
                Text = page.Text ?? string.Empty
            });
        }
        return pages;
    }

    public static List<DocumentPage> ExtractPptx(string filePath)
    {
        var pages = new List<DocumentPage>();
        using var zip = ZipFile.OpenRead(filePath);

        var slideEntries = zip.Entries
            .Where(e => System.Text.RegularExpressions.Regex.IsMatch(e.FullName, @"^ppt/slides/slide\d+\.xml$"))
            .OrderBy(e => int.Parse(System.Text.RegularExpressions.Regex.Match(e.FullName, @"\d+").Value))
            .ToList();

        int slideNumber = 1;
        XNamespace a = "http://schemas.openxmlformats.org/drawingml/2006/main";

        foreach (var entry in slideEntries)
        {
            using var stream = entry.Open();
            var doc = XDocument.Load(stream);

            // Extract all <a:t> text tags across paragraphs
            var textRuns = doc.Descendants(a + "t").Select(t => t.Value);
            string slideText = string.Join(" ", textRuns);

            pages.Add(new DocumentPage
            {
                PageNumber = slideNumber++,
                Text = slideText
            });
        }
        return pages;
    }
}