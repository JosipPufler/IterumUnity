using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using UnityEngine;

namespace Iterum.Scripts.Utils
{
    public class MarkDownService
    {
        public static string Convert(string markdown)
        {
            var lines = markdown.Split('\n');
            StringBuilder result = new StringBuilder();

            foreach (string rawLine in lines)
            {
                string line = rawLine.Trim();

                if (Regex.IsMatch(line, @"^#{1,6} "))
                {
                    int level = line.TakeWhile(c => c == '#').Count();
                    string content = line.Substring(level).Trim();
                    float[] headerSizes = { 200f, 150f, 120f, 100f, 90f, 80f };
                    float sizeMultiplier = headerSizes[Mathf.Clamp(level - 1, 0, headerSizes.Length - 1)];
                    result.AppendLine($"<size={sizeMultiplier}%><b>{content}</b></size>");
                }
                else if (Regex.IsMatch(line, @"^[-*] "))
                {
                    string content = line.Substring(2).Trim();
                    result.AppendLine($"• {ConvertInline(content)}");
                }
                else
                {
                    result.AppendLine(ConvertInline(line));
                }
            }

            return result.ToString();
        }

        private static string ConvertInline(string text)
        {
            text = Regex.Replace(text, @"\*\*(.+?)\*\*", "<b>$1</b>");
            text = Regex.Replace(text, @"(?<!\*)\*(?!\*)(.+?)(?<!\*)\*(?!\*)", "<i>$1</i>");
            return text;
        }
    }
}
