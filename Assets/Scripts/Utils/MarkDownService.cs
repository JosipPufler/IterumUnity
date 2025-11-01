using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

namespace Iterum.Scripts.Utils
{
    public static class MarkdownService
    {

        private static readonly float[] headerSizes = { 200f, 150f, 120f, 100f, 90f, 80f };
        private static readonly char lineSeparator = '\n';

        public static string Convert(string markdown)
        {
            var lines = markdown.Split(lineSeparator);
            StringBuilder result = new();

            foreach (string rawLine in lines) {
                string line = rawLine.Trim();
                if (Regex.IsMatch(line, @"^#{1,6} "))
                {
                    int level = line.TakeWhile(c => c == '#').Count();
                    string content = line[level..].Trim();
                    float sizeMultiplier = headerSizes[Mathf.Clamp(level - 1, 0, headerSizes.Length - 1)];
                    result.AppendLine($"<size={sizeMultiplier}%><b>{content}</b></size>");
                }
                else if (Regex.IsMatch(line, @"^-+[*] ")) {
                    Match match = Regex.Match(line, @"^(?<dashes>-+)\* (?<content>.+)");
                    int indentLevel = match.Groups["dashes"].Value.Length;
                    string content = match.Groups["content"].Value.Trim();

                    string indent = new(' ', indentLevel * 2);
                    result.AppendLine($"{indent}• {ConvertInline(content)}");
                }
                else {
                    result.AppendLine(ConvertInline(line));
                }
            }

            return result.ToString();
        }

        private static string ConvertInline(string text)
        {
            text = Regex.Replace(text, @"_(\*(.+?)\*)_", "<i><b>$2</b></i>");
            text = Regex.Replace(text, @"\*(_(.+?)_)\*", "<b><i>$2</i></b>");

            text = Regex.Replace(text, @"\*(.+?)\*", "<b>$1</b>");

            text = Regex.Replace(text, @"_(.+?)_", "<i>$1</i>");
            return text;
        }
    }
}
