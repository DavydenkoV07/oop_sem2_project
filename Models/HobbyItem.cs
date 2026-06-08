using System;

namespace HobbyTracker.Models
{
    public class HobbyItem
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int? ReleaseYear { get; set; }
        public string PersonalComment { get; set; }
        public string Link { get; set; }
        public ItemStatus Status { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.Now;
    }
}