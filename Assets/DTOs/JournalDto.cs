using System;

namespace Iterum.DTOs
{
    [Serializable]
    public class JournalDto
    {
        public JournalDto(string name, string content)
        {
            Name = name;
            Content = content;
            LastModified = DateTime.Now;
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public DateTime? LastModified { get; set; }
    }
}
